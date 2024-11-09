using Unity.Entities;
using UnityEngine;

namespace DOTStoMono
{
    public class VirtualPlayerManagedComponent : IComponentData
    {
        public Transform VirtualPlayerTransform;
    }
}