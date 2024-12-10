using DOTStoMono;
using Unity.Entities;

namespace DOTS
{
    public partial struct FirearmSystme : ISystem
    {
        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach (var item in SystemAPI.Query<TargetPointComponent>())
            {

            }
        }
    }
}