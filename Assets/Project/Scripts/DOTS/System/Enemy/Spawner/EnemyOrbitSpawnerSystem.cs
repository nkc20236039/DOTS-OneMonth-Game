using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    public partial struct EnemyOrbitSpawnerSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            var requireQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemySpawnerComponent, EnemyOrbitSpawnTag>()
                .WithNone<EnemyRandomSpawnComponent>()
                .Build();

            state.RequireForUpdate(requireQuery);
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
            {
                // è¢ä´Ç∑ÇÈínì_Çì¸ÇÍÇÈ
                spawner.ValueRW.Position = new
                (
                    math.cos((float)SystemAPI.Time.ElapsedTime)
                        * spawner.ValueRO.SpawnRadius
                        + playerTransform.Position.x,
                    0,
                    math.sin((float)SystemAPI.Time.ElapsedTime)
                        * spawner.ValueRO.SpawnRadius
                        + playerTransform.Position.z
                );
            }
        }
    }
}