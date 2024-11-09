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
        public float3 PlayerPosition;

        private void Execute([ChunkIndexInQuery] int index, ref EnemySpawnerComponent enemySpawner)
        {
            // ���Ԃ����Z
            enemySpawner.SpawnTime += DeltaTime;
            // �������Ԃ𖞂����Ă��Ȃ���ΏI��
            if (enemySpawner.SpawnInterval > enemySpawner.SpawnTime) { return; }

            // �����n�_�����߂�
            float3 spawnPosition = new
            (
                math.cos(ElapsedTime) * enemySpawner.SpawnRadius + PlayerPosition.x,
                0,
                math.sin(ElapsedTime) * enemySpawner.SpawnRadius + PlayerPosition.z
            );

            // �G���e�B�e�B��ecb���ŏ���
            var entity = ParallelEcb.Instantiate(index, enemySpawner.Enemy);
            // �ʒu�����Z�b�g
            ParallelEcb.SetComponent(index, entity, LocalTransform.FromPosition(spawnPosition));

            // �v�����Ԃ�������
            enemySpawner.SpawnTime = 0;
        }
    }
}