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
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

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

            // ecb‚ÌŒãˆ—
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
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
                = EnvironmentGroup.EntityExists(triggerEvent.EntityA)
                && BulletGroup.EntityExists(triggerEvent.EntityB);
            bool isEnvironmentHitBtoA
                = BulletGroup.EntityExists(triggerEvent.EntityA)
                && EnvironmentGroup.EntityExists(triggerEvent.EntityB);
            // “G‚Æ’e‚ª“–‚½‚Á‚½
            bool isEnemyHitA
                = EnemyGroup.EntityExists(triggerEvent.EntityA)
                || BulletGroup.EntityExists(triggerEvent.EntityA);
            bool isEnemyHitB
                = EnemyGroup.EntityExists(triggerEvent.EntityB)
                || BulletGroup.EntityExists(triggerEvent.EntityB);
            bool isEnemyHit = isEnemyHitA && isEnemyHitB;

            // ŠÂ‹«‚Æ’e‚ª“–‚½‚Á‚½‚ç’e‚ğíœ
            if (isEnvironmentHitAtoB)
            {
                // B‚ªe’e‚Æ‚í‚©‚é‚½‚ßA‚ğíœ
                Ecb.DestroyEntity(triggerEvent.EntityA);
            }
            else if (isEnvironmentHitBtoA)
            {
                // A‚ªe’e‚Æ‚í‚©‚é‚½‚ßB‚ğíœ
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }

            if (isEnemyHit)
            {
                Ecb.DestroyEntity(triggerEvent.EntityA);
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
        }
    }
}