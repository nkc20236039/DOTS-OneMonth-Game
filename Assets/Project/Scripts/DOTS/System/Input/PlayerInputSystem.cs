using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DOTS
{
    // �K�����͏������g�p����O���[�v�̍ŏ��Ɏ��s����
    [UpdateInGroup(typeof(InputUpdateGroup), OrderFirst = true)]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputAction inputActions;

        // �쐬���ɓo�^
        protected override void OnCreate()
        {
            // �A�N�V������o�^����
            inputActions = new();
            inputActions.IngamePlayer.Move.performed += OnMove;
            inputActions.IngamePlayer.Move.canceled += OnMove;
            inputActions.IngamePlayer.Avoid.started += OnAvoid;
            inputActions.IngamePlayer.Avoid.canceled += OnAvoid;
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
            inputActions.IngamePlayer.Avoid.canceled -= OnAvoid;
            inputActions.Disable();
            inputActions.Dispose();
        }

        /// <summary>
        /// �ړ����͂��R���|�[�l���g�ɓo�^���܂�
        /// </summary>
        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();

            Entities.ForEach((ref MovementComponent move) =>
            {
                move.MoveDirection = direction;
            }).Run();
        }

        /// <summary>
        /// �����͂��R���|�[�l���g�ɓo�^���܂�
        /// </summary>
        private void OnAvoid(InputAction.CallbackContext context)
        {
            bool isAvoid = context.ReadValue<bool>();

            Entities.ForEach((ref AvoidComponent avoid) =>
            {
                avoid.IsAvoidInput = isAvoid;
            }).Run();
        }

        // ���t���[���X�V�͖���
        protected override void OnUpdate() { }
    }
}