using System.Globalization;
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
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
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
                = EnvironmentGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnvironmentHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnvironmentGroup.HasComponent(triggerEvent.EntityB);
            // �G�ƒe����������
            bool isEnemyHitAtoB
                = EnemyGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnemyHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnemyGroup.HasComponent(triggerEvent.EntityB);

            // ���ƒe������������e���폜
            if (isEnvironmentHitAtoB)
            {
                // B���e�e�Ƃ킩�邽��B���폜
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
            else if (isEnvironmentHitBtoA)
            {
                // A���e�e�Ƃ킩�邽��A���폜
                Ecb.DestroyEntity(triggerEvent.EntityA);
            }

            if (isEnemyHitAtoB || isEnemyHitBtoA)
            {
                Ecb.DestroyEntity(triggerEvent.EntityA);
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
        }
    }
}