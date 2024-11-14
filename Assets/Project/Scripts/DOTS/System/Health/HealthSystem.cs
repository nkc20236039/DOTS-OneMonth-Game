using Unity.Burst;
using Unity.Entities;

namespace DOTS
{
    [BurstCompile]
    public partial struct HealthSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((var health, var entity) in SystemAPI.Query<RefRO<HealthComponent>>().WithEntityAccess())
            {
                // �̗͂�0�ł͂Ȃ������炱��Entity�̓X�L�b�v
                if (0 < health.ValueRO.Health) { continue; }

                // �̗͂������Ȃ���Entity���폜
                ecb.DestroyEntity(entity);
            }
        }
    }
}