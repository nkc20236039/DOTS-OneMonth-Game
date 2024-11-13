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

        public class BulletBaker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BulletComponent
                {
                    Speed = authoring.speed,
                    Lifetime = authoring.lifeTime
                });
            }
        }
    }
}