using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
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

            foreach ((var gun, var transform) in SystemAPI.Query<
                RefRO<GunComponent>,
                RefRW<LocalTransform>>())
            {
                // �e�����^�[�Q�b�g�Ɍ�������
                var rotation = quaternion.LookRotationSafe(gun.ValueRO.TargetDirection, math.up());
                transform.ValueRW.Rotation = rotation;

                // �e�̉�]�z�u�ʒu������
                var gunOffset
                    = gun.ValueRO.TargetDirection
                    * gun.ValueRO.Offset.x;
                // Y���W�I�t�Z�b�g��ݒ�
                gunOffset.y += gun.ValueRO.Offset.y;

                var targetPosition = gunOffset + playerTransform.Position;

                // �v���C���[����̈ʒu��ύX����
                transform.ValueRW.Position = gunOffset;
                   // = math.lerp(transform.ValueRW.Position, targetPosition, gun.ValueRO.Smooth);
            }
        }
    }
}