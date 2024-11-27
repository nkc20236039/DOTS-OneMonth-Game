using System;
using Unity.Entities;

namespace DOTS
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class CompletedEventSystem : SystemBase
    {
        private CompletedEventManagedSingleton completedEvent;

        protected override void OnCreate()
        {
            EntityManager.CreateSingleton(new CompletedEventManagedSingleton());
            completedEvent = SystemAPI.ManagedAPI.GetSingleton<CompletedEventManagedSingleton>();

            RequireForUpdate<CompletedEventManagedSingleton>();
        }

        protected override void OnUpdate()
        {
            completedEvent.OnCompleted?.Invoke();
        }
    }
}