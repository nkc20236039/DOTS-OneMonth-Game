using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct EnemyHealthSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<EnemyTag>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((var hitDamage, var health, var entity) in SystemAPI
                .Query<
                    RefRW<HitDamageComponent>,
                    RefRW<HealthComponent>>()
                .WithAll<EnemyTag>()
                .WithEntityAccess())
            {
                // ダメージを受けていれば体力を減少させる
                bool isDamageEnable = state.EntityManager
                    .IsComponentEnabled<HitDamageComponent>(entity);
                if (isDamageEnable && !hitDamage.ValueRO.IsDistributed)
                {
                    health.ValueRW.Health -= hitDamage.ValueRO.DamageValue;
                    hitDamage.ValueRW.IsDistributed = true;
                }
            }

            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<EnemyTag, LocalTransform, HealthComponent, ExperienceOrbDropComponent>()
                .Build(state.EntityManager);

            state.Dependency = new EnemyDieJob
            {
                ParallelEcb = ecb.AsParallelWriter(),
            }.ScheduleParallel(entityQuery, state.Dependency);

            state.Dependency.Complete();
        }
    }

    public partial struct EnemyDieJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ParallelEcb;

        private void Execute(
            [ChunkIndexInQuery] int index,
            Entity entity,
            in LocalTransform transform,
            in HealthComponent health,
            in ExperienceOrbDropComponent exOrbDrop)
        {
            if (0 < health.Health) { return; }

            // 経験値をドロップする
            for (int i = 0; i < exOrbDrop.SpawnAmount; i++)
            {
                var orb = ParallelEcb.Instantiate(index, exOrbDrop.ExperienceOrb);

                ParallelEcb.SetComponent(index, orb, new LocalTransform
                {
                    Position = transform.Position,
                    Scale = 1,
                });
            }

            // 体力が無くなったEntityを削除
            ParallelEcb.DestroyEntity(index, entity);
        }
    }
}