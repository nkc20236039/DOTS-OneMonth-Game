using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

namespace DOTS
{
    [BurstCompile]
    [UpdateAfter(typeof(ActionUpdateGroup))]
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

    [BurstCompile]
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

            // プレイヤーからの位置を変更する
            transform.Position = gunOffset;
        }
    }
}