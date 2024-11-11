using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public partial struct EnemyRandomSpawnSystem : ISystem
    {
        Random random;
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            // ランダムスポーンするけど周回上には配置させない処理限定
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
                var randomPosition
                    = random.NextFloat3
                    (
                        float3.zero,
                        spawner.ValueRO.SpawnRadius * new float3(1, 0, 1)
                    );

                spawner.ValueRW.Position = randomSpawn.ValueRO.Origin + randomPosition;
            }
        }
    }
}