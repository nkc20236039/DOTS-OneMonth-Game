using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace DOTS
{
    public struct EnhancemetTypeGetter
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
        public static EnhancementResult GetEnhancementValue(EntityManager entityManager, EnhancementContents enhancementType)
        {
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerSingleton, EnhancementComponent>();
            var enhancementBuffer = entityManager
                .CreateEntityQuery(entityQueryBuilder)
                .GetSingletonBuffer<EnhancementComponent>();

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