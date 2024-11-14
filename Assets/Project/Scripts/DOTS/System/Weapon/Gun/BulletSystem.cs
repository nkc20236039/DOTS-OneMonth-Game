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

            // ジョブをスケジュール
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
    /// 銃弾の基本挙動
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
            // 時間を経過させる
            bullet.Age += DeltaTime;
            if (bullet.Lifetime > bullet.Age)
            {
                // 生存期間は直進させる
                transform.Position += math.forward(transform.Rotation) * bullet.Speed;
            }
            else
            {
                // 生存時間を過ぎたら削除する
                ParallelEcb.DestroyEntity(index, entity);
            }
        }
    }

    /// <summary>
    /// 銃弾の衝突判定
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
            /*必要な衝突情報のboolを設定*/
            // 環境と弾が当たった
            bool isEnvironmentHitAtoB
                = EnvironmentGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnvironmentHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnvironmentGroup.HasComponent(triggerEvent.EntityB);
            // 敵と弾が当たった
            bool isEnemyHitAtoB
                = EnemyGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnemyHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnemyGroup.HasComponent(triggerEvent.EntityB);

            // 環境と弾が当たったら弾を削除
            if (isEnvironmentHitAtoB)
            {
                // Bが銃弾とわかるためBを削除
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
            else if (isEnvironmentHitBtoA)
            {
                // Aが銃弾とわかるためAを削除
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