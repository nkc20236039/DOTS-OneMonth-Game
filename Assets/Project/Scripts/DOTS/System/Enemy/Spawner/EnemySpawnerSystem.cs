using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct EnemySpawnerSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<EnemySpawnerComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            state.Dependency = new EnemySpawnerJob
            {
                ParallelEcb = ecb.AsParallelWriter(),
                DeltaTime = SystemAPI.Time.DeltaTime,
                ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
                PlayerPosition = playerTransform.Position
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
        public float3 PlayerPosition;

        private void Execute([ChunkIndexInQuery] int index, ref EnemySpawnerComponent enemySpawner)
        {
            // 時間を加算
            enemySpawner.SpawnTime += DeltaTime;
            // 処理時間を満たしていなければ終了
            if (enemySpawner.SpawnInterval > enemySpawner.SpawnTime) { return; }

            // 召喚地点を決める
            float3 spawnPosition = new
            (
                math.cos(ElapsedTime) * enemySpawner.SpawnRadius + PlayerPosition.x,
                0,
                math.sin(ElapsedTime) * enemySpawner.SpawnRadius + PlayerPosition.z
            );

            // エンティティをecb内で召喚
            var entity = ParallelEcb.Instantiate(index, enemySpawner.Enemy);
            // 位置情報をセット
            ParallelEcb.SetComponent(index, entity, LocalTransform.FromPosition(spawnPosition));

            // 計測時間を初期化
            enemySpawner.SpawnTime = 0;
        }
    }
}