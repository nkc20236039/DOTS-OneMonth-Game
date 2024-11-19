using Unity.Burst;
using Unity.Entities;

namespace DOTS
{
    [BurstCompile]
    public partial struct EnemyHealthSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<EnemyTag>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((var health, var damage, var entity) in SystemAPI
                .Query<RefRW<HealthComponent>, RefRW<HitDamageComponent>>()
                .WithAll<EnemyTag>()
                .WithEntityAccess())
            {
                // �_���[�W���󂯂Ă���Α̗͂�����������
                bool isDamageEnable = state.EntityManager.IsComponentEnabled<HitDamageComponent>(entity);
                if (isDamageEnable && !damage.ValueRO.IsDistributed)
                {
                    health.ValueRW.Health -= damage.ValueRO.DamageValue;
                    damage.ValueRW.IsDistributed = true;
                }

                // �̗͂�0�ł͂Ȃ������炱��Entity�̓X�L�b�v
                if (0 < health.ValueRO.Health) { continue; }

                // �̗͂������Ȃ���Entity���폜
                ecb.DestroyEntity(entity);
            }
        }
    }
}