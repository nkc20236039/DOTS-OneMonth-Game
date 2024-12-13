using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityPhysicsExpansion;

namespace DOTS
{
    [BurstCompile]
    public partial struct BulletSystem : ISystem
    {
        private ComponentLookup<LocalTransform> transformGroup;
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<BulletParameterComponent>();
            state.RequireForUpdate<HealthComponent>();
            transformGroup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // Jobに渡すものの準備
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            transformGroup.Update(ref state);

            // Jobの後処理
            state.Dependency.Complete();

            // 生存中の弾を管理するJobを作成
            state.Dependency = new BulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                TransformGroup = transformGroup,
                ParallelEcb = ecb.AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);
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
        [ReadOnly] public ComponentLookup<LocalTransform> TransformGroup;

        private void Execute(
            [EntityIndexInQuery] int index,
            Entity entity,
            in BulletComponent bullet,
            ref BulletParameterComponent bulletParameter)
        {
            if (TransformGroup.TryGetComponent(entity, out var transform) == false) { return; }

            // 対象がいればトラッキングする
            if (bullet.Target != Entity.Null && TransformGroup.TryGetComponent(bullet.Target, out var target))
            {
                var direction = math.normalize(target.Position - transform.Position);

                transform.Rotation = quaternion.LookRotationSafe(direction, math.up());
            }

            // 時間を経過させる
            bulletParameter.Age += DeltaTime;
            if (bulletParameter.Lifetime > bulletParameter.Age)
            {
                // 生存期間は直進させる
                transform.Position += math.forward(TransformGroup[entity].Rotation) * bulletParameter.Speed * DeltaTime;

                ParallelEcb.SetComponent(index, entity, transform);
            }
            else
            {
                // 生存時間を過ぎたら削除する
                ParallelEcb.DestroyEntity(index, entity);
            }
        }
    }
}