using UnityEngine;

public enum Ability
{
    Speed,
    CollectRange,
    PistolFireRate,
    PistolBulletDamage,
}

namespace DOTS
{
    //TODO: バッファーコンポーネントで管理
    [CreateAssetMenu(menuName = "ScriptableObject/PowerUpData")]
    public class PowerUpData : ScriptableObject
    {
        [SerializeField]    // 能力
        private Ability abilityType;
        [SerializeField]    // 最初から実装するか
        private bool isAwake;
        [SerializeField]    // 最初の値
        private float firstValue;
        [SerializeField]    // 増幅量
        private float fluctuationAmount;

        public Ability AbilityType => abilityType;
        public bool IsAwake => isAwake;
        public float FirstValue => firstValue;
        public float FluctuationAmount => fluctuationAmount;
    }
}