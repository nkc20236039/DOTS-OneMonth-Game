using TMPro;
using UnityEngine;

namespace Mono
{
    public class ExpGageView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform expGage;
        [SerializeField]
        private TMP_Text canvasText;

        public void Show(int level, float exp)
        {
            expGage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, exp);
            canvasText.text = level.ToString();
        }
    }
}