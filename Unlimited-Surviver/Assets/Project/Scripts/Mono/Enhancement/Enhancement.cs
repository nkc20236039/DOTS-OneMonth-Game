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
        private PlayerSingleton player;
        private DynamicBuffer<EnhancementComponent> enhancementBuffer;

        private void Start()
        {
            // 初期化処理
            StartCoroutine(Initalize());
        }

        public void OnClik(EnhancementData enhancementData)
        {
            AddOrSetEnhancementComponent(enhancementData);
        }

        private IEnumerator Initalize()
        {
            // エンティティマネージャーを取得する
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            yield return null;

            // 必要なコンポーネントの取得
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerSingleton>();
            var playerEntity = entityManager
                .CreateEntityQuery(entityQueryBuilder)
                .GetSingletonEntity();

            // バッファを追加
            enhancementBuffer = entityManager
                .AddBuffer<EnhancementComponent>(playerEntity);

            foreach (var enhancementData in enhancementDecide.enhancementDataCollection)
            {
                if (enhancementData.IsAwake)
                {
                    // 最初から必要なコンポーネントを追加する
                    AddOrSetEnhancementComponent(enhancementData);
                }
            }
        }

        /// <summary>
        /// 強化するコンポーネントデータを追加もしくは上書きします
        /// </summary>
        private void AddOrSetEnhancementComponent(EnhancementData enhancementData)
        {
            for (int i = 0; i < enhancementBuffer.Length; i++)
            {
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

                    // 新しい強化コンポーネントがBuffer内に存在していたら書き換える
                    enhancementBuffer[i] = new EnhancementComponent
                    {
                        EnhancementType = enhancementData.EnhancementType,
                        Value = enhancementedValue
                    };

                    return;
                }
            }

            // 存在しなければ追加する
            enhancementBuffer.Add(new EnhancementComponent
            {
                EnhancementType = enhancementData.EnhancementType,
                Value = enhancementData.FirstValue,
            });
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