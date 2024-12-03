﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityPhysicsExpansion;

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

            state.Dependency = new PlayerMovementJob
            {
                Player = SystemAPI.GetSingleton<PlayerSingleton>(),
                Speed = speed.Value,
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

        private void Execute(
            ref LocalTransform transform,
            ref PhysicsVelocity velocity,
            ref PhysicsMass mass,
            in PlayerInputComponent playerInput)
        {
            // 受け取った入力を平面へ変換
            float3 moveDirection = new
            (
                playerInput.MoveDirection.x,
                0,
                playerInput.MoveDirection.y
            );
            moveDirection = math.normalizesafe(moveDirection);

            // 速度を適用
            velocity.Linear
                = moveDirection
                * Speed;

            // Y座標を0に固定
            transform.Position.y = 0;

            // 移動をしていなければこれ以降の処理を実行しない
            // (入力を監視していてエンティティ全て処理が行われないことが確定しているためループを抜けてよい)
            if (math.distancesq(float3.zero, moveDirection) == 0) { return; }

            /*回転の計算*/
            quaternion currentRotation = transform.Rotation;
            quaternion lookRotation = quaternion.LookRotationSafe(moveDirection, math.up());

            // スムーズに回転させる
            transform.Rotation
                = math.slerp
                (
                    currentRotation,
                    lookRotation,
                    Player.RotationSpeed
                );

            // 物理の回転を固定
            mass.InverseInertia = float3.zero;
        }
    }
}