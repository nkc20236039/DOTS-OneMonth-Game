using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PlayerInputComponent : IComponentData
    {
        public float2 MoveDirection;    // 移動方向
        public bool IsAvoidInput;   // 回避入力が行われたか
    }

    public struct AvoidComponent : IComponentData
    {
        public float AvoidPower;    // 回避の移動量
        public float AvoidingTime;  // 回避の有効時間

        public float3 AvoidDirection;
        public bool IsAvoiding;     // 回避中か
        public float AvoidingElapsedTime;   // 回避の経過時間
    }
}