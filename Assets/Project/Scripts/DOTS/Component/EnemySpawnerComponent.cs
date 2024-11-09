using Unity.Entities;

namespace DOTS
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public float SpawnRadius;   // “G‚ª¢Š«‚³‚ê‚é”¼Œa
        public Entity Enemy;
        public float SpawnInterval; // ¢Š«‚·‚éŠÔŠu

        public float SpawnTime;     // Ÿ‚Ì¢Š«‚Ü‚Å‚Ì‘Ò‚¿ŠÔ
    }
}