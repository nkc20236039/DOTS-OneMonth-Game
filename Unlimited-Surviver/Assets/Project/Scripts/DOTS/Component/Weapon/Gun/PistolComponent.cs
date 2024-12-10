using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PistolComponent : IComponentData
    {
        public float ShotInterval;
        public Entity Bullet;
        public float2 Offset;   // 弾を召喚するオフセット
        public float MaxAngle;

        public float NextShot;
    }
}