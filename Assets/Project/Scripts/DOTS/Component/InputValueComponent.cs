using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct MovementComponent : IComponentData
    {
        public float2 MoveDirection;
    }

    public struct AvoidComponent : IComponentData
    {
        public bool IsAvoidInput;
        public bool IsAvoiding;
    }
}