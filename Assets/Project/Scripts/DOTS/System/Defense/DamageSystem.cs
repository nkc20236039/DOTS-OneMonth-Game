using Unity.Burst;
using Unity.Entities;

namespace DOTS
{
    [BurstCompile]
    public partial struct DamageSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<DefenseComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            
        }
    }
}