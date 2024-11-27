using DOTS;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class ExpGagePresenter : MonoBehaviour
    {
        [SerializeField]
        private ExpGageView view;
        [SerializeField]
        private float maxGageValue;

        private LevelSingleton level;
        private EntityManager entityManager;

        private void Start()
        {
            StartCoroutine(Initalize());
        }

        private void UIObserver()
        {
            // 表示
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LevelSingleton>();
            level = entityManager
                .CreateEntityQuery(in entityQueryBuilder)
                .GetSingleton<LevelSingleton>();
            Show(level);
        }

        private IEnumerator Initalize()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // EntityManagerの準備が完了するのを待つ
            yield return null;

            // EntityQueryを作成する
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LevelSingleton>();
            level = entityManager
                .CreateEntityQuery(in entityQueryBuilder)
                .GetSingleton<LevelSingleton>();

            // Queryをリセットしてイベントシングルトンを追加
            entityQueryBuilder
                .Reset()
                .WithAll<CompletedEventManagedSingleton>();
            var completedEvent = entityManager
                .CreateEntityQuery(in entityQueryBuilder)
                .GetSingleton<CompletedEventManagedSingleton>();

            // DOTS処理終了時に呼び出すイベントに追加
            completedEvent.OnCompleted += UIObserver;
        }

        private void Show(LevelSingleton level)
        {
            float ratio = Mathf.InverseLerp(0, level.NextLevelUpValue, level.CurrentExp);
            view.Show(level.CurrentLevel, maxGageValue * ratio);
        }
    }
}