using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct EnemyHomingComponent : IComponentData
    {
        public float Speed;             // ���x
        public float HomingAccuracy;    // �z�[�~���O���x

        public quaternion TargetRotation;
    }
}