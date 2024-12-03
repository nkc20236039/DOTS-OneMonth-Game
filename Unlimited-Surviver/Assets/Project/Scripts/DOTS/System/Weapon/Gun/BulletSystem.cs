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
            state.RequireForUpdate<BulletParameterComponent>();
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
                GameEntityGroup = SystemAPI.GetComponentLookup<HitDamageComponent>(true),
                BulletParameterGroup = SystemAPI.GetComponentLookup<BulletParameterComponent>(true),
                BulletGroup = SystemAPI.GetComponentLookup<BulletComponent>(true),
                Transform = SystemAPI.GetComponentLookup<LocalTransform>(true),
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
            ref BulletParameterComponent bullet,
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
        [ReadOnly] public ComponentLookup<HitDamageComponent> GameEntityGroup;
        [ReadOnly] public ComponentLookup<BulletParameterComponent> BulletParameterGroup;
        [ReadOnly] public ComponentLookup<BulletComponent> BulletGroup;
        [ReadOnly] public ComponentLookup<LocalTransform> Transform;

        public void Execute(TriggerEvent triggerEvent)
        {
            // 環境と弾
            (bool IsHit, Entity bullet, Entity environment) environmentInfo
                = CollisionResponseExplicit.TriggerEvent(triggerEvent, BulletParameterGroup, EnvironmentGroup);
            // ゲームエンティティと弾
            (bool IsHit, Entity bullet, Entity gameEntity) gameEntityInfo
                = CollisionResponseExplicit.TriggerEvent(triggerEvent, BulletParameterGroup, GameEntityGroup);

            if (environmentInfo.IsHit)
            {
                // 環境と弾が当たった場合
                // Bが銃弾とわかるためBを削除
                Ecb.DestroyEntity(environmentInfo.bullet);
            }

            if (gameEntityInfo.IsHit)
            {
                // 必要なコンポーネントを取得
                HitDamageComponent hitDamage;
                BulletParameterComponent bulletParameter;
                BulletComponent bullet;
                if (GameEntityGroup.TryGetComponent(gameEntityInfo.gameEntity, out hitDamage) == false) { return; }
                if (BulletParameterGroup.TryGetComponent(gameEntityInfo.bullet, out bulletParameter) == false) { return; }
                if (BulletGroup.TryGetComponent(gameEntityInfo.bullet, out bullet) == false) { return; }

                // DamageComponentを所持しているエンティティがBulletの発射主だったらダメージ処理をしない
                if (gameEntityInfo.gameEntity == bullet.Owner) { return; }
                // 発射主がチームのコンポーネントを所持していたらそのチームとの衝突判定も無視する

                // 当たった相手にダメージの情報を与える
                LocalTransform transform;
                if (Transform.TryGetComponent(gameEntityInfo.gameEntity, out transform) == false) { return; }
                Ecb.SetComponentEnabled<DisplayOnUITag>(gameEntityInfo.gameEntity, true);
                Ecb.SetComponent(gameEntityInfo.gameEntity, new HitDamageComponent
                {
                    IsDistributed = false,
                    DamageValue = bulletParameter.AttackDamage,
                    Position = transform.Position,
                });

                // 弾を削除する
                Ecb.DestroyEntity(gameEntityInfo.bullet);
            }
        }
    }
}