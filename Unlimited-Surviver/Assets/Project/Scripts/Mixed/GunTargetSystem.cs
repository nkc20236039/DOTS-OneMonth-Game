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
        private TargetPointManagedSingleton targetPoint;

        protected override void OnCreate()
        {
            RequireForUpdate<TargetPointManagedSingleton>();
            RequireForUpdate<WeaponComponent>();
            RequireForUpdate<PlayerSingleton>();
        }

        protected override void OnUpdate()
        {
            if (targetPoint == null)
            {
                // �^�[�Q�b�g���W���Ǘ�����R���|�[�l���g���擾
                targetPoint = SystemAPI.ManagedAPI
                    .GetSingleton<TargetPointManagedSingleton>();
            }

            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            foreach (var gun in SystemAPI.Query<RefRW<WeaponComponent>>())
            {
                float3 direction = targetPoint.Position - playerTransform.Position;
                
                // ���ʂ��e�̃^�[�Q�b�g�ɐݒ�
                gun.ValueRW.TargetDirection = math.normalize(direction);
            }
        }
    }
}