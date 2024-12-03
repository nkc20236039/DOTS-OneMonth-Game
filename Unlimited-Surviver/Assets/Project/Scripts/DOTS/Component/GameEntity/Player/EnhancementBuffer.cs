using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public struct EnhancementBuffer : IBufferElementData
    {
        public EnhancementContents EnhancementType;
        public float Value;
    }
}