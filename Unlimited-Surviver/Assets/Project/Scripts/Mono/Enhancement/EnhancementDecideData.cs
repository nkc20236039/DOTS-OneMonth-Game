using System.Collections.Generic;
using UnityEngine;

namespace Mono
{
    [CreateAssetMenu(menuName = "ScriptableObject/EnhancementDecideData")]
    public class EnhancementDecideData : ScriptableObject
    {
        public EnhancementData[] enhancementDataCollection;
    }
}