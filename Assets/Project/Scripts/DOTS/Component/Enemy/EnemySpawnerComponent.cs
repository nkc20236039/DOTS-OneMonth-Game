using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public Entity Enemy;        // ��������G
        public float SpawnRadius;   // �G����������锼�a
        public float SpawnInterval; // ��������Ԋu

        public float3 Position;     // �������钆�S�_
        public float SpawnTime;     // ���̏����܂ł̑҂�����
    }

    /* Tag */
    // �����ɏ�������X�|�i�[
    public struct EnemyOrbitSpawnTag : IComponentData { }
}