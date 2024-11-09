using Unity.Entities;

namespace DOTS
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public float SpawnRadius;   // �G����������锼�a
        public Entity Enemy;
        public float SpawnInterval; // ��������Ԋu

        public float SpawnTime;     // ���̏����܂ł̑҂�����
    }
}