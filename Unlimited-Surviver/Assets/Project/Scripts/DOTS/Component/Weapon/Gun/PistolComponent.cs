using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PistolComponent : IComponentData
    {
        public float ShotInterval;
        public Entity Bullet;
        public float2 Offset;   // �e����������I�t�Z�b�g

        public float Cooldown;
    }
}