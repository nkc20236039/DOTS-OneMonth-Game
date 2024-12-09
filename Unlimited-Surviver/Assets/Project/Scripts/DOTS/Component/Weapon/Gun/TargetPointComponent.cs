using Unity.Entities;
using Unity.Mathematics;

namespace DOTStoMono
{
    public struct TargetPointComponent : IComponentData
    {
        public float TargetAngle;
        public float Pitch;
    }
}