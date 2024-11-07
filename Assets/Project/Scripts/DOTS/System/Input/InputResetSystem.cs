using Unity.Burst;
using Unity.Entities;

namespace DOTS
{
    /// <summary>
    /// 入力に使用した値を次のフレームに持ち越さないために初期化する
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