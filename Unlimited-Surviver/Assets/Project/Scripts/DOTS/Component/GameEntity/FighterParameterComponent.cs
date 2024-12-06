using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct FighterParameterComponent : IComponentData
    {
        public float MaxTiltAngle;
        public float TiltSpeed;
    }
}