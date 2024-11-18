using Unity.Entities;

namespace DOTS
{
    // �q�[�v�ɐ[���ˑ����Ă���system�̂��߁A�Ȃ�ׂ����Ȃ������ŏI��������
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