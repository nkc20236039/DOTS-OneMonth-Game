using Unity.Entities;

namespace DOTS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(InputUpdateGroup))]
    public partial class ActionUpdateGroup : ComponentSystemGroup { }
}