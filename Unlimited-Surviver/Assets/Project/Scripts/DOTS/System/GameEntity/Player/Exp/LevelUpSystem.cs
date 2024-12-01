using Unity.Burst;
using Unity.Entities;

namespace DOTS
{
    [BurstCompile]
    public partial struct LevelUpSystem : ISystem
    {
        [BurstCompile]
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<LevelSingleton>();
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach (var level in SystemAPI.Query<RefRW<LevelSingleton>>())
            {
                // 追加経験値が次のレベルアップを超えていたら超えなくなるまで繰り返す
                while (level.ValueRO.NextLevelUpValue <= level.ValueRO.AdditionExp)
                {
                    // 追加経験値から今回のレベルアップに必要な分を減算
                    level.ValueRW.AdditionExp -= level.ValueRO.NextLevelUpValue;
                    LevelUp(level);
                }

                // 残った追加経験値を現在の経験値に加算
                level.ValueRW.CurrentExp += level.ValueRO.AdditionExp;
                level.ValueRW.AdditionExp = 0;

                // 追加したうえで次のレベルアップを超えていたらレベルを上げる
                if (level.ValueRO.NextLevelUpValue <= level.ValueRO.CurrentExp)
                {
                    // 追加経験値から今回のレベルアップに必要な分を減算
                    level.ValueRW.CurrentExp -= level.ValueRO.NextLevelUpValue;
                    LevelUp(level);
                }
            }
        }

        [BurstCompile]
        private void LevelUp(RefRW<LevelSingleton> level)
        {
            // レベルを加算
            level.ValueRW.CurrentLevel++;
            level.ValueRW.IsLevelUp = true;
            // 次のレベルアップを決定
            level.ValueRW.NextLevelUpValue += level.ValueRO.NextLevelUpIncrease;
        }
    }
}