using Unity.Entities;
using UnityEngine;

namespace DOTStoMono
{
    public class VirtualPlayerManagedSingleton : IComponentData
    {
        public Transform VirtualPlayerTransform;
    }
}