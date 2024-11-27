using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct HitDamageComponent : IComponentData
    {
        public bool IsDistributed;  // ダメージを与えたか
        public float DamageValue;   // ダメージ値
        public float3 Position;     // ダメージが与えられた座標
    }
}