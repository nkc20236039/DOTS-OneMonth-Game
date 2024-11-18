using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField]
        private uint maxHealth;

        private class HealthBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new HealthComponent
                {
                    MaxHealth = authoring.maxHealth,
                    Health = (int)authoring.maxHealth
                });
            }
        }
    }
}