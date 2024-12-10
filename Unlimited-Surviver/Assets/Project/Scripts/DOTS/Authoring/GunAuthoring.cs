using DOTStoMono;
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
        private Vector2 muzzleOffset;
        [SerializeField]
        private float maxAngle;

        private class GunBaker : Baker<GunAuthoring>
        {
            public override void Bake(GunAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic);


                AddComponent(entity, new PistolComponent
                {
                    Bullet = bullet,
                    ShotInterval = authoring.shotInterval,
                    Offset = authoring.muzzleOffset,
                    MaxAngle = authoring.maxAngle,
                });
                AddComponent(entity, new TargetPointComponent());
            }
        }
    }
}