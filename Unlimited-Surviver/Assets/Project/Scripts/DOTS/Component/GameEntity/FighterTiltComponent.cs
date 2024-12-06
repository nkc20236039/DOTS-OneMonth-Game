using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct FighterTiltComponent : IComponentData
    {
        public float3 TargetTurnDirection;
    }
}