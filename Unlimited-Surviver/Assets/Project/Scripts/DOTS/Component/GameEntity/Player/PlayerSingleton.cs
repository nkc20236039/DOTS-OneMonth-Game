using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PlayerSingleton : IComponentData
    {
        public float PropulsionPower;
        public float RotationSpeed;
        public float PitchRotation;
    }
}