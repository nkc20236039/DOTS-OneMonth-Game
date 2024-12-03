using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PlayerSingleton : IComponentData
    {
        public float RotationSpeed;
    }
}