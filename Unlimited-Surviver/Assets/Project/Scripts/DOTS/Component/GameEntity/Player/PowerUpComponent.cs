using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public struct PowerUpComponent : IComponentData
    {
        public Ability AbilityType;

    }
}