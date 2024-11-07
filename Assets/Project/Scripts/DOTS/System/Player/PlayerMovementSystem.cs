using Unity.Burst;
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
            state.RequireForUpdate<PlayerComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var transform, var velocity, var physicsMass, var move, var player) in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<PhysicsVelocity>,
                RefRW<PhysicsMass>,
                RefRO<PlayerInputComponent>,
                RefRO<PlayerComponent>>())
            {
                // �󂯎�������͂𕽖ʂ֕ϊ�
                float3 moveDirection = new
                (
                    move.ValueRO.MoveDirection.x,
                    0,
                    move.ValueRO.MoveDirection.y
                );
                moveDirection = math.normalizesafe(moveDirection);

                // ���x��K�p
                velocity.ValueRW.Linear
                    = moveDirection
                    * player.ValueRO.Speed
                    * SystemAPI.Time.DeltaTime;

                // �ړ������Ă��Ȃ���΂���ȍ~�̏��������s���Ȃ�
                // (���͂��Ď����Ă��ăG���e�B�e�B�S�ď������s���Ȃ����Ƃ��m�肵�Ă��邽�߃��[�v�𔲂��Ă悢)
                if (math.distancesq(float3.zero, moveDirection) == 0) { return; }

                /*��]�̌v�Z*/
                quaternion currentRotation = transform.ValueRO.Rotation;
                quaternion lookRotation = quaternion.LookRotationSafe(moveDirection, math.up());

                // �X���[�Y�ɉ�]������
                transform.ValueRW.Rotation
                    = math.slerp
                    (
                        currentRotation,
                        lookRotation,
                        player.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime
                    );

                // �����̉�]���Œ�
                physicsMass.ValueRW.InverseInertia = float3.zero;
            }
        }
    }
}