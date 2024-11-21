using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace DOTS
{
    [MaterialProperty("_Seed")]
    public struct ExperienceMaterialSeedComponent : IComponentData
    {
        public float2 Value;
    }
}