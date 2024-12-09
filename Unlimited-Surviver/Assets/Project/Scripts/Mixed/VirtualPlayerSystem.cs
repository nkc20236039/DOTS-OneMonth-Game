using DOTS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTStoMono
{
    [UpdateAfter(typeof(ActionUpdateGroup))]
    public partial class VirtualPlayerSystem : SystemBase
    {
        private VirtualPlayerManagedSingleton virtualPlayer;
        protected override void OnCreate()
        {
            RequireForUpdate<VirtualPlayerManagedSingleton>();
            RequireForUpdate<PlayerSingleton>();
        }

        protected override void OnUpdate()
        {
            if (virtualPlayer == null)
            {
                // MonoBehaviour側のプレイヤーを取得
                virtualPlayer = SystemAPI.ManagedAPI
                    .GetSingleton<VirtualPlayerManagedSingleton>();
            }

            // DOTS側のプレイヤーを取得
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            // 位置を同期
            virtualPlayer.VirtualPlayerTransform.position = playerTransform.Position;
            // 回転を同期
            virtualPlayer.VirtualPlayerTransform.rotation = playerTransform.Rotation;
        }
    }
}