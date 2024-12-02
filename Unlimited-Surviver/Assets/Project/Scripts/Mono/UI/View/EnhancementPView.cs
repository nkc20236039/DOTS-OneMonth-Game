using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mono
{
    public class EnhancementPView : MonoBehaviour
    {
        [SerializeField]
        private GameObject canvas;
        [SerializeField]
        private GameObject[] selectButtons;

        private Image[] icon;
        private TMP_Text[] titleText;
        private TMP_Text[] descriptionText;

        private void Awake()
        {
            // TODO: 直接参照を修正する
            icon = new Image[selectButtons.Length];
            titleText = new TMP_Text[selectButtons.Length];
            descriptionText = new TMP_Text[selectButtons.Length];

            for (int i = 0; i < selectButtons.Length; i++)
            {
                icon[i] = selectButtons[i]
                    .transform
                    .GetChild(0)
                    .GetComponent<Image>();
                titleText[i] = selectButtons[i]
                    .transform
                    .GetChild(1)
                    .GetComponent<TMP_Text>();
                descriptionText[i] = selectButtons[i]
                    .transform
                    .GetChild(2)
                    .GetComponent<TMP_Text>();
            }
            canvas.SetActive(false);
        }

        public void Show(EnhancementData[] enhancementData)
        {
            // それぞれに情報を入れる
            for (int i = 0; i < selectButtons.Length; i++)
            {
                icon[i].sprite = enhancementData[i].EnhancementIcon;
                titleText[i].text = enhancementData[i].EnhancementTitle;
                descriptionText[i].text = enhancementData[i].EnhancementDescription;
            }

            // キャンバス表示
            canvas.SetActive(true);
        }
    }
}