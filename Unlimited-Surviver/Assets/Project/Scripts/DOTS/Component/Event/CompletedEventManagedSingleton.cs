using System;
using Unity.Entities;

namespace DOTS
{
    public class CompletedEventManagedSingleton : IComponentData
    {
        public Action OnCompleted;
    }
}