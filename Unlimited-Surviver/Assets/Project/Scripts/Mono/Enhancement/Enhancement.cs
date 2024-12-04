using DOTS;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class Enhancement : MonoBehaviour
    {
        [SerializeField]
        private EnhancementDecideData enhancementDecide;

        private EntityManager entityManager;
        private Entity playerEntity;
        private DynamicBuffer<EnhancementBuffer> enhancementBuffer;

        private void Start()
        {
            // 初期化処理
            StartCoroutine(Initalize());
        }

        public void EnhancementUpdate(EnhancementData enhancementData)
        {
            Time.timeScale = 1;
            AddOrSetEnhancementComponent(enhancementData);
        }

        private IEnumerator Initalize()
        {
            // エンティティマネージャーを取得する
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            yield return null;

            // 必要なコンポーネントの取得

            playerEntity = new();
            yield return new WaitUntil(() =>
            {
                var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerSingleton>();
                return entityManager
                    .CreateEntityQuery(entityQueryBuilder)
                    .TryGetSingletonEntity<Entity>(out playerEntity);
            });

            // バッファを追加
            enhancementBuffer = entityManager
                .AddBuffer<EnhancementBuffer>(playerEntity);

            foreach (var enhancementData in enhancementDecide.enhancementDataCollection)
            {
                if (enhancementData.IsAwake)
                {
                    // 最初から必要なコンポーネントを追加する
                    AddOrSetEnhancementComponent(enhancementData, true);
                }
            }
        }

        /// <summary>
        /// 強化するコンポーネントデータを追加もしくは上書きします
        /// </summary>
        private void AddOrSetEnhancementComponent(EnhancementData enhancementData, bool isInit = false)
        {
            enhancementBuffer = entityManager.GetBuffer<EnhancementBuffer>(playerEntity);
            for (int i = 0; i < enhancementBuffer.Length; i++)
            {
                if (isInit)
                {
                    // 初期化する場合は強化処理を行わない
                    break;
                }

                if (enhancementBuffer[i].EnhancementType == enhancementData.EnhancementType)
                {
                    // 強化後の数値を計算する
                    float enhancementedValue
                        = CalculateEnhancementValue
                        (
                            enhancementBuffer[i].Value,
                            enhancementData.EnhancementValue,
                            enhancementData.CalculationType
                        );

                    Debug.Log($"{enhancementData.EnhancementType}が存在しているため強化しました。\n{enhancementBuffer[i].Value}から{enhancementedValue}へ強化");

                    // 新しい強化コンポーネントがBuffer内に存在していたら書き換える
                    enhancementBuffer[i] = new EnhancementBuffer
                    {
                        EnhancementType = enhancementData.EnhancementType,
                        Value = enhancementedValue
                    };

                    return;
                }
            }

            // 存在しなければ追加する
            enhancementBuffer.Add(new EnhancementBuffer
            {
                EnhancementType = enhancementData.EnhancementType,
                Value = enhancementData.FirstValue,
            });
            Debug.Log($"{enhancementData.EnhancementType}を新規追加しました。\n初期値: {enhancementData.FirstValue}");
        }

        /// <summary>
        /// 計算方法に従って2つの値を計算する
        /// </summary>
        private float CalculateEnhancementValue(float currentValue, float calculateValue, EnhancementCalculation calculationType)
        {
            switch (calculationType)
            {
                // 加算
                case EnhancementCalculation.Increase:
                    return currentValue + calculateValue;
                // 乗算
                case EnhancementCalculation.Multiply:
                    return currentValue * calculateValue;
                // 割増し
                case EnhancementCalculation.PercentIncrease:
                    return currentValue + currentValue / calculateValue;
                default:
                    return currentValue;
            }
        }
    }
}