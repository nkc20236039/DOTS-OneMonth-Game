using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class ExperienceOrbDropAuthoring : MonoBehaviour
    {
        [SerializeField]
        private int spawnAmount;
        [SerializeField]
        private GameObject experienceOrbPrefab;

        private class ExperienceOrbDropBaker : Baker<ExperienceOrbDropAuthoring>
        {
            public override void Bake(ExperienceOrbDropAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var orb = GetEntity(authoring.experienceOrbPrefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new ExperienceOrbDropComponent
                {
                    ExperienceOrb = orb,
                    SpawnAmount = authoring.spawnAmount,
                });
            }
        }
    }
}