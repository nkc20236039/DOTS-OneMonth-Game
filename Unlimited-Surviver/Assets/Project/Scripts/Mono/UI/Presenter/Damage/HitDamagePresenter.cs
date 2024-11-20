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
            // IObjectPool���t���Ă��Ȃ���Όx��
            Debug.LogError($"Hit Damage Pool Object�ɖ����Ȓl���Z�b�g����Ă��܂�\n{hitDamagePoolObject}��IObjectPool<HitDamageView>���A�^�b�`����Ă��܂���");
            hitDamagePoolObject = null;
        }
    }
#endif

    private void Start()
    {
        // �I�u�W�F�N�g�v�[�����擾
        hitDamagePool = hitDamagePoolObject.GetComponent<IObjectPool<HitDamageView>>();
        // ���̃L�����o�X�擾
        rootCanvasTransform = canvasTransform.root.GetComponent<RectTransform>();

        StartCoroutine(StartProcess());
    }

    private IEnumerator StartProcess()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // EntityManager�̏�������������̂�҂�
        yield return null;

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
