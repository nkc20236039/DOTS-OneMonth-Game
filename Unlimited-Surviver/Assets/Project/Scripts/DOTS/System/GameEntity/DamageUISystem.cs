using Unity.Entities;

namespace DOTS
{
    // ヒープに深く依存しているsystemのため、なるべく少ない処理で終了させる
    public partial class DamageUISystem : SystemBase
    {
        private DamageManagedSingleton damageSingleton;

        protected override void OnCreate()
        {
            EntityManager.CreateSingleton(new DamageManagedSingleton
            {
                damagePresenter = new()
            });

            RequireForUpdate<DamageManagedSingleton>();
            RequireForUpdate<HealthComponent>();
        }

        protected override void OnUpdate()
        {
            if (damageSingleton == null)
            {
                damageSingleton = SystemAPI.ManagedAPI.GetSingleton<DamageManagedSingleton>();
            }

            Entities
                .ForEach((ref HealthComponent health) =>
                {

                }).Run();
        }
    }
}