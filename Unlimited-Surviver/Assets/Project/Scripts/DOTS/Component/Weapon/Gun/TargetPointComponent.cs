using Unity.Entities;
using Unity.Mathematics;

namespace DOTStoMono
{
    public struct TargetPointComponent : IComponentData
    {
        public float TargetAngle;
        public float Pitch;
        public float3 TargetPosition;
        public float3 TargetDirection;
    }
}