using DOTStoMono;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.Windows.Speech;

namespace DOTS
{
    [BurstCompile]
    public partial struct PistolSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<PistolComponent, LocalTransform>()
                .Build();

            state.RequireForUpdate(query);
            state.RequireForUpdate<EnhancementBuffer>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // ECBの準備
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            // 発射間隔の取得
            var shotInterval = EnhancemetTypeCollection
                .GetEnhancementValue(EnhancementContents.PistolFireRate, state.EntityManager);
            // 取得できなければ処理を終了
            if (shotInterval == false) { return; }

            state.Dependency = new PistolJob
            {
                ShotInterval = shotInterval.Value,
                ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
                ParallelEcb = ecb.AsParallelWriter(),
                PhysicsWorld = physicsWorldSingleton.PhysicsWorld,
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }
    }

    [BurstCompile]
    public partial struct PistolJob : IJobEntity
    {
        private const int HIT_FILTER = 1 << 1;  // Raycastがヒットするレイヤー

        public float ElapsedTime;
        public float ShotInterval;
        public EntityCommandBuffer.ParallelWriter ParallelEcb;
        [ReadOnly] public PhysicsWorld PhysicsWorld;

        private void Execute(
            [EntityIndexInQuery] int index,
            Entity entity,
            in LocalTransform transform,
            in TargetPointComponent targetPoint,
            ref PistolComponent pistol)
        {
            // クールダウンの判定
            if (ElapsedTime < pistol.NextShot) { return; }

            // 弾を召喚
            var bullet = ParallelEcb.Instantiate(index, pistol.Bullet);

            // オフセットを適用
            var offsetDirection
                = math.forward(transform.Rotation)
                * pistol.Offset.x;
            var position = transform.Position + offsetDirection;
            position.y += pistol.Offset.y;

            // 角度を計算
            var angleStrengthSign = math.sign(targetPoint.TargetAngle);
            var angleStrengthAbs = math.abs(targetPoint.TargetAngle);

            var angle = math.lerp(0, math.radians(pistol.MaxAngle), angleStrengthAbs) * angleStrengthSign;

            // 位置を書き換え
            ParallelEcb.SetComponent(index, bullet, new LocalTransform
            {
                Position = position,
                Scale = 1,
                Rotation = math.mul(transform.Rotation, quaternion.Euler(0, angle, 0))
            });

            ParallelEcb.AddComponent(index, bullet, new BulletComponent
            {
                // 親になっている(プレイヤー想定)エンティティを取得
                Owner = entity,
                Target = Raycast(targetPoint.TargetPosition, targetPoint.TargetDirection),
            });

            // 次の発射時間を指定
            pistol.NextShot = ElapsedTime + ShotInterval;
        }

        private Entity Raycast(float3 position, float3 direction)
        {
            var filter = new CollisionFilter
            {
                GroupIndex = 0,
                BelongsTo = ~0u,
                CollidesWith = HIT_FILTER,
            };

            var rayInput = new RaycastInput
            {
                Start = position,
                End = direction * 10000,
                Filter = CollisionFilter.Default,
            };

            if (PhysicsWorld.CastRay(rayInput, out var hitInfo))
            {
                return hitInfo.Entity;
            }
            else
            {
                return Entity.Null;
            }
        }
    }
}