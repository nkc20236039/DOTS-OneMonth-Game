using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct AimingSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<GunComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalToWorld>(player);
            var playerWorld = SystemAPI.GetComponent<LocalToWorld>(player);

            state.Dependency = new AimingJob
            {
                PlayerPosition = playerTransform.Position,
                PlayerWorld = playerWorld,
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }
    }

    public partial struct AimingJob : IJobEntity
    {
        public float3 PlayerPosition;
        public LocalToWorld PlayerWorld;
        private void Execute(in GunComponent gun, ref LocalTransform transform)
        {
            // 銃口をターゲットに向かせる
            var rotation = quaternion.LookRotationSafe(gun.TargetDirection, math.up());
            transform.Rotation = math.mul(math.inverse(PlayerWorld.Rotation), rotation);

            // 銃の回転配置位置を決定
            var gunOffset
                = math.forward(transform.Rotation)
                * gun.Offset.x;
            // Y座標オフセットを設定
            gunOffset.y += gun.Offset.y;

            var targetPosition = gunOffset + PlayerPosition;

            // プレイヤーからの位置を変更する
            transform.Position = targetPosition;
            // = math.lerp(transform.Position, targetPosition, gun.Smooth);
        }
    }
}