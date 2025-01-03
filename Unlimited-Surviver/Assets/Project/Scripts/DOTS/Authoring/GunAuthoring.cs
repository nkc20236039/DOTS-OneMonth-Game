using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class GunAuthoring : MonoBehaviour
    {
        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private float shotInterval;
        [SerializeField]
        private Vector2 gunOffset;
        [SerializeField]
        private Vector2 muzzleOffset;

        private class GunBaker : Baker<GunAuthoring>
        {
            public override void Bake(GunAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new WeaponComponent
                {
                    Offset = authoring.gunOffset,
                });
                AddComponent(entity, new PistolComponent
                {
                    Bullet = bullet,
                    ShotInterval = authoring.shotInterval,
                    Offset = authoring.muzzleOffset,
                });
            }
        }
    }
}