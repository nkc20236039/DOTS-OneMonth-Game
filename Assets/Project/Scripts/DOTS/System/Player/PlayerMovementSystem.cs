using Unity.Burst;
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
            state.RequireForUpdate<PlayerComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var transform, var velocity, var physicsMass, var move, var player) in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<PhysicsVelocity>,
                RefRW<PhysicsMass>,
                RefRO<PlayerInputComponent>,
                RefRO<PlayerComponent>>())
            {
                // 受け取った入力を平面へ変換
                float3 moveDirection = new
                (
                    move.ValueRO.MoveDirection.x,
                    0,
                    move.ValueRO.MoveDirection.y
                );
                moveDirection = math.normalizesafe(moveDirection);

                // 速度を適用
                velocity.ValueRW.Linear
                    = moveDirection
                    * player.ValueRO.Speed
                    * SystemAPI.Time.DeltaTime;

                // 移動をしていなければこれ以降の処理を実行しない
                // (入力を監視していてエンティティ全て処理が行われないことが確定しているためループを抜けてよい)
                if (math.distancesq(float3.zero, moveDirection) == 0) { return; }

                /*回転の計算*/
                quaternion currentRotation = transform.ValueRO.Rotation;
                quaternion lookRotation = quaternion.LookRotationSafe(moveDirection, math.up());

                // スムーズに回転させる
                transform.ValueRW.Rotation
                    = math.slerp
                    (
                        currentRotation,
                        lookRotation,
                        player.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime
                    );

                // 物理の回転を固定
                physicsMass.ValueRW.InverseInertia = float3.zero;
            }
        }
    }
}