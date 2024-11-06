using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    [UpdateInGroup(typeof(InputUpdateGroup))]
    public partial struct PlayerSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<MovementComponent>();
            state.RequireForUpdate<PlayerComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var velocity, var physicsMass, var move, var player) in SystemAPI.Query<
                RefRW<PhysicsVelocity>,
                RefRW<PhysicsMass>,
                RefRO<MovementComponent>,
                RefRO<PlayerComponent>>())
            {
                float3 direction = new
                (
                    move.ValueRO.MoveDirection.x,
                    0,
                    move.ValueRO.MoveDirection.y
                );

                velocity.ValueRW.Linear = direction * player.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                physicsMass.ValueRW.InverseInertia = float3.zero;
            }
        }
    }
}