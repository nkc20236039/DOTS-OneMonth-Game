using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct BulletComponent : IComponentData
    {
        public float Speed;     // �e�̑��x
        public float Lifetime;  // ����

        public float Age;       // ���݂�������
    }
}