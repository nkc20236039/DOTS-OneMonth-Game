using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        private enum SpawnerType
        {
            Orbit,
            Random,
        }
        [SerializeField]
        private SpawnerType spawnerType;

        [Space]
        [SerializeField]
        private float SpawnRadius;
        [SerializeField]
        private float SpawnInterval;
        [SerializeField]
        private GameObject enemyPrefab;

        private class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var spawner = GetEntity(TransformUsageFlags.None);
                var enemy = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic);

                AddComponent(spawner, new EnemySpawnerComponent
                {
                    Enemy = enemy,
                    SpawnRadius = authoring.SpawnRadius,
                    SpawnInterval = authoring.SpawnInterval,
                    Position = authoring.transform.position
                });

                switch (authoring.spawnerType)
                {
                    case SpawnerType.Orbit:
                        AddComponent(spawner, typeof(EnemyOrbitSpawnTag));
                        break;
                    case SpawnerType.Random:
                        AddComponent(spawner, new EnemyRandomSpawnComponent
                        {
                            Origin = authoring.transform.position,
                        });
                        break;
                }
            }
        }
    }
}