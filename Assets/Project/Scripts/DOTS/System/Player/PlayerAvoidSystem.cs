using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace DOTS
{
    /// <summary>
    /// ÉvÉåÉCÉÑÅ[ÇÃâÒîèàóùÇí«â¡
    /// </summary>
    [UpdateInGroup(typeof(InputUpdateGroup))]
    [BurstCompile]
    public partial struct PlayerAvoidSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<AvoidComponent>();
            state.RequireForUpdate<PlayerInputComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var avoid, var input) in SystemAPI.Query<
                RefRW<AvoidComponent>,
                RefRO<PlayerInputComponent>>())
            {
                if (avoid.ValueRO.IsAvoiding) { return; }

                avoid.ValueRW.IsAvoiding = input.ValueRO.IsAvoidInput;

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