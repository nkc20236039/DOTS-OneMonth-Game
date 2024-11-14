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
            // Jobに渡すものの準備
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // 衝突判定のJobを作成
            state.Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(),
                HealthGroup = SystemAPI.GetComponentLookup<HealthComponent>(),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(),
            }.Schedule(simulation, state.Dependency);

            // 衝突判定Jobが終了することを待機
            state.Dependency.Complete();

            // 生存中の弾を管理するJobを作成
            state.Dependency = new BulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ParallelEcb = ecb.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            // Jobの後処理
            state.Dependency.Complete();
            JobHandle.ScheduleBatchedJobs();
        }
    }

    /// <summary>
    /// 銃弾の基本挙動
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
            // 時間を経過させる
            bullet.Age += DeltaTime;
            if (bullet.Lifetime > bullet.Age)
            {
                // 生存期間は直進させる
                transform.Position += math.forward(transform.Rotation) * bullet.Speed * DeltaTime;
            }
            else
            {
                // 生存時間を過ぎたら削除する
                ParallelEcb.DestroyEntity(index, entity);
            }
        }
    }

    /// <summary>
    /// 銃弾の衝突判定
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
            /*必要な衝突情報のboolを設定*/
            // 環境と弾が当たった
            bool isEnvironmentHitAtoB
                = EnvironmentGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isEnvironmentHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && EnvironmentGroup.HasComponent(triggerEvent.EntityB);
            // 敵と弾が当たった
            bool isHealthHitAtoB
                = HealthGroup.HasComponent(triggerEvent.EntityA)
                && BulletGroup.HasComponent(triggerEvent.EntityB);
            bool isHealthHitBtoA
                = BulletGroup.HasComponent(triggerEvent.EntityA)
                && HealthGroup.HasComponent(triggerEvent.EntityB);

            // 環境と弾が当たったら弾を削除
            if (isEnvironmentHitAtoB)
            {
                // Bが銃弾とわかるためBを削除
                Ecb.DestroyEntity(triggerEvent.EntityB);
            }
            else if (isEnvironmentHitBtoA)
            {
                // Aが銃弾とわかるためAを削除
                Ecb.DestroyEntity(triggerEvent.EntityA);
            }

            if (isHealthHitAtoB)
            {
                // 必要なコンポーネントを取得
                HealthComponent _health;
                BulletComponent _bullet;
                if (HealthGroup.TryGetComponent(triggerEvent.EntityA, out _health) == false) { return; }
                if (BulletGroup.TryGetComponent(triggerEvent.EntityB, out _bullet) == false) { return; }


                // 当たったら相手の体力を減らす
                _health = Attack(triggerEvent.EntityB, _health, _bullet.AttackDamage);
                Ecb.SetComponent(triggerEvent.EntityA, _health);
            }
            else if (isHealthHitBtoA)
            {
                // 必要なコンポーネントを取得
                HealthComponent health;
                BulletComponent bullet;
                if (BulletGroup.TryGetComponent(triggerEvent.EntityA, out bullet) == false) { return; }
                if (HealthGroup.TryGetComponent(triggerEvent.EntityB, out health) == false) { return; }

                // 当たったら相手の体力を減らす
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