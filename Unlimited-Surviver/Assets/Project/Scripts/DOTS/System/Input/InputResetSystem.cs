using Unity.Burst;
using Unity.Entities;

namespace DOTS
{
    /// <summary>
    /// ���͂Ɏg�p�����l�����̃t���[���Ɏ����z���Ȃ����߂ɏ���������
    /// </summary>
    [UpdateInGroup(typeof(InputUpdateGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct InputResetSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<PlayerInputComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach (var input in SystemAPI.Query<RefRW<PlayerInputComponent>>())
            {
                input.ValueRW.IsAvoidInput = false;
            }
        }
    }
}