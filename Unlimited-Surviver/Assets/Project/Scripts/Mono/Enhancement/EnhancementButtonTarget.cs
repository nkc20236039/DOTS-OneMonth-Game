using UnityEngine;

namespace Mono
{
    public class EnhancementButtonTarget : MonoBehaviour
    {
        [SerializeField]
        private Enhancement enhancement;

        private EnhancementData targetEnhancementData;
        public EnhancementData TargetEnhancementData { set => targetEnhancementData = value; }

        public void OnClick()
        {
            enhancement.EnhancementUpdate(targetEnhancementData);
        }
    }
}