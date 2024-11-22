using Unity.Entities;

namespace DOTS
{
    public struct LevelCompoent : IComponentData
    {
        public int CurrentLevel;
        public float CurrentExp;

        public float NextLevelUp;
        public float AdditionExp;
    }
}