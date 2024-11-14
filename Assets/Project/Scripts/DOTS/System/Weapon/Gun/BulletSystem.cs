using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    [BurstCompile]
    public partial struct BulletSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
            state.RequireForUpdate<HealthComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // Job�ɓn�����̂̏���
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // �Փ˔����Job���쐬
            state.Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(),
                GameEntityGroup = SystemAPI.GetComponentLookup<HealthComponent>(),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(),
            }.Schedule(simulation, state.Dependency);

            // �Փ˔���Job���I�����邱�Ƃ�ҋ@
            state.Dependency.Complete();

            // �������̒e���Ǘ�����Job���쐬
            state.Dependency = new BulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ParallelEcb = ecb.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            // Job�̌㏈��
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
                transform.Position += math.forward(transform.Rotation) * bullet.Speed * DeltaTime;
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
        [ReadOnly] public ComponentLookup<EnvironmentTag> EnvironmentGroup;
        [ReadOnly] public ComponentLookup<HealthComponent> GameEntityGroup;
        [ReadOnly] public ComponentLookup<BulletComponent> BulletGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            // ���ƒe
            var environmentInfo = TriggerEventExplicit(triggerEvent, EnvironmentGroup, BulletGroup);
            // �Q�[���G���e�B�e�B�ƒe
            var gameEntityInfo = TriggerEventExplicit(triggerEvent, GameEntityGroup, BulletGroup);

            if (environmentInfo.IsHit)
            {
                // ���ƒe�����������ꍇ
                // B���e�e�Ƃ킩�邽��B���폜
                Ecb.DestroyEntity(environmentInfo.EntityB);
            }

            if (gameEntityInfo.IsHit)
            {
                // �K�v�ȃR���|�[�l���g���擾
                HealthComponent health;
                BulletComponent bullet;
                if (GameEntityGroup.TryGetComponent(gameEntityInfo.EntityA, out health) == false) { return; }
                if (BulletGroup.TryGetComponent(gameEntityInfo.EntityB, out bullet) == false) { return; }

                // ���������瑊��̗̑͂����炷
                // TODO: �����Ɩ߂�l�̊֌W����������ɂ����̂ŏC������
                health = Attack(gameEntityInfo.EntityB, health, bullet.AttackDamage);
                Ecb.SetComponent(gameEntityInfo.EntityA, health);
            }
        }

        private (bool IsHit, Entity EntityA, Entity EntityB) TriggerEventExplicit<EntityA, EntityB>(
            TriggerEvent triggerEvent,
            ComponentLookup<EntityA> entityA,
            ComponentLookup<EntityB> entityB)
            where EntityA : unmanaged, IComponentData
            where EntityB : unmanaged, IComponentData
        {
            if (entityA.HasComponent(triggerEvent.EntityA) && entityB.HasComponent(triggerEvent.EntityB))
            {
                return (true, triggerEvent.EntityA, triggerEvent.EntityB);
            }
            if (entityA.HasComponent(triggerEvent.EntityB) && entityB.HasComponent(triggerEvent.EntityA))
            {
                return (true, triggerEvent.EntityB, triggerEvent.EntityA);
            }

            return (false, Entity.Null, Entity.Null);
        }

        private HealthComponent Attack(Entity bullet, HealthComponent health, int damage)
        {
            Ecb.DestroyEntity(bullet);
            health.Health -= damage;

            return health;
        }
    }
}