using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DOTS
{
    // �K�����͏������g�p����O���[�v�̍ŏ��Ɏ��s����
    [UpdateInGroup(typeof(InputUpdateGroup), OrderFirst = true)]
    [BurstCompile]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputAction inputActions;

        // �쐬���ɓo�^
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInputComponent>();

            // �A�N�V������o�^����
            inputActions = new();
            inputActions.IngamePlayer.Move.performed += OnMove;
            inputActions.IngamePlayer.Move.canceled += OnMove;
            inputActions.IngamePlayer.Avoid.started += OnAvoid;
            inputActions.Enable();
        }

        // �폜���ɓo�^����
        protected override void OnDestroy()
        {
            // �A�N�V������o�^����
            inputActions = new();
            inputActions.IngamePlayer.Move.performed -= OnMove;
            inputActions.IngamePlayer.Move.canceled -= OnMove;
            inputActions.IngamePlayer.Avoid.started -= OnAvoid;
            inputActions.Disable();
            inputActions.Dispose();
        }

        /// <summary>
        /// �ړ����͂��R���|�[�l���g�ɓo�^���܂�
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
        /// �����͂��R���|�[�l���g�ɓo�^���܂�
        /// </summary>
        private void OnAvoid(InputAction.CallbackContext context)
        {
            bool isAvoid = context.ReadValue<float>() != 0;

            Entities.ForEach((ref PlayerInputComponent input) =>
            {
                input.IsAvoidInput = isAvoid;
            }).Run();
        }

        // ���t���[���X�V�͖���
        protected override void OnUpdate() { }
    }
}