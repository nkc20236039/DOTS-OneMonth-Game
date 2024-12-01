using Unity.Entities;

namespace DOTS
{
    public struct LevelSingleton : IComponentData
    {
        public float NextLevelUpIncrease;  // 次のレベルへの倍率
        public float NextLevelUpValue;  // 次のレベルアップまでの数
        public float GettableRange;

        public float AdditionExp;       // 加算された経験値

        public bool IsLevelUp;      // レベルアップが実行されたか
        public int CurrentLevel;    // 現在のレベル
        public float CurrentExp;    // 現在のEXP(経験値)
    }
}