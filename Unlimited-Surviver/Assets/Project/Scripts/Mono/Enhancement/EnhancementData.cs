using UnityEngine;

public enum EnhancementContents : byte
{
    PlayerSpeed,            // プレイヤーの移動速度
    ExpCollectRange,        // Exp取得可能範囲
    PistolFireRate,         // ピストルの発射レート
    PistolBulletDamage,     // ピストルの攻撃力
}

public enum EnhancementCalculation
{
    Increase,
    Multiply,
    PercentIncrease,
}

namespace Mono
{
    //TODO: バッファーコンポーネントで管理
    [CreateAssetMenu(menuName = "ScriptableObject/EnhancementData")]
    public class EnhancementData : ScriptableObject
    {
        [SerializeField]    // 能力
        private EnhancementContents enhancementType;
        [SerializeField]    // 最初から実装するか
        private bool isAwake;
        [SerializeField]    // 最初の値
        private float firstValue;
        [SerializeField]    // 計算方法
        private EnhancementCalculation calculationType;
        [SerializeField]    // 強化値
        private float enhancementValue;
        [Space]
        [Header("UI")]
        [SerializeField]    // 強化名
        private string enhancementTitle;
        [SerializeField]    // イメージアイコン
        private Sprite enhancementIcon;
        [SerializeField]    // 説明文
        private string enhancementDescription;

        public EnhancementContents EnhancementType => enhancementType;
        public bool IsAwake => isAwake;
        public float FirstValue => firstValue;
        public EnhancementCalculation CalculationType => calculationType;
        public float EnhancementValue => enhancementValue;
        public string EnhancementTitle => enhancementTitle;
        public Sprite EnhancementIcon => enhancementIcon;
        public string EnhancementDescription => enhancementDescription;
    }
}