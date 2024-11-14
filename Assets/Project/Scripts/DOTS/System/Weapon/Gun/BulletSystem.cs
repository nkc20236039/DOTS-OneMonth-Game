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
                HealthGroup = SystemAPI.GetComponentLookup<HealthComponent>(),
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
        [ReadOnly]
        public ComponentLookup<EnvironmentTag> EnvironmentGroup;
        public ComponentLookup<HealthComponent> HealthGroup;
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
            bool isHealthHitAtoB
                = HealthGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isHealthHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && HealthGroup.HasComponent(triggerEvent.EntityB);

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

            if (isHealthHitAtoB)
            {
                // �K�v�ȃR���|�[�l���g���擾
                HealthComponent _health;
                BulletComponent _bullet;
                if (HealthGroup.TryGetComponent(triggerEvent.EntityA, out _health) == false) { return; }
                if (BulletGroup.TryGetComponent(triggerEvent.EntityB, out _bullet) == false) { return; }


                // ���������瑊��̗̑͂����炷
                _health = Attack(triggerEvent.EntityB, _health, _bullet.AttackDamage);
                Ecb.SetComponent(triggerEvent.EntityA, _health);
            }
            else if (isHealthHitBtoA)
            {
                // �K�v�ȃR���|�[�l���g���擾
                HealthComponent health;
                BulletComponent bullet;
                if (BulletGroup.TryGetComponent(triggerEvent.EntityA, out bullet) == false) { return; }
                if (HealthGroup.TryGetComponent(triggerEvent.EntityB, out health) == false) { return; }

                // ���������瑊��̗̑͂����炷
                health = Attack(triggerEvent.EntityA, health, bullet.AttackDamage);
                Ecb.SetComponent(triggerEvent.EntityB, health);
            }
        }

        private (bool isHit, Entity entityA, Entity entityB) TriggerEventExplicit<EntityA, EntityB>(
            TriggerEvent triggerEvent,
            ComponentLookup<EntityA> entityA,
            ComponentLookup<EntityB> entityB)
            where EntityA : unmanaged, IComponentData
            where EntityB : unmanaged, IComponentData
        {
            if (EnvironmentGroup.HasComponent(triggerEvent.EntityA) && BulletGroup.HasComponent(triggerEvent.EntityB))
            {

            }

            return default;
        }

        private HealthComponent Attack(Entity bullet, HealthComponent health, int damage)
        {
            Ecb.DestroyEntity(bullet);
            health.Health -= damage;

            return health;
        }
    }
}