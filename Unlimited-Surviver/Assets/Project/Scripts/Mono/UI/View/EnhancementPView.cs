using TMPro;
using UnityEngine;

namespace Mono
{
    public class EnhancementPView : MonoBehaviour
    {
        [SerializeField]
        private GameObject canvas;

        public void Show()
        {
            canvas.SetActive(true);
        }
    }
}