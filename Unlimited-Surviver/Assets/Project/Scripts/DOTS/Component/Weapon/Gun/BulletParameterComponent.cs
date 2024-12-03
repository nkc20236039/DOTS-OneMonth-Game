using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct BulletParameterComponent : IComponentData
    {
        public float Speed;     // 弾の速度
        public float Lifetime;  // 寿命
        public int AttackDamage;// 攻撃力

        public float Age;       // 存在した時間
    }

    public struct BulletComponent : IComponentData
    {
        public Entity Owner;   // この弾を撃った生存しているエンティティ
    }
}