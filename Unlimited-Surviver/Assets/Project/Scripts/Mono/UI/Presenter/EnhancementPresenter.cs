using DOTS;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class EnhancementPresenter : MonoBehaviour
    {
        [SerializeField]
        private EnhancementView view;
        [SerializeField]
        private EnhancementDecider model;

        private void Start()
        {
            model.UIDisplay += Show;
        }

        private void Show(EnhancementData[] enhancementDataCollection)
        {
            // 表示
            view.Show(enhancementDataCollection);
        }
    }
}