using DOTS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float propulsionPower;  // 推進力
        [SerializeField]
        private float pitchRotation;
        [SerializeField]
        private float rotationSpeed; // 回転の速度
        [SerializeField]
        private float avoidPower;   // 回避力
        [SerializeField]
        private float avoidingTime; // 回避が有効な時間
        [SerializeField]
        private float firstNextLevelUpValue;    // 最初のレベルアップに必要な経験値
        [SerializeField]
        private float nextLevelUpIncrease; // レベルアップ倍率
        [SerializeField]
        private float gettableRange;    // 経験値を取得可能な範囲

        private class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity player = GetEntity(TransformUsageFlags.None);

                // プレイヤーにコンポーネントを追加
                AddComponent(player, typeof(PlayerInputComponent));
                AddComponent(player, new PlayerSingleton
                {
                    PropulsionPower = authoring.propulsionPower,
                    RotationSpeed = authoring.rotationSpeed,
                    PitchRotation = authoring.pitchRotation,
                });
                // 回避を追加
                AddComponent(player, new AvoidComponent
                {
                    AvoidPower = authoring.avoidPower,
                    AvoidingTime = authoring.avoidingTime,
                });
                // レベルを追加
                AddComponent(player, new LevelSingleton
                {
                    CurrentLevel = 1,
                    NextLevelUpIncrease = authoring.nextLevelUpIncrease,
                    NextLevelUpValue = authoring.firstNextLevelUpValue,
                    GettableRange = authoring.gettableRange,
                });
            }
        }
    }
}