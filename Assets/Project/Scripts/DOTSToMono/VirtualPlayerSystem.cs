using DOTS;
using Unity.Burst;
using Unity.Entities;
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
                // MonoBehaviour���̃v���C���[���擾
                virtualPlayer = SystemAPI.ManagedAPI
                    .GetSingleton<VirtualPlayerManagedSingleton>();
            }

            // DOTS���̃v���C���[���擾
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            // �ʒu�𓯊�
            virtualPlayer.VirtualPlayerTransform.position = playerTransform.Position;
        }
    }
}