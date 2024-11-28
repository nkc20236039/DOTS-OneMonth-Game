using DOTS;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class HitDamagePresenter : MonoBehaviour
    {
        [SerializeField]
        private GameObject hitDamagePoolObject;
        [SerializeField]
        private Camera viewCamera;
        [SerializeField]
        private RectTransform canvasTransform;

        private IObjectPool<HitDamageView> hitDamagePool;
        private RectTransform rootCanvasTransform;
        private EntityQuery displayDamageQuery;
        private EntityManager entityManager;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (hitDamagePoolObject != null && !hitDamagePoolObject.TryGetComponent<IObjectPool<HitDamageView>>(out _))
            {
                // IObjectPoolが付いていなければ警告
                Debug.LogError($"Hit Damage Pool Objectに無効な値がセットされています\n{hitDamagePoolObject}にIObjectPool<HitDamageView>がアタッチされていません");
                hitDamagePoolObject = null;
            }
        }
#endif

        private void Start()
        {
            // オブジェクトプールを取得
            hitDamagePool = hitDamagePoolObject.GetComponent<IObjectPool<HitDamageView>>();
            // 元のキャンバス取得
            rootCanvasTransform = canvasTransform.root.GetComponent<RectTransform>();

            StartCoroutine(Initalize());
        }

        private void UIObserver()
        {
            // 表示するダメージUIが無ければ終了
            if (0 == displayDamageQuery.CalculateEntityCount()) { }

            using (var hitDamageEntities = displayDamageQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (var hitDamageEntity in hitDamageEntities)
                {
                    var hitDamage = entityManager.GetComponentData<HitDamageComponent>(hitDamageEntity);

                    // ダメージをUIに表示するフラグが立っていれば処理をする
                    if (entityManager.IsEnabled(hitDamageEntity))
                    {
                        // 表示
                        Show(hitDamage);
                        // 表示が完了したエンティティのフラグをおろす
                        entityManager.SetComponentEnabled<DisplayOnUITag>(hitDamageEntity, false);
                    }
                }
            }
        }

        private IEnumerator Initalize()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // EntityManagerの準備が完了するのを待つ
            yield return null;

            // EntityQueryを作成する
            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<HitDamageComponent, DisplayOnUITag>();
            displayDamageQuery = entityManager.CreateEntityQuery(in entityQueryBuilder);

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

        private void Show(HitDamageComponent hitDamage)
        {
            // オブジェクトの位置がカメラに収まるか調べる
            Vector3 targetDirection = (Vector3)hitDamage.Position - viewCamera.transform.position;
            float cameraDot = Vector3.Dot(targetDirection, viewCamera.transform.forward);

            // カメラの外だったら終了
            if (cameraDot <= 0) { return; }

            HitDamageView ui = hitDamagePool.Get();

            ui.Show(hitDamage.DamageValue, hitDamage.Position);
        }
    }
}