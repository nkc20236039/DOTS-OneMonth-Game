using Unity.Entities;

namespace DOTS
{
    public struct HealthComponent : IComponentData
    {
        public float MaxHealth;

        public float Health;
    }
}