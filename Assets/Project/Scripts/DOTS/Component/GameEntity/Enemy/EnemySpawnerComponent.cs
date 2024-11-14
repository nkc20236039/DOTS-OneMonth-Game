using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public Entity Enemy;        // 召喚する敵
        public float SpawnRadius;   // 敵が召喚される半径
        public float SpawnInterval; // 召喚する間隔

        public float3 Position;     // 召喚する中心点
        public float SpawnTime;     // 次の召喚までの待ち時間
    }

    /* Tag */
    // 周回上に召喚するスポナー
    public struct EnemyOrbitSpawnTag : IComponentData { }
}