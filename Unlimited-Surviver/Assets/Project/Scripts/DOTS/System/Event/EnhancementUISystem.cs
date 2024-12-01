using System;
using Unity.Entities;

namespace DOTS
{
    public partial class EnhancementUISystem : SystemBase
    {
        private EnhancementUIManagedSingleton completedEvent;

        protected override void OnCreate()
        {
            EntityManager.CreateSingleton(new EnhancementUIManagedSingleton());
            completedEvent = SystemAPI.ManagedAPI.GetSingleton<EnhancementUIManagedSingleton>();

            RequireForUpdate<EnhancementUIManagedSingleton>();
            RequireForUpdate<LevelSingleton>();
        }

        protected override void OnUpdate()
        {
            foreach (var level in SystemAPI.Query<RefRW<LevelSingleton>>())
            {
                if (level.ValueRO.IsLevelUp)
                {
                    completedEvent.Show?.Invoke();
                    level.ValueRW.IsLevelUp = false;
                }
            }
        }
    }
}