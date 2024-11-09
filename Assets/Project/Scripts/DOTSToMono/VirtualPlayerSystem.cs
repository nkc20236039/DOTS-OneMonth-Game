using DOTS;
using Unity.Entities;

namespace DOTStoMono
{
    [UpdateAfter(typeof(ActionUpdateGroup))]
    public partial struct VirtualPlayerSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<VirtualPlayerManagedComponent>();
            state.RequireForUpdate<PlayerComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // MonoBehaviour側のプレイヤーを取得
            var virtualPlayer = SystemAPI.ManagedAPI
                .GetSingleton<VirtualPlayerManagedComponent>();

            // DOTS側のプレイヤーを取得
            var player = SystemAPI.GetSingleton<PlayerComponent>();

            // 位置を同期
            virtualPlayer.VirtualPlayerTransform.position = player.Position;
        }
    }
}