using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct GunComponent : IComponentData
    {
        public float ShotInterval;
        public Entity Bullet;
        public float2 Offset;
        public float Smooth;

        public float3 TargetDirection;
    }
}