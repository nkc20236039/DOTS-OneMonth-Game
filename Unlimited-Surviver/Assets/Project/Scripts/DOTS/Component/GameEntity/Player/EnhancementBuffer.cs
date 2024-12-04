using Unity.Entities;

namespace DOTS
{
    public struct EnhancementBuffer : IBufferElementData
    {
        public EnhancementContents EnhancementType;
        public float Value;
    }
}