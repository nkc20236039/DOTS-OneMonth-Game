using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct EnemyHomingComponent : IComponentData
    {
        public float Speed;             // 速度
        public float HomingAccuracy;    // ホーミング精度

        public quaternion TargetRotation;
    }
}