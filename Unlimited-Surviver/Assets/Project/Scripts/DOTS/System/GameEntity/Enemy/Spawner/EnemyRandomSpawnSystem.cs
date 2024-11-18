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
                // ランダムに方向を決める
                float3 randomRotation
                    = random.NextFloat3
                    (
                        new float3(-1, 0, -1),
                        new float3(1, 0, 1)
                    );
                randomRotation = math.normalize(randomRotation);

                // ランダムに距離を決める
                float randomRadius = random.NextFloat(0, spawner.ValueRO.SpawnRadius);
                // 合成
                float3 randomPosition = randomRotation * randomRadius;

                spawner.ValueRW.Position = randomSpawn.ValueRO.Origin + randomPosition;
            }
        }
    }
}