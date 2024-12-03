using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class BulletAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private float lifeTime;
        [SerializeField]
        private int attackDamage;

        private class BulletBaker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BulletParameterComponent
                {
                    Speed = authoring.speed,
                    Lifetime = authoring.lifeTime,
                    AttackDamage = authoring.attackDamage,
                });
            }
        }
    }
}