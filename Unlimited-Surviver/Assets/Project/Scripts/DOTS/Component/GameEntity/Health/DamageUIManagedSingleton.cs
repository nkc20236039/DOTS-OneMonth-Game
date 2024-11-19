using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public class DamageUIManagedSingleton : IComponentData
    {
        public HitDamagePresenter damagePresenter;
    }
}