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
                // �̗̓R���|�[�l���g�ǉ�
                AddComponent(entity, new HealthComponent
                {
                    MaxHealth = authoring.maxHealth,
                    Health = authoring.maxHealth
                });
                // �_���[�W�R���|�[�l���g�ǉ�
                AddComponent(entity, new HitDamageComponent());
                SetComponentEnabled<HitDamageComponent>(entity, false);
            }
        }
    }
}