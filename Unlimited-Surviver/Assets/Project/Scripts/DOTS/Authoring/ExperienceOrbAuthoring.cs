using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class ExperienceOrbAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float point;
        [SerializeField]
        private float attractedSpeed;
        [SerializeField]
        private float attractedRange;
        [SerializeField]
        private float dropAngleRange;
        [SerializeField]
        private float dropForce;

        private class ExperienceOrbBaker : Baker<ExperienceOrbAuthoring>
        {
            public override void Bake(ExperienceOrbAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ExperienceOrbComponent
                {
                    Point = authoring.point,
                    AttractedSpeed = authoring.attractedSpeed,
                    AttractedRange = authoring.attractedRange,
                    DropAngleRange = authoring.dropAngleRange,
                    DropForce = authoring.dropForce,
                });
                AddComponent(entity, new ExperienceMaterialSeedComponent());
            }
        }
    }
}