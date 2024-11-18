using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class BulletCollisionTagAuthoring : MonoBehaviour
    {
        private class EnvironmentTagBaker : Baker<BulletCollisionTagAuthoring>
        {
            public override void Bake(BulletCollisionTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BulletCollisionTag());
            }
        }
    }
}