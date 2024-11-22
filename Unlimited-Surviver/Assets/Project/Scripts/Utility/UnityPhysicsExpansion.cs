using Unity.Entities;
using Unity.Physics;

namespace UnityPhysicsExpansion
{
    public struct CollisionResponseExplicit
    {
        /// <summary>
        /// 引き起こされたエンティティの順番を明確にします
        /// </summary>
        /// <typeparam name="Self">自分側のコンポーネント</typeparam>
        /// <typeparam name="Opponent">相手側のコンポーネント</typeparam>
        /// <param name="triggerEvent">トリガーイベント</param>
        /// <param name="selfGroup">自分側のコンポーネントグループ</param>
        /// <param name="opponentGroup">相手側のコンポーネントグループ</param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) TriggerEvent<Self, Opponent>(
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

        /// <summary>
        /// 引き起こされたエンティティの順番を明確にします
        /// </summary>
        /// <typeparam name="Opponent">相手側のコンポーネント</typeparam>
        /// <param name="triggerEvent">トリガーイベント</param>
        /// <param name="selfEntity">自分側のエンティティ</param>
        /// <param name="opponentGroup">相手側のコンポーネントグループ</param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) TriggerEvent<Opponent>(
            TriggerEvent triggerEvent,
            Entity selfEntity,
            ComponentLookup<Opponent> opponentGroup)
                where Opponent : unmanaged, IComponentData
        {
            if (selfEntity == triggerEvent.EntityA && opponentGroup.HasComponent(triggerEvent.EntityB))
            {
                // EntityAが自身、EntityBが相手として衝突
                return (true, triggerEvent.EntityA, triggerEvent.EntityB);
            }
            if (selfEntity == triggerEvent.EntityB && opponentGroup.HasComponent(triggerEvent.EntityA))
            {
                // EntityAが相手、EntityBが自身として衝突
                return (true, triggerEvent.EntityB, triggerEvent.EntityA);
            }

            // 衝突無し
            return (false, Entity.Null, Entity.Null);
        }

        /// <summary>
        /// 引き起こされたエンティティの順番を明確にします
        /// </summary>
        /// <typeparam name="Self">自分側のコンポーネント</typeparam>
        /// <param name="triggerEvent">トリガーイベント</param>
        /// <param name="selfGroup">自分側のコンポーネントグループ</param>
        /// <param name="opponentEntity">相手側のエンティティ</param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) TriggerEvent<Self>(
            TriggerEvent triggerEvent,
            ComponentLookup<Self> selfGroup,
            Entity opponentEntity)
                where Self : unmanaged, IComponentData
        {
            if (selfGroup.HasComponent(triggerEvent.EntityA) && opponentEntity == triggerEvent.EntityB)
            {
                // EntityAが自身、EntityBが相手として衝突
                return (true, triggerEvent.EntityA, triggerEvent.EntityB);
            }
            if (selfGroup.HasComponent(triggerEvent.EntityB) && opponentEntity == triggerEvent.EntityA)
            {
                // EntityAが相手、EntityBが自身として衝突
                return (true, triggerEvent.EntityB, triggerEvent.EntityA);
            }

            // 衝突無し
            return (false, Entity.Null, Entity.Null);
        }

        /// <summary>
        /// 衝突した二つのエンティティの順番を明確にします
        /// </summary>
        /// <typeparam name="Self">自分側のコンポーネント</typeparam>
        /// <typeparam name="Opponent">相手側のコンポーネント</typeparam>
        /// <param name="collisionEvent">コリジョンイベント</param>
        /// <param name="selfGroup">自分側のコンポーネントグループ</param>
        /// <param name="opponentGroup">相手側のコンポーネントグループ</param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) CollisionEvent<Self, Opponent>(
            CollisionEvent collisionEvent,
            ComponentLookup<Self> selfGroup,
            ComponentLookup<Opponent> opponentGroup)
                where Self : unmanaged, IComponentData
                where Opponent : unmanaged, IComponentData
        {
            if (selfGroup.HasComponent(collisionEvent.EntityA) && opponentGroup.HasComponent(collisionEvent.EntityB))
            {
                // EntityAが自身、EntityBが相手として衝突
                return (true, collisionEvent.EntityA, collisionEvent.EntityB);
            }
            if (selfGroup.HasComponent(collisionEvent.EntityB) && opponentGroup.HasComponent(collisionEvent.EntityA))
            {
                // EntityAが相手、EntityBが自身として衝突
                return (true, collisionEvent.EntityB, collisionEvent.EntityA);
            }

            // 衝突無し
            return (false, Entity.Null, Entity.Null);
        }

        /// <summary>
        /// 衝突した二つのエンティティの順番を明確にします
        /// </summary>
        /// <typeparam name="Opponent">相手側のコンポーネント</typeparam>
        /// <param name="collisionEvent">コリジョンイベント</param>
        /// <param name="selfEntity">自分側のエンティティ</param>
        /// <param name="opponentGroup">相手側のコンポーネントグループ</param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) CollisionEvent<Opponent>(
            CollisionEvent collisionEvent,
            Entity selfEntity,
            ComponentLookup<Opponent> opponentGroup)
                where Opponent : unmanaged, IComponentData
        {
            if (selfEntity == collisionEvent.EntityA && opponentGroup.HasComponent(collisionEvent.EntityB))
            {
                // EntityAが自身、EntityBが相手として衝突
                return (true, collisionEvent.EntityA, collisionEvent.EntityB);
            }
            if (selfEntity == collisionEvent.EntityB && opponentGroup.HasComponent(collisionEvent.EntityA))
            {
                // EntityAが相手、EntityBが自身として衝突
                return (true, collisionEvent.EntityB, collisionEvent.EntityA);
            }

            // 衝突無し
            return (false, Entity.Null, Entity.Null);
        }

        /// <summary>
        /// 衝突した二つのエンティティの順番を明確にします
        /// </summary>
        /// <typeparam name="Self">自分側のコンポーネント</typeparam>
        /// <param name="collisionEvent">コリジョンイベント</param>
        /// <param name="selfGroup">自分側のコンポーネントグループ</param>
        /// <param name="opponentEntity">相手側のエンティティ</param>
        /// <returns></returns>
        public static (bool IsHit, Entity self, Entity opponent) CollisionEvent<Self>(
            CollisionEvent collisionEvent,
            ComponentLookup<Self> selfGroup,
            Entity opponentEntity)
                where Self : unmanaged, IComponentData
        {
            if (selfGroup.HasComponent(collisionEvent.EntityA) && opponentEntity == collisionEvent.EntityB)
            {
                // EntityAが自身、EntityBが相手として衝突
                return (true, collisionEvent.EntityA, collisionEvent.EntityB);
            }
            if (selfGroup.HasComponent(collisionEvent.EntityB) && opponentEntity == collisionEvent.EntityA)
            {
                // EntityAが相手、EntityBが自身として衝突
                return (true, collisionEvent.EntityB, collisionEvent.EntityA);
            }

            // 衝突無し
            return (false, Entity.Null, Entity.Null);
        }
    }
}