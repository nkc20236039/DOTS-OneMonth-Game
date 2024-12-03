using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace DOTS
{
    public struct EnhancemetTypeCollection
    {
        public struct EnhancementResult
        {
            private bool isFound;
            public float Value;

            public EnhancementResult(bool isFound, float value)
            {
                this.isFound = isFound;
                Value = value;
            }

            public static implicit operator bool(EnhancementResult result)
            {
                return result.isFound;
            }
        }

        [BurstCompile]
        public static EnhancementResult GetEnhancementValue(EnhancementContents enhancementType, EntityManager entityManager)
        {
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerSingleton, EnhancementBuffer>();
            var enhancementBuffer = entityManager
                .CreateEntityQuery(entityQueryBuilder)
                .GetSingletonBuffer<EnhancementBuffer>();

            foreach (var enhancement in enhancementBuffer)
            {
                if (enhancement.EnhancementType == enhancementType)
                {
                    return new EnhancementResult(true, enhancement.Value);
                }
            }
            return new EnhancementResult(false, 0);
        }
    }
}