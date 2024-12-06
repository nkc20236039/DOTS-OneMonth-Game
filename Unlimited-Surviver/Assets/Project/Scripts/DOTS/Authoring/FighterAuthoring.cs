using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class FighterAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float maxTiltAngle;
        [SerializeField]
        private float TiltSpeed;

        private class FighterBaker : Baker<FighterAuthoring>
        {
            public override void Bake(FighterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FighterParameterComponent
                {
                    MaxTiltAngle = authoring.maxTiltAngle,
                    TiltSpeed = authoring.TiltSpeed,
                });
                AddComponent(entity, new FighterTiltComponent());
            }
        }
    }
}