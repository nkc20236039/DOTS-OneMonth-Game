using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS
{
    /// <summary>
    /// プレイヤーの移動コントロール
    /// </summary>
    [UpdateInGroup(typeof(InputUpdateGroup))]
    [BurstCompile]
    public partial struct PlayerMovementSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<PlayerInputComponent>();
            state.RequireForUpdate<PlayerSingleton>();
            state.RequireForUpdate<EnhancementBuffer>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            // 時間停止されていたら処理を実行しない
            if (SystemAPI.Time.DeltaTime == 0) { return; }

            // 速度を取得
            var speed = EnhancemetTypeCollection
                .GetEnhancementValue(EnhancementContents.PlayerSpeed, state.EntityManager);

            // 速度が取得できなければ処理を終了
            if (speed == false) { return; }

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            state.Dependency = new PlayerMovementJob
            {
                Player = SystemAPI.GetSingleton<PlayerSingleton>(),
                Speed = speed.Value,
                ParallelEcb = ecb.AsParallelWriter(),
                FighterTiltGroup = SystemAPI.GetComponentLookup<FighterTiltComponent>(true)
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }
    }

    /// <summary>
    /// プレイヤー移動Job
    /// </summary>
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        [ReadOnly] public PlayerSingleton Player;
        [ReadOnly] public float Speed;
        [ReadOnly] public ComponentLookup<FighterTiltComponent> FighterTiltGroup;
        public EntityCommandBuffer.ParallelWriter ParallelEcb;

        [BurstCompile]
        private void Execute(
            [EntityIndexInQuery] int index,
            Entity player,
            ref LocalTransform transform,
            ref PhysicsVelocity velocity,
            ref PhysicsMass mass,
            in PlayerInputComponent playerInput,
            in DynamicBuffer<Child> children)
        {
            // 物理の回転を固定
            mass.InverseInertia = float3.zero;
            // オブジェクト基準の方向を取得
            float3 forward = math.forward(transform.Rotation);
            float3 right = math.mul(transform.Rotation, new float3(1, 0, 0));

            // 移動をしていなければ処理をしない
            if (math.distancesq(float2.zero, playerInput.MoveDirection) == 0)
            {
                velocity.Linear = forward * Player.PropulsionPower;

                foreach (var child in children)
                {
                    if (FighterTiltGroup.HasComponent(child.Value))
                    {
                        FighterTiltComponent fighterTilt = FighterTiltGroup[child.Value];
                        fighterTilt.TiltPower = 0;
                        ParallelEcb.SetComponent(index, child.Value, fighterTilt);
                    }
                }

                return;
            }
            // 移動方向を計算
            var forwardMoveDirection = forward * playerInput.MoveDirection.y;
            var rightMoveDirection = right * playerInput.MoveDirection.x;
            var moveDirection = forwardMoveDirection + rightMoveDirection;

            moveDirection = math.normalizesafe(moveDirection);

            // 移動量
            var movePower = moveDirection * Speed;
            // 前後入力方向に推進力をかける
            movePower += forward * Player.PropulsionPower;

            // 速度を適用
            velocity.Linear = movePower;

            // 機体を斜めにするために値を渡す
            foreach (var child in children)
            {
                if (FighterTiltGroup.HasComponent(child.Value))
                {
                    // 子オブジェクトのコンポーネントに値をセット
                    FighterTiltComponent fighterTilt = FighterTiltGroup[child.Value];
                    fighterTilt.TiltPower = playerInput.MoveDirection.x;
                    ParallelEcb.SetComponent(index, child.Value, fighterTilt);
                }
            }

            // 移動をしていなければこれ以降の処理を実行しない
            if (math.distancesq(float3.zero, rightMoveDirection) == 0) { return; }

            /*回転の計算*/
            quaternion currentRotation = transform.Rotation;
            quaternion lookRotation = quaternion.LookRotationSafe(rightMoveDirection, math.up());

            // スムーズに回転させる
            transform.Rotation
                = math.slerp
                (
                    currentRotation,
                    lookRotation,
                    Player.RotationSpeed
                );

        }
    }
}