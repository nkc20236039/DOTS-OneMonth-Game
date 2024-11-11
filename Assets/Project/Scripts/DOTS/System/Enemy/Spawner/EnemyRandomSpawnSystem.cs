using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public partial struct EnemyRandomSpawnSystem : ISystem
    {
        Random random;
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            // �����_���X�|�[�����邯�ǎ����ɂ͔z�u�����Ȃ���������
            var requireQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemySpawnerComponent, EnemyRandomSpawnComponent>()
                .WithNone<EnemyOrbitSpawnTag>()
                .Build();

            state.RequireForUpdate(requireQuery);

            random = new((uint)state.UnmanagedMetaIndex);
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var spawner, var randomSpawn) in SystemAPI.Query<
                RefRW<EnemySpawnerComponent>,
                RefRO<EnemyRandomSpawnComponent>>())
            {
                // �����_���ɕ��������߂�
                float3 randomRotation
                    = random.NextFloat3
                    (
                        new float3(-1, 0, -1),
                        new float3(1, 0, 1)
                    );
                randomRotation = math.normalize(randomRotation);

                // �����_���ɋ��������߂�
                float randomRadius = random.NextFloat(0, spawner.ValueRO.SpawnRadius);
                // ����
                float3 randomPosition = randomRotation * randomRadius;

                spawner.ValueRW.Position = randomSpawn.ValueRO.Origin + randomPosition;
            }
        }
    }
}