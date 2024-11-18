using Unity.Entities;

namespace DOTS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class InputUpdateGroup : ComponentSystemGroup { }
}