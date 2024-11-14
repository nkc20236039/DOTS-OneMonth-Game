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
                GameEntityGroup = SystemAPI.GetComponentLookup<HealthComponent>(),
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
        [ReadOnly] public ComponentLookup<EnvironmentTag> EnvironmentGroup;
        [ReadOnly] public ComponentLookup<HealthComponent> GameEntityGroup;
        [ReadOnly] public ComponentLookup<BulletComponent> BulletGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            // ŠÂ‹«‚Æ’e
            var environmentInfo = TriggerEventExplicit(triggerEvent, EnvironmentGroup, BulletGroup);
            // ƒQ[ƒ€ƒGƒ“ƒeƒBƒeƒB‚Æ’e
            var gameEntityInfo = TriggerEventExplicit(triggerEvent, GameEntityGroup, BulletGroup);

            if (environmentInfo.IsHit)
            {
                // ŠÂ‹«‚Æ’e‚ª“–‚½‚Á‚½ê‡
                // B‚ªe’e‚Æ‚í‚©‚é‚½‚ßB‚ğíœ
                Ecb.DestroyEntity(environmentInfo.EntityB);
            }

            if (gameEntityInfo.IsHit)
            {
                // •K—v‚ÈƒRƒ“ƒ|[ƒlƒ“ƒg‚ğæ“¾
                HealthComponent health;
                BulletComponent bullet;
                if (GameEntityGroup.TryGetComponent(gameEntityInfo.EntityA, out health) == false) { return; }
                if (BulletGroup.TryGetComponent(gameEntityInfo.EntityB, out bullet) == false) { return; }

                // “–‚½‚Á‚½‚ç‘Šè‚Ì‘Ì—Í‚ğŒ¸‚ç‚·
                // TODO: ˆø”‚Æ–ß‚è’l‚ÌŠÖŒW«‚ª•ª‚©‚è‚É‚­‚¢‚Ì‚ÅC³‚·‚é
                health = Attack(gameEntityInfo.EntityB, health, bullet.AttackDamage);
                Ecb.SetComponent(gameEntityInfo.EntityA, health);
            }
        }

        private (bool IsHit, Entity EntityA, Entity EntityB) TriggerEventExplicit<EntityA, EntityB>(
            TriggerEvent triggerEvent,
            ComponentLookup<EntityA> entityA,
            ComponentLookup<EntityB> entityB)
            where EntityA : unmanaged, IComponentData
            where EntityB : unmanaged, IComponentData
        {
            if (entityA.HasComponent(triggerEvent.EntityA) && entityB.HasComponent(triggerEvent.EntityB))
            {
                return (true, triggerEvent.EntityA, triggerEvent.EntityB);
            }
            if (entityA.HasComponent(triggerEvent.EntityB) && entityB.HasComponent(triggerEvent.EntityA))
            {
                return (true, triggerEvent.EntityB, triggerEvent.EntityA);
            }

            return (false, Entity.Null, Entity.Null);
        }

        private HealthComponent Attack(Entity bullet, HealthComponent health, int damage)
        {
            Ecb.DestroyEntity(bullet);
            health.Health -= damage;

            return health;
        }
    }
}