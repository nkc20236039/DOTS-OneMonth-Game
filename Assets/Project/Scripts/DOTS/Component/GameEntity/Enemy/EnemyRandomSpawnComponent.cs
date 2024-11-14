using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{

    // マップ上に配置される固定スポナー
    public struct EnemyRandomSpawnComponent : IComponentData
    {
        public float3 Origin;
    }
}