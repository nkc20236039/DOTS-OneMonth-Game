using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class EnemyHomingAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private float HomingAccuracy;

        private class EnemyHomingBaker : Baker<EnemyHomingAuthoring>
        {
            public override void Bake(EnemyHomingAuthoring authoring)
            {
                var enemy = GetEntity(TransformUsageFlags.None);
                // �Ǐ]�G�l�~�[�R���|�[�l���g�ǉ�
                AddComponent(enemy, new EnemyHomingComponent
                {
                    Speed = authoring.speed,
                    HomingAccuracy = authoring.HomingAccuracy
                });
                // �G�t���O�ǉ�
                AddComponent(enemy, typeof(EnemyTag));
            }
        }
    }
}