using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct BulletComponent : IComponentData
    {
        public float Speed;     // �e�̑��x
        public float Lifetime;  // ����
        public int AttackDamage;// �U����

        public float Age;       // ���݂�������
        public Entity Owner;   // ���̒e���������G���e�B�e�B
    }
}