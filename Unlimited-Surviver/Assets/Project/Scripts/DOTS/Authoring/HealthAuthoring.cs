using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth;

        private class HealthBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                // 体力コンポーネント追加
                AddComponent(entity, new HealthComponent
                {
                    MaxHealth = authoring.maxHealth,
                    Health = authoring.maxHealth
                });
                // ダメージコンポーネント追加
                AddComponent(entity, new HitDamageComponent());
                SetComponentEnabled<HitDamageComponent>(entity, false);
            }
        }
    }
}