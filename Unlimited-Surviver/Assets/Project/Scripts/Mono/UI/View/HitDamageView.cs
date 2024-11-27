using DOTS;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class HitDamageView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text uiText;
    [SerializeField]
    private Vector2 uiOffset;
    [SerializeField]
    private float displayTime;

    private Camera viewCamera;
    private IObjectPool<HitDamageView> objectPool;

    private bool isShow;
    private Vector3 displayPosition;

    public void Initalize(Camera viewCamera, IObjectPool<HitDamageView> objectPool)
    {
        this.viewCamera = viewCamera;
        this.objectPool = objectPool;

        // DOTSWorldから取得
        var world = World.DefaultGameObjectInjectionWorld;
        var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<CompletedEventManagedSingleton>();
        var completedEvent = world.EntityManager.CreateEntityQuery(entityQueryBuilder).GetSingletonRW<CompletedEventManagedSingleton>();
        completedEvent.OnCompleted += OnUIUpdate;
    }

    private void OnUIUpdate()
    {
        if (isShow)
        {
            transform.position = ConvertUIPosition(displayPosition);
        }
    }

    public void Show(float damage, Vector3 position)
    {
        // 表示準備
        isShow = true;
        displayPosition = position;
        transform.position = ConvertUIPosition(displayPosition);    // 表示するタイミングで位置を移動しておく
        uiText.text = damage.ToString();
        StartCoroutine(DisplayTime());
    }

    private IEnumerator DisplayTime()
    {
        yield return new WaitForSeconds(displayTime);
        // 非表示
        isShow = false;
        uiText.text = "";   // 次の表示時に一瞬表示されてしまうのを防止
        objectPool.Return(this);
    }

    private Vector2 ConvertUIPosition(Vector3 position)
    {
        // ワールド座標をUIの座標に変換する
        Vector2 screenPosition = viewCamera.WorldToScreenPoint(position);

        screenPosition += uiOffset;

        return screenPosition;
    }
}
