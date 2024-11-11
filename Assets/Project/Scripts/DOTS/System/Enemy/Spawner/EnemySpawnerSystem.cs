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

            // Job�ҋ@
            state.Dependency.Complete();
            // ecb�̏��������s
            ecb.Playback(state.EntityManager);
            // ecb�j��
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
            // ���Ԃ����Z
            enemySpawner.SpawnTime += DeltaTime;
            // �������Ԃ𖞂����Ă��Ȃ���ΏI��
            if (enemySpawner.SpawnInterval > enemySpawner.SpawnTime) { return; }

            // �G���e�B�e�B��ecb���ŏ���
            var entity = ParallelEcb.Instantiate(index, enemySpawner.Enemy);
            // �ʒu�����Z�b�g
            ParallelEcb.SetComponent
            (
                index,
                entity,
                LocalTransform.FromPosition(enemySpawner.Position)
            );

            // �v�����Ԃ�������
            enemySpawner.SpawnTime = 0;
        }
    }
}