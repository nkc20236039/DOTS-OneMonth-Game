using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class EnvironmentTagAuthoring : MonoBehaviour
    {
        private class EnvironmentTagBaker : Baker<EnvironmentTagAuthoring>
        {
            public override void Bake(EnvironmentTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EnvironmentTag());
            }
        }
    }
}