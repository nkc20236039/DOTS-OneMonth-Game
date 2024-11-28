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
    public partial struct WeaponAimingSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<WeaponComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            if (SystemAPI.Time.DeltaTime == 0) { return; }

            // プレイヤーを取得
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

        private void Execute(ref WeaponComponent weapon, ref LocalTransform transform)
        {
            // ターゲットの方向を向く
            var rotation = quaternion.LookRotationSafe(weapon.TargetDirection, math.up());
            transform.Rotation = math.mul(math.inverse(PlayerWorld.Rotation), rotation);

            // プレイヤーからのオフセットを作成
            var offset
                = math.forward(transform.Rotation)
                * weapon.Offset.x;
            // Y座標オフセットを設定
            offset.y += weapon.Offset.y;
            transform.Position = offset;

            // ワールド座標用のオフセットを作成
            var worldOffset
                = math.forward(rotation)
                * weapon.Offset.x;
            worldOffset.y += weapon.Offset.y;

            // ワールド情報を保存
            weapon.WorldPosition = PlayerPosition + worldOffset;
            weapon.WorldRotation = rotation;
        }
    }
}