using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct HitDamageComponent : IComponentData, IEnableableComponent
    {
        public bool IsUIShowing;    // UI�ɕ\������
        public bool IsDistributed;  // �_���[�W��^������
        public float DamageValue;   // �_���[�W�l
        public float3 Position;     // �_���[�W���^����ꂽ���W
    }
}