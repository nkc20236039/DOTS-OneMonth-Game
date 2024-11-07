using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct PlayerInputComponent : IComponentData
    {
        public float2 MoveDirection;    // ˆÚ“®•ûŒü
        public bool IsAvoidInput;   // ‰ñ”ğ“ü—Í‚ªs‚í‚ê‚½‚©
    }

    public struct AvoidComponent : IComponentData
    {
        public float AvoidPower;    // ‰ñ”ğ‚ÌˆÚ“®—Ê
        public float AvoidingTime;  // ‰ñ”ğ‚Ì—LŒøŠÔ

        public float3 AvoidDirection;
        public bool IsAvoiding;     // ‰ñ”ğ’†‚©
        public float AvoidingElapsedTime;   // ‰ñ”ğ‚ÌŒo‰ßŠÔ
    }
}