using Unity.Entities;
using Unity.Physics;

namespace UnityPhysicsExpansion
{
    public struct PhysicsTriggerEvent
    {
        /// <summary>
        /// 衝突した二つのエンティティの
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
                // EntityAが自身、EntityBが相手として衝突
                return (true, triggerEvent.EntityA, triggerEvent.EntityB);
            }
            if (selfGroup.HasComponent(triggerEvent.EntityB) && opponentGroup.HasComponent(triggerEvent.EntityA))
            {
                // EntityAが相手、EntityBが自身として衝突
                return (true, triggerEvent.EntityB, triggerEvent.EntityA);
            }

            // 衝突無し
            return (false, Entity.Null, Entity.Null);
        }
    }
}