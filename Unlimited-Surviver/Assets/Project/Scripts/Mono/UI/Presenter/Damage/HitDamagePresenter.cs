using DOTS;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
    private EntityQuery query;

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

        StartCoroutine(StartProcess());
    }

    private IEnumerator StartProcess()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // EntityManagerの準備が完了するのを待つ
        yield return null;

        // EntityQueryを作成する
        var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<HitDamageComponent>();
        query = entityManager.CreateEntityQuery(in entityQueryBuilder);
        WaitUntil waitUntil = new WaitUntil(() => query.CalculateEntityCount() != 0);

        while (true)
        {
            yield return waitUntil;

            using (var hitDamageEntities = query.ToEntityArray(Allocator.Temp))
            {
                foreach (var hitDamageEntity in hitDamageEntities)
                {
                    var hitDamage = entityManager.GetComponentData<HitDamageComponent>(hitDamageEntity);

                    if (!hitDamage.IsUIShowing)
                    {
                        Show(hitDamage);
                        hitDamage.IsUIShowing = true;
                        entityManager.SetComponentEnabled<HitDamageComponent>(hitDamageEntity, false);
                    }
                }
            }
        }
    }

    private void Show(HitDamageComponent hitDamage)
    {
        // オブジェクトの位置がカメラに収まるか調べる
        Vector3 targetDirection = (Vector3)hitDamage.Position - viewCamera.transform.position;
        float cameraDot = Vector3.Dot(targetDirection, viewCamera.transform.forward);

        // カメラの外だったら終了
        if (cameraDot <= 0) { return; }

        HitDamageView ui = hitDamagePool.Get();

        // ワールド座標をUIの座標に変換する
        var screenPosition = viewCamera.WorldToScreenPoint(hitDamage.Position);

        screenPosition.y += 125;

        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            rootCanvasTransform,
            screenPosition,
            null,
            out Vector2 uiLocalPosition
        );

        ui.Show(hitDamage.DamageValue, screenPosition, hitDamagePool);
    }
}
