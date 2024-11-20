using Unity.Entities;

namespace DOTS
{
    public struct ExperienceOrbDropComponent : IComponentData
    {
        public Entity ExperienceOrb;    // 経験値オーブ
        public float SpawnAmount;       // 召喚量
    }
}