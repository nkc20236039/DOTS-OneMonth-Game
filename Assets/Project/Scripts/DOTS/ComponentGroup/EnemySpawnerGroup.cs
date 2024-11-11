using Unity.Entities;

namespace DOTS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ActionUpdateGroup))]
    public partial class EnemySpawnerGroup : ComponentSystemGroup { }
}