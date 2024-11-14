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
                EnvironmentGroup = SystemAPI.GetComponentLookup<EnvironmentTag>(true),
                GameEntityGroup = SystemAPI.GetComponentLookup<HealthComponent>(true),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(true),
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
        [ReadOnly] public ComponentLookup<EnvironmentTag> EnvironmentGroup;
        [ReadOnly] public ComponentLookup<HealthComponent> GameEntityGroup;
        [ReadOnly] public ComponentLookup<BulletComponent> BulletGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            // 環境と弾
            var environmentInfo = TriggerEventExplicit(triggerEvent, EnvironmentGroup, BulletGroup);
            // ゲームエンティティと弾
            var gameEntityInfo = TriggerEventExplicit(triggerEvent, GameEntityGroup, BulletGroup);

            if (environmentInfo.IsHit)
            {
                // 環境と弾が当たった場合
                // Bが銃弾とわかるためBを削除
                Ecb.DestroyEntity(environmentInfo.EntityB);
            }

            if (gameEntityInfo.IsHit)
            {
                // 必要なコンポーネントを取得
                HealthComponent health;
                BulletComponent bullet;
                if (GameEntityGroup.TryGetComponent(gameEntityInfo.EntityA, out health) == false) { return; }
                if (BulletGroup.TryGetComponent(gameEntityInfo.EntityB, out bullet) == false) { return; }

                // HealthComponentを所持しているエンティティがBulletの発射主だったらダメージ処理をしない
                if (gameEntityInfo.EntityA == bullet.Owner) { return; }
                // 発射主がチームのコンポーネントを所持していたらそのチームとの衝突判定も無視する

                // 当たったら相手の体力を減らす
                // TODO: 引数と戻り値の関係性が分かりにくいので修正する
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