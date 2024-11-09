using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    /// <summary>
    /// プレイヤーの回避処理を追加
    /// </summary>
    [UpdateInGroup(typeof(InputUpdateGroup))]
    [BurstCompile]
    public partial struct PlayerAvoidSystem : ISystem
    {
        void ISystem.OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AvoidComponent>();
            state.RequireForUpdate<PlayerInputComponent>();
        }

        void ISystem.OnUpdate(ref SystemState state)
        {
            foreach ((var avoid, var input) in SystemAPI.Query<
                RefRW<AvoidComponent>,
                RefRO<PlayerInputComponent>>())
            {
                if (avoid.ValueRO.IsAvoiding) { return; }

                // 回避状態にする
                avoid.ValueRW.IsAvoiding = input.ValueRO.IsAvoidInput;

                // 方向を指定
                float3 direction = new
                (
                    input.ValueRO.MoveDirection.x,
                    0,
                    input.ValueRO.MoveDirection.y
                );
                avoid.ValueRW.AvoidDirection = direction;
            }
        }
    }
}