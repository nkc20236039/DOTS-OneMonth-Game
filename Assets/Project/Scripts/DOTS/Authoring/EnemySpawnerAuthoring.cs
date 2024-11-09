using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
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
                    SpawnRadius = authoring.SpawnRadius,
                    SpawnInterval = authoring.SpawnInterval,
                    Enemy = enemy
                });
            }
        }
    }
}