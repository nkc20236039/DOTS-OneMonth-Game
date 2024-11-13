using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct BulletSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

            state.Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(),
                EnemyGroup = SystemAPI.GetComponentLookup<EnemyHomingComponent>(),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(),
            }.Schedule(simulation, state.Dependency);

            state.Dependency.Complete();

            // �W���u���X�P�W���[��
            state.Dependency = new BulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ParallelEcb = ecb.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);


            state.Dependency.Complete();
            JobHandle.ScheduleBatchedJobs();

            // ecb�̌㏈��
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    /// <summary>
    /// �e�e�̊�{����
    /// </summary>
    [BurstCompile]
    public partial struct BulletJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ParallelEcb;
        private void Execute(
            [EntityIndexInQuery] int index,
            Entity entity,
            ref BulletComponent bullet,
            ref LocalTransform transform)
        {
            // ���Ԃ��o�߂�����
            bullet.Age += DeltaTime;
            if (bullet.Lifetime > bullet.Age)
            {
                // �������Ԃ͒��i������
                transform.Position += math.forward(transform.Rotation) * bullet.Speed;
            }
            else
            {
                // �������Ԃ��߂�����폜����
                ParallelEcb.DestroyEntity(index, entity);
            }
        }
    }

    /// <summary>
    /// �e�e�̏Փ˔���
    /// </summary>
    [BurstCompile]
    public partial struct BulletTriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly]
        public ComponentLookup<EnvironmentTag> EnvironmentGroup;
        public ComponentLookup<EnemyHomingComponent> EnemyGroup;
        public ComponentLookup<BulletComponent> BulletGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            /*�K�v�ȏՓˏ���bool��ݒ�*/
            // ���ƒe����������
            bool isEnvironmentHitAtoB
                = EnvironmentGroup.EntityExists(triggerEvent.EntityA)
                && BulletGroup.EntityExists(triggerEvent.EntityB);
            bool isEnvironmentHitBtoA
                = BulletGroup.EntityExists(triggerEvent.EntityA)
                && EnvironmentGroup.EntityExists(triggerEvent.EntityB);
            // �G�ƒe����������
            bool isEnemyHitA
                = EnemyGroup.EntityExists(triggerEvent.EntityA)
                || BulletGroup.EntityExists(triggerEvent.EntityA);
            bool isEnemyHitB
                = EnemyGroup.EntityExists(triggerEvent.EntityB)
                || BulletGroup.EntityExists(triggerEvent.EntityB);
            bool isEnemyHit = isEnemyHitA && isEnemyHitB;

            // ���ƒe������������e���폜
            if (isEnvironmentHitAtoB)
            {
                // B���e�e�Ƃ킩�邽��A���폜
                Ecb.DestroyEntity(triggerEvent.EntityA);
            }
            else if (isEnvironmentHitBtoA)
            {
                // A���e�e�Ƃ킩�邽��B���폜
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }

            if (isEnemyHit)
            {
                Ecb.DestroyEntity(triggerEvent.EntityA);
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
        }
    }
}