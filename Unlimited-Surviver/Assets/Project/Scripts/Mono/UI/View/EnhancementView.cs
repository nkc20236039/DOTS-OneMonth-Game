using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mono
{
    public class EnhancementView : MonoBehaviour
    {
        [SerializeField]
        private GameObject canvas;
        [SerializeField]
        private GameObject[] selectButtons;

        private EnhancementButtonTarget[] enhancementButtonTargets;
        private Image[] icon;
        private TMP_Text[] titleText;
        private TMP_Text[] descriptionText;

        private void Awake()
        {
            // TODO: 直接参照を修正する
            enhancementButtonTargets = new EnhancementButtonTarget[selectButtons.Length];
            icon = new Image[selectButtons.Length];
            titleText = new TMP_Text[selectButtons.Length];
            descriptionText = new TMP_Text[selectButtons.Length];

            for (int i = 0; i < selectButtons.Length; i++)
            {
                enhancementButtonTargets[i] = selectButtons[i]
                        .GetComponent<EnhancementButtonTarget>();

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
                enhancementButtonTargets[i].TargetEnhancementData = enhancementData[i];
                icon[i].sprite = enhancementData[i].EnhancementIcon;
                titleText[i].text = enhancementData[i].EnhancementTitle;
                descriptionText[i].text = enhancementData[i].EnhancementDescription;
            }

            // キャンバス表示
            canvas.SetActive(true);
        }

        public void Hide()
        {
            // キャンバス非表示
            canvas.SetActive(false);
        }
    }
}