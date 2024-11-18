using Unity.Entities;
using Unity.Physics;

namespace UnityPhysicsExpansion
{
    public struct PhysicsTriggerEvent
    {
        /// <summary>
        /// �Փ˂�����̃G���e�B�e�B��
        /// </summary>
        /// <typeparam name="Self"></typeparam>
        /// <typeparam name="Opponent"></typeparam>
        /// <param name="triggerEvent"></param>
        /// <param name="selfGroup"></param>
        /// <param name="opponentGroup"></param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) TriggerEventExplicit<Self, Opponent>(
            TriggerEvent triggerEvent,
            ComponentLookup<Self> selfGroup,
            ComponentLookup<Opponent> opponentGroup)
                where Self : unmanaged, IComponentData
                where Opponent : unmanaged, IComponentData
        {
            if (selfGroup.HasComponent(triggerEvent.EntityA) && opponentGroup.HasComponent(triggerEvent.EntityB))
            {
                // EntityA�����g�AEntityB������Ƃ��ďՓ�
                return (true, triggerEvent.EntityA, triggerEvent.EntityB);
            }
            if (selfGroup.HasComponent(triggerEvent.EntityB) && opponentGroup.HasComponent(triggerEvent.EntityA))
            {
                // EntityA������AEntityB�����g�Ƃ��ďՓ�
                return (true, triggerEvent.EntityB, triggerEvent.EntityA);
            }

            // �Փ˖���
            return (false, Entity.Null, Entity.Null);
        }
    }
}