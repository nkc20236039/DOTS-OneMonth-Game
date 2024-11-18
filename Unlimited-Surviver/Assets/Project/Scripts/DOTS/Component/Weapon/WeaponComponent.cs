using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct WeaponComponent : IComponentData
    {
        public float2 Offset;

        public float3 TargetDirection;
        public float3 WorldPosition;
        public quaternion WorldRotation;
    }
}