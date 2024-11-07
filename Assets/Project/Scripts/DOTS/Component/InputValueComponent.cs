using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PlayerInputComponent : IComponentData
    {
        public float2 MoveDirection;    // �ړ�����
        public bool IsAvoidInput;   // �����͂��s��ꂽ��
    }

    public struct AvoidComponent : IComponentData
    {
        public float AvoidPower;    // ����̈ړ���
        public float AvoidingTime;  // ����̗L������

        public float3 AvoidDirection;
        public bool IsAvoiding;     // ��𒆂�
        public float AvoidingElapsedTime;   // ����̌o�ߎ���
    }
}