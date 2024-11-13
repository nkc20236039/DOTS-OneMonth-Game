using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct BulletComponent : IComponentData
    {
        public float Speed;     // ’e‚Ì‘¬“x
        public float Lifetime;  // õ–½

        public float Age;       // ‘¶İ‚µ‚½ŠÔ
    }
}