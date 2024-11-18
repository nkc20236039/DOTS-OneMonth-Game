using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public class DamageManagedSingleton : IComponentData
    {
        public DamagePresenter damagePresenter;
    }
}