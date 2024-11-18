using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DOTS
{
    // 必ず入力処理を使用するグループの最初に実行する
    [UpdateInGroup(typeof(InputUpdateGroup), OrderFirst = true)]
    [BurstCompile]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputAction inputActions;

        // 作成時に登録
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInputComponent>();

            // アクションを登録する
            inputActions = new();
            inputActions.IngamePlayer.Move.performed += OnMove;
            inputActions.IngamePlayer.Move.canceled += OnMove;
            inputActions.IngamePlayer.Avoid.started += OnAvoid;
            inputActions.Enable();
        }

        // 削除時に登録解除
        protected override void OnDestroy()
        {
            // アクションを登録する
            inputActions = new();
            inputActions.IngamePlayer.Move.performed -= OnMove;
            inputActions.IngamePlayer.Move.canceled -= OnMove;
            inputActions.IngamePlayer.Avoid.started -= OnAvoid;
            inputActions.Disable();
            inputActions.Dispose();
        }

        /// <summary>
        /// 移動入力をコンポーネントに登録します
        /// </summary>
        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();

            Entities.ForEach((ref PlayerInputComponent move) =>
            {
                move.MoveDirection = direction;
            }).Run();
        }

        /// <summary>
        /// 回避入力をコンポーネントに登録します
        /// </summary>
        private void OnAvoid(InputAction.CallbackContext context)
        {
            bool isAvoid = context.ReadValue<float>() != 0;

            Entities.ForEach((ref PlayerInputComponent input) =>
            {
                input.IsAvoidInput = isAvoid;
            }).Run();
        }

        // 毎フレーム更新は無し
        protected override void OnUpdate() { }
    }
}