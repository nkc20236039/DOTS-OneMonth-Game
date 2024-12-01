using System;
using Unity.Entities;

namespace DOTS
{
    public class EnhancementUIManagedSingleton : IComponentData
    {
        public Action Show;
    }
}