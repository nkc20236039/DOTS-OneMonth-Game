using Unity.Entities;

namespace DOTS
{
    public struct PlayerComponent : IComponentData
    {
        public float Speed;
        public float RotationSpeed;
    }
}