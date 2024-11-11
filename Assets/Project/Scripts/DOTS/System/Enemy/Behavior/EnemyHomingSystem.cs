using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct EnemyHomingSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            // 召喚する条件が整っているエンティティのみに処理
            var requireQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemyHomingComponent, LocalTransform, PhysicsVelocity>()
                .Build();

            state.RequireForUpdate(requireQuery);
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            new EnemyHomingJob
            {
                PlayerPositon = playerTransform.Position,
            }.ScheduleParallel();
        }
    }

    public partial struct EnemyHomingJob : IJobEntity
    {
        public float3 PlayerPositon;

        private void Execute(
            in EnemyHomingComponent enemy,
            ref LocalTransform transform,
            ref PhysicsVelocity velocity)
        {
            // 現在の方向を取得
            // プレイヤーの方向を取得
            float3 targetDirection = math.normalize(PlayerPositon - transform.Position);
            targetDirection.y = 0f;
            quaternion targetRotate = quaternion.LookRotationSafe(targetDirection, math.up());

            // 曲がる角度を確定する(ホーミング)
            quaternion direction = math.slerp(transform.Rotation, targetRotate, enemy.HomingAccuracy);

            transform.Rotation = direction;
            velocity.Linear = transform.Forward() * enemy.Speed;
            transform.Position.y = 0;
        }
    }
}