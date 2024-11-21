using Unity.Entities;

namespace DOTS
{
    public struct ExperienceOrbComponent : IComponentData
    {
        public float Point;
        public float AttractedSpeed;
        public float AttractedRange;

        public float DropAngleRange;    // 経験値が落ちるときにはじける角度の量
        public float DropForce;         // 経験値が落ちるときの飛ぶ力

        public bool IsSpawned;
    }
}