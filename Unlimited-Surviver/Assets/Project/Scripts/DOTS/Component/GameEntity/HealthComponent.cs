using Unity.Entities;

namespace DOTS
{
    public struct HealthComponent : IComponentData
    {
        public uint MaxHealth;

        public int Health;
    }
}