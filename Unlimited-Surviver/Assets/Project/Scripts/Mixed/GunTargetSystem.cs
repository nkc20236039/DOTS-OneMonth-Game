using DOTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTStoMono
{
    [BurstCompile]
    [UpdateInGroup(typeof(ActionUpdateGroup))]
    public partial class GunTargetSystem : SystemBase
    {
        private TargetPointComponent targetPoint;

        protected override void OnCreate()
        {
            Enabled = false;
            RequireForUpdate<TargetPointComponent>();
            RequireForUpdate<WeaponComponent>();
            RequireForUpdate<PlayerSingleton>();
        }

        protected override void OnUpdate()
        {
            /*if (targetPoint == null)
            {
                // ターゲット座標を管理するコンポーネントを取得
                targetPoint = SystemAPI.ManagedAPI
                    .GetSingleton<TargetPointComponent>();
            }*/

            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            foreach (var gun in SystemAPI.Query<RefRW<WeaponComponent>>())
            {
                float3 direction = targetPoint.Position - playerTransform.Position;

                // 結果を銃のターゲットに設定
                gun.ValueRW.TargetDirection = math.normalize(direction);
            }
        }
    }
}