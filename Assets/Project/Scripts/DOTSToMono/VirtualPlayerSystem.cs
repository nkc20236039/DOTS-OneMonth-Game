using DOTS;
using Unity.Entities;
using Unity.Transforms;

namespace DOTStoMono
{
    [UpdateAfter(typeof(ActionUpdateGroup))]
    public partial struct VirtualPlayerSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<VirtualPlayerManagedComponent>();
            state.RequireForUpdate<PlayerSingleton>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // MonoBehaviour側のプレイヤーを取得
            var virtualPlayer = SystemAPI.ManagedAPI
                .GetSingleton<VirtualPlayerManagedComponent>();

            // DOTS側のプレイヤーを取得
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            // 位置を同期
            virtualPlayer.VirtualPlayerTransform.position = playerTransform.Position;
        }
    }
}