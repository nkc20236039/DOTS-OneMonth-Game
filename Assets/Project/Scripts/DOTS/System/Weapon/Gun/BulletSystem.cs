using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct BulletSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            state.Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(),
                EnemyGroup = SystemAPI.GetComponentLookup<EnemyHomingComponent>(),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(),
            }.Schedule(simulation, state.Dependency);

            state.Dependency.Complete();

            // ƒWƒ‡ƒu‚ğƒXƒPƒWƒ…[ƒ‹
            state.Dependency = new BulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ParallelEcb = ecb.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);


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
                transform.Position += math.forward(transform.Rotation) * bullet.Speed;
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
        public ComponentLookup<EnemyHomingComponent> EnemyGroup;
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
            bool isEnemyHitAtoB
                = EnemyGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnemyHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnemyGroup.HasComponent(triggerEvent.EntityB);

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

            if (isEnemyHitAtoB || isEnemyHitBtoA)
            {
                Ecb.DestroyEntity(triggerEvent.EntityA);
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
        }
    }
}