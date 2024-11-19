using DOTS;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class HitDamagePresenter : MonoBehaviour
{
    [SerializeField]
    private IObjectPool<HitDamageView> hitDamagePool;
    [SerializeField]
    private Camera viewCamera;
    [SerializeField]
    private RectTransform canvasTransform;

    private EntityQuery query;
    private bool isInitalized;

    private void Start()
    {
        StartCoroutine(StartProcess());
    }

    private IEnumerator StartProcess()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // EntityManager�̏�������������̂�҂�
        yield return null;
        isInitalized = true;

        // EntityQuery���쐬����
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
        // �I�u�W�F�N�g�̈ʒu���J�����Ɏ��܂邩���ׂ�
        Vector3 targetDirection = (Vector3)hitDamage.Position - viewCamera.transform.position;
        float cameraDot = Vector3.Dot(targetDirection, viewCamera.transform.forward);

        // �J�����̊O��������I��
        if (cameraDot <= 0) { return; }

        HitDamageView ui = hitDamagePool.Get();

        // ���[���h���W��UI�̍��W�ɕϊ�����
        var screenPosition = viewCamera.WorldToScreenPoint(hitDamage.Position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasTransform,
            screenPosition,
            null,
            out Vector2 uiLocalPosition
        );

        ui.Show(hitDamage.DamageValue, uiLocalPosition);
    }
}
