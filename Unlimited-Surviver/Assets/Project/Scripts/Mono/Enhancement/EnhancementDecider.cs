using DOTS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// TODO: 可読性が低いコードであるため修正する

namespace Mono
{
    public class EnhancementDecider : MonoBehaviour
    {
        [SerializeField]
        private EnhancementDecideData enhancementDecide;
        [SerializeField]
        private int pickupCount;

        private EnhancementData[] chosenData;
        private List<EnhancementData> enhancementDataList;

        private Action<EnhancementData[]> uiDisplay;
        public event Action<EnhancementData[]> UIDisplay
        {
            add { uiDisplay += value; }
            remove { uiDisplay -= value; }
        }

        private void Start()
        {
            // 選択されたデータ配列を初期化
            chosenData = new EnhancementData[pickupCount];
            enhancementDataList = enhancementDecide.enhancementDataCollection.ToList();

            StartCoroutine(Initalize());
        }

        private IEnumerator Initalize()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // EntityManagerの準備が完了するのを待つ
            yield return null;

            // EntityQueryを作成する
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LevelSingleton>();

            // Queryをリセットしてイベントシングルトンを追加
            entityQueryBuilder
                .Reset()
                .WithAll<EnhancementUIManagedSingleton>();
            var completedEvent = entityManager
                .CreateEntityQuery(in entityQueryBuilder)
                .GetSingleton<EnhancementUIManagedSingleton>();

            // DOTS処理終了時に呼び出すイベントに追加
            completedEvent.Show += EnhancementDecide;
        }

        private void EnhancementDecide()
        {
            // 重なりが出ないように強化するデータを選ぶ
            NoOverlapPickup(pickupCount);
            // 時間停止
            Time.timeScale = 0; // プロジェクトのサイズを考慮してこれを許容

            // UIに表示するものを決定
            uiDisplay?.Invoke(chosenData);
        }

        /// <summary>
        /// 乱数が同じ数を返さないように数字をまとめる
        /// </summary>
        /// <param name="count">乱数の最大</param>
        /// <returns></returns>
        private void NoOverlapPickup(int count)
        {
            // もし表示する量より強化データの量が少なければ最初のデータで埋める
            if (count < enhancementDecide.enhancementDataCollection.Length)
            {
                Debug.LogWarning("強化データが不足しているため最初のデータで埋めました");
                for (int i = 0; i < count; i++)
                {
                    chosenData[i] = enhancementDecide.enhancementDataCollection[0];
                }
                return;
            }

            // リストを初期化
            enhancementDataList = enhancementDecide
                .enhancementDataCollection
                .ToList();

            // ランダムに強化内容を選出
            for (int i = 0; i < chosenData.Length; i++)
            {
                int pickup = UnityEngine.Random.Range(0, enhancementDataList.Count);

                chosenData[i] = enhancementDataList[pickup];
                enhancementDataList.RemoveAt(pickup);
            }
        }
    }
}