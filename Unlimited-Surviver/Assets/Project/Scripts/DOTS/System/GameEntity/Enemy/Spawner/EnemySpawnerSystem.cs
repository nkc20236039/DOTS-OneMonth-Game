using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    [UpdateInGroup(typeof(EnemySpawnerGroup), OrderLast = true)]
    public partial struct EnemySpawnerSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            var requireQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemySpawnerComponent>()
                .WithAny<EnemyOrbitSpawnTag, EnemyRandomSpawnComponent>()
                .Build();

            state.RequireForUpdate(requireQuery);
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            state.Dependency = new EnemySpawnerJob
            {
                ParallelEcb = ecb.AsParallelWriter(),
                DeltaTime = SystemAPI.Time.DeltaTime,
                ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
            }.ScheduleParallel(state.Dependency);

            // Job待機
            state.Dependency.Complete();
            // ecbの処理を実行
            ecb.Playback(state.EntityManager);
            // ecb破棄
            ecb.Dispose();
        }
    }

    public partial struct EnemySpawnerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ParallelEcb;
        public float DeltaTime;
        public float ElapsedTime;

        private void Execute([ChunkIndexInQuery] int index, ref EnemySpawnerComponent enemySpawner)
        {
            // 時間を加算
            enemySpawner.SpawnTime += DeltaTime;
            // 処理時間を満たしていなければ終了
            if (enemySpawner.SpawnInterval > enemySpawner.SpawnTime) { return; }

            // エンティティをecb内で召喚
            var entity = ParallelEcb.Instantiate(index, enemySpawner.Enemy);
            // 位置情報をセット
            ParallelEcb.SetComponent
            (
                index,
                entity,
                LocalTransform.FromPosition(enemySpawner.Position)
            );

            // 計測時間を初期化
            enemySpawner.SpawnTime = 0;
        }
    }
}