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
            // Jobに渡すものの準備
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
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
            (bool IsHit, Entity bullet, Entity environment) environmentInfo
                = PhysicsTriggerEvent.TriggerEventExplicit(triggerEvent, BulletGroup, EnvironmentGroup);
            // ゲームエンティティと弾
            (bool IsHit, Entity bullet, Entity gameEntity) gameEntityInfo
                = PhysicsTriggerEvent.TriggerEventExplicit(triggerEvent, BulletGroup, GameEntityGroup);

            if (environmentInfo.IsHit)
            {
                // 環境と弾が当たった場合
                // Bが銃弾とわかるためBを削除
                Ecb.DestroyEntity(environmentInfo.bullet);
            }

            if (gameEntityInfo.IsHit)
            {
                // 必要なコンポーネントを取得
                HealthComponent health;
                BulletComponent bullet;
                if (GameEntityGroup.TryGetComponent(gameEntityInfo.gameEntity, out health) == false) { return; }
                if (BulletGroup.TryGetComponent(gameEntityInfo.bullet, out bullet) == false) { return; }

                // HealthComponentを所持しているエンティティがBulletの発射主だったらダメージ処理をしない
                if (gameEntityInfo.gameEntity == bullet.Owner) { return; }
                // 発射主がチームのコンポーネントを所持していたらそのチームとの衝突判定も無視する

                // 当たった相手の体力を減少させる
                health.Health -= bullet.AttackDamage;
                Ecb.SetComponent(gameEntityInfo.gameEntity, health);
                // 弾を削除する
                Ecb.DestroyEntity(gameEntityInfo.bullet);
            }
        }
    }
}