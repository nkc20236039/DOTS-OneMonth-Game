using Unity.Entities;
using Unity.Mathematics;

namespace DOTStoMono
{
    public class TargetPointManagedSingleton : IComponentData
    {
        public float3 Position;
    }
}