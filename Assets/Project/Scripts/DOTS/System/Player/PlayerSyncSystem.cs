using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct PlayerSyncSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<PlayerComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var transform, var avoid, var player) in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<AvoidComponent>,
                RefRW<PlayerComponent>>())
            {
                // ‰ñ”ğ’†‚ÍêŠ‚ğ“¯Šú‚µ‚È‚¢
                if (avoid.ValueRO.IsAvoiding) { return; }

                player.ValueRW.Position = transform.ValueRO.Position;
            }
        }
    }
}