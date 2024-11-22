using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityPhysicsExpansion;

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
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // �Փ˔����Job���쐬
            state.Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(true),
                GameEntityGroup = SystemAPI.GetComponentLookup<HitDamageComponent>(true),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(true),
                Transform = SystemAPI.GetComponentLookup<LocalTransform>(true),
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
        [ReadOnly] public ComponentLookup<HitDamageComponent> GameEntityGroup;
        [ReadOnly] public ComponentLookup<BulletComponent> BulletGroup;
        [ReadOnly] public ComponentLookup<LocalTransform> Transform;

        public void Execute(TriggerEvent triggerEvent)
        {
            // ���ƒe
            (bool IsHit, Entity bullet, Entity environment) environmentInfo
                = CollisionResponseExplicit.TriggerEvent(triggerEvent, BulletGroup, EnvironmentGroup);
            // �Q�[���G���e�B�e�B�ƒe
            (bool IsHit, Entity bullet, Entity gameEntity) gameEntityInfo
                = CollisionResponseExplicit.TriggerEvent(triggerEvent, BulletGroup, GameEntityGroup);

            if (environmentInfo.IsHit)
            {
                // ���ƒe�����������ꍇ
                // B���e�e�Ƃ킩�邽��B���폜
                Ecb.DestroyEntity(environmentInfo.bullet);
            }

            if (gameEntityInfo.IsHit)
            {
                // �K�v�ȃR���|�[�l���g���擾
                HitDamageComponent hitDamage;
                BulletComponent bullet;
                if (GameEntityGroup.TryGetComponent(gameEntityInfo.gameEntity, out hitDamage) == false) { return; }
                if (BulletGroup.TryGetComponent(gameEntityInfo.bullet, out bullet) == false) { return; }

                // DamageComponent���������Ă���G���e�B�e�B��Bullet�̔��ˎ傾������_���[�W���������Ȃ�
                if (gameEntityInfo.gameEntity == bullet.Owner) { return; }
                // ���ˎ傪�`�[���̃R���|�[�l���g���������Ă����炻�̃`�[���Ƃ̏Փ˔������������

                // ������������Ƀ_���[�W�̏���^����
                LocalTransform transform;
                if (Transform.TryGetComponent(gameEntityInfo.gameEntity, out transform) == false) { return; }
                Ecb.SetComponentEnabled<HitDamageComponent>(gameEntityInfo.gameEntity, true);
                Ecb.SetComponent(gameEntityInfo.gameEntity, new HitDamageComponent
                {
                    IsUIShowing = false,
                    IsDistributed = false,
                    DamageValue = bullet.AttackDamage,
                    Position = transform.Position,
                });

                // �e���폜����
                Ecb.DestroyEntity(gameEntityInfo.bullet);
            }
        }
    }
}