using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    [BurstCompile]
    public partial struct BulletSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
            state.RequireForUpdate<HealthComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // Job‚É“n‚·‚à‚Ì‚Ì€”õ
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // Õ“Ë”»’è‚ÌJob‚ğì¬
            state.Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(),
                HealthGroup = SystemAPI.GetComponentLookup<HealthComponent>(),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(),
            }.Schedule(simulation, state.Dependency);

            // Õ“Ë”»’èJob‚ªI—¹‚·‚é‚±‚Æ‚ğ‘Ò‹@
            state.Dependency.Complete();

            // ¶‘¶’†‚Ì’e‚ğŠÇ—‚·‚éJob‚ğì¬
            state.Dependency = new BulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ParallelEcb = ecb.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            // Job‚ÌŒãˆ—
            state.Dependency.Complete();
            JobHandle.ScheduleBatchedJobs();
        }
    }

    /// <summary>
    /// e’e‚ÌŠî–{‹““®
    /// </summary>
    [BurstCompile]
    public partial struct BulletJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ParallelEcb;

        private void Execute(
            [EntityIndexInQuery] int index,
            Entity entity,
            ref BulletComponent bullet,
            ref LocalTransform transform)
        {
            // ŠÔ‚ğŒo‰ß‚³‚¹‚é
            bullet.Age += DeltaTime;
            if (bullet.Lifetime > bullet.Age)
            {
                // ¶‘¶ŠúŠÔ‚Í’¼i‚³‚¹‚é
                transform.Position += math.forward(transform.Rotation) * bullet.Speed * DeltaTime;
            }
            else
            {
                // ¶‘¶ŠÔ‚ğ‰ß‚¬‚½‚çíœ‚·‚é
                ParallelEcb.DestroyEntity(index, entity);
            }
        }
    }

    /// <summary>
    /// e’e‚ÌÕ“Ë”»’è
    /// </summary>
    [BurstCompile]
    public partial struct BulletTriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly]
        public ComponentLookup<EnvironmentTag> EnvironmentGroup;
        public ComponentLookup<HealthComponent> HealthGroup;
        public ComponentLookup<BulletComponent> BulletGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            /*•K—v‚ÈÕ“Ëî•ñ‚Ìbool‚ğİ’è*/
            // ŠÂ‹«‚Æ’e‚ª“–‚½‚Á‚½
            bool isEnvironmentHitAtoB
                = EnvironmentGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnvironmentHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnvironmentGroup.HasComponent(triggerEvent.EntityB);
            // “G‚Æ’e‚ª“–‚½‚Á‚½
            bool isHealthHitAtoB
                = HealthGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isHealthHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && HealthGroup.HasComponent(triggerEvent.EntityB);

            // ŠÂ‹«‚Æ’e‚ª“–‚½‚Á‚½‚ç’e‚ğíœ
            if (isEnvironmentHitAtoB)
            {
                // B‚ªe’e‚Æ‚í‚©‚é‚½‚ßB‚ğíœ
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
            else if (isEnvironmentHitBtoA)
            {
                // A‚ªe’e‚Æ‚í‚©‚é‚½‚ßA‚ğíœ
                Ecb.DestroyEntity(triggerEvent.EntityA);
            }

            if (isHealthHitAtoB)
            {
                // •K—v‚ÈƒRƒ“ƒ|[ƒlƒ“ƒg‚ğæ“¾
                HealthComponent _health;
                BulletComponent _bullet;
                if (HealthGroup.TryGetComponent(triggerEvent.EntityA, out _health) == false) { return; }
                if (BulletGroup.TryGetComponent(triggerEvent.EntityB, out _bullet) == false) { return; }


                // “–‚½‚Á‚½‚ç‘Šè‚Ì‘Ì—Í‚ğŒ¸‚ç‚·
                _health = Attack(triggerEvent.EntityB, _health, _bullet.AttackDamage);
                Ecb.SetComponent(triggerEvent.EntityA, _health);
            }
            else if (isHealthHitBtoA)
            {
                // •K—v‚ÈƒRƒ“ƒ|[ƒlƒ“ƒg‚ğæ“¾
                HealthComponent health;
                BulletComponent bullet;
                if (BulletGroup.TryGetComponent(triggerEvent.EntityA, out bullet) == false) { return; }
                if (HealthGroup.TryGetComponent(triggerEvent.EntityB, out health) == false) { return; }

                // “–‚½‚Á‚½‚ç‘Šè‚Ì‘Ì—Í‚ğŒ¸‚ç‚·
                health = Attack(triggerEvent.EntityA, health, bullet.AttackDamage);
                Ecb.SetComponent(triggerEvent.EntityB, health);
            }
        }

        private (bool isHit, Entity entityA, Entity entityB) TriggerEventExplicit<EntityA, EntityB>(
            TriggerEvent triggerEvent,
            ComponentLookup<EntityA> entityA,
            ComponentLookup<EntityB> entityB)
            where EntityA : unmanaged, IComponentData
            where EntityB : unmanaged, IComponentData
        {
            if (EnvironmentGroup.HasComponent(triggerEvent.EntityA) && BulletGroup.HasComponent(triggerEvent.EntityB))
            {

            }

            return default;
        }

        private HealthComponent Attack(Entity bullet, HealthComponent health, int damage)
        {
            Ecb.DestroyEntity(bullet);
            health.Health -= damage;

            return health;
        }
    }
}