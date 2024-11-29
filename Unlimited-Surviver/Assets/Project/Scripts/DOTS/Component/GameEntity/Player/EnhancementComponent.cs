using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public struct EnhancementComponent : IBufferElementData
    {
        public EnhancementContents EnhancementType;
        public float Value;
    }
}