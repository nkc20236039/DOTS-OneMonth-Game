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
            // �v���C���[���擾
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
            // �^�[�Q�b�g�̕���������
            var rotation = quaternion.LookRotationSafe(weapon.TargetDirection, math.up());
            transform.Rotation = math.mul(math.inverse(PlayerWorld.Rotation), rotation);

            // �v���C���[����̃I�t�Z�b�g���쐬
            var offset
                = math.forward(transform.Rotation)
                * weapon.Offset.x;
            // Y���W�I�t�Z�b�g��ݒ�
            offset.y += weapon.Offset.y;
            transform.Position = offset;

            // ���[���h���W�p�̃I�t�Z�b�g���쐬
            var worldOffset
                = math.forward(rotation)
                * weapon.Offset.x;
            worldOffset.y += weapon.Offset.y;

            // ���[���h����ۑ�
            weapon.WorldPosition = PlayerPosition + worldOffset;
            weapon.WorldRotation = rotation;
        }
    }
}