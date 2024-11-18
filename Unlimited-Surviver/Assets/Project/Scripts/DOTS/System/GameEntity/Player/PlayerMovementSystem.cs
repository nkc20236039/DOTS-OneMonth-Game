using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    /// <summary>
    /// �v���C���[�̈ړ��R���g���[��
    /// </summary>
    [UpdateInGroup(typeof(InputUpdateGroup))]
    [BurstCompile]
    public partial struct PlayerMovementSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<PlayerInputComponent>();
            state.RequireForUpdate<PlayerSingleton>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            state.Dependency = new PlayerMovementJob
            {
                Player = SystemAPI.GetSingleton<PlayerSingleton>()
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }
    }

    /// <summary>
    /// �v���C���[�ړ�Job
    /// </summary>
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        [ReadOnly] public PlayerSingleton Player;

        private void Execute(
            ref LocalTransform transform,
            ref PhysicsVelocity velocity,
            ref PhysicsMass mass,
            in PlayerInputComponent playerInput)
        {
            // �󂯎�������͂𕽖ʂ֕ϊ�
            float3 moveDirection = new
            (
                playerInput.MoveDirection.x,
                0,
                playerInput.MoveDirection.y
            );
            moveDirection = math.normalizesafe(moveDirection);

            // ���x��K�p
            velocity.Linear
                = moveDirection
                * Player.Speed;

            // Y���W��0�ɌŒ�
            transform.Position.y = 0;

            // �ړ������Ă��Ȃ���΂���ȍ~�̏��������s���Ȃ�
            // (���͂��Ď����Ă��ăG���e�B�e�B�S�ď������s���Ȃ����Ƃ��m�肵�Ă��邽�߃��[�v�𔲂��Ă悢)
            if (math.distancesq(float3.zero, moveDirection) == 0) { return; }

            /*��]�̌v�Z*/
            quaternion currentRotation = transform.Rotation;
            quaternion lookRotation = quaternion.LookRotationSafe(moveDirection, math.up());

            // �X���[�Y�ɉ�]������
            transform.Rotation
                = math.slerp
                (
                    currentRotation,
                    lookRotation,
                    Player.RotationSpeed
                );

            // �����̉�]���Œ�
            mass.InverseInertia = float3.zero;
        }
    }
}