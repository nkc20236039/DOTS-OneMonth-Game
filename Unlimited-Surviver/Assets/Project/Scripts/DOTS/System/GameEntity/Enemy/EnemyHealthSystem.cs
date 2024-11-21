using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

            // 死亡処理
            // JobのQuery作成
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<EnemyTag, LocalTransform, HealthComponent, ExperienceOrbDropComponent>()
                .Build(state.EntityManager);

            state.Dependency = new EnemyDieJob
            {
                ParallelEcb = ecb.AsParallelWriter(),
                TransformGroup = SystemAPI.GetComponentLookup<LocalTransform>(),
            }.ScheduleParallel(entityQuery, state.Dependency);

            state.Dependency.Complete();
        }
    }

    public partial struct EnemyDieJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ParallelEcb;
        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformGroup;

        private void Execute(
            [ChunkIndexInQuery] int index,
            Entity entity,
            in LocalTransform transform,
            in HealthComponent health,
            in ExperienceOrbDropComponent expOrbDrop)
        {
            if (0 < health.Health) { return; }
            if (TransformGroup.HasComponent(expOrbDrop.ExperienceOrb) == false) { return; }

            // 経験値をドロップする
            for (int i = 0; i < expOrbDrop.SpawnAmount; i++)
            {
                var orb = ParallelEcb.Instantiate(index, expOrbDrop.ExperienceOrb);
                var dropPosition = transform.Position;
                dropPosition.y += 1f;
                ParallelEcb.SetComponent(index, orb, new LocalTransform
                {
                    Position = dropPosition,
                    Scale = TransformGroup[expOrbDrop.ExperienceOrb].Scale,
                    Rotation = TransformGroup[expOrbDrop.ExperienceOrb].Rotation,
                });
            }

            // 体力が無くなったEntityを削除
            ParallelEcb.DestroyEntity(index, entity);
        }
    }
}