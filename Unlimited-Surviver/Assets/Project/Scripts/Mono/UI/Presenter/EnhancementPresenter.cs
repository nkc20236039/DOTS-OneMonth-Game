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
        private EnhancementPView view;

        private void Start()
        {
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
            completedEvent.Show += Show;
        }

        private void Show()
        {
            view.Show();
        }
    }
}