using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
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

    public partial struct AimingJob : IJobEntity
    {
        public float3 PlayerPosition;
        public LocalToWorld PlayerWorld;
        private void Execute(in GunComponent gun, ref LocalTransform transform)
        {
            // �e�����^�[�Q�b�g�Ɍ�������
            var rotation = quaternion.LookRotationSafe(gun.TargetDirection, math.up());
            transform.Rotation = math.mul(math.inverse(PlayerWorld.Rotation), rotation);

            // �e�̉�]�z�u�ʒu������
            var gunOffset
                = math.forward(transform.Rotation)
                * gun.Offset.x;
            // Y���W�I�t�Z�b�g��ݒ�
            gunOffset.y += gun.Offset.y;

            var targetPosition = gunOffset + PlayerPosition;

            // �v���C���[����̈ʒu��ύX����
            transform.Position = targetPosition;
            // = math.lerp(transform.Position, targetPosition, gun.Smooth);
        }
    }
}