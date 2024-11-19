using System.Collections.Generic;
using UnityEngine;

public sealed class DamageUIPool : MonoBehaviour, IObjectPool<HitDamageView>
{
    [SerializeField]    // �_���[�WUI�̃v���n�u
    private HitDamageView damagePrefab;
    [SerializeField]    // �v�[���̏����T�C�Y
    private int poolSize = 10;

    // �I�u�W�F�N�g�v�[��
    private Queue<HitDamageView> pool;

    private void Awake()
    {
        // �v�[���̐���
        pool = new Queue<HitDamageView>();

        // �v�[����������
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewDamageView();
        }
    }

    /// <summary>
    /// �g�p����Ă��Ȃ��r���[���擾���܂�
    /// </summary>
    /// <returns></returns>
    public HitDamageView Get()
    {
        if (pool.Count == 0)
        {
            // �v�[������Ȃ�V�����쐬
            CreateNewDamageView();
        }
        // �I�u�W�F�N�g�𗘗p�\�ɂ���
        HitDamageView damageView = pool.Dequeue();
        damageView.gameObject.SetActive(true);

        return damageView;
    }

    /// <summary>
    /// �I�u�W�F�N�g���v�[���ɕԊ҂��܂�
    /// </summary>
    /// <param name="damageView"></param>
    public void Return(HitDamageView damageView)
    {
        // �I�u�W�F�N�g���v�[���ɕۊǂ���
        damageView.gameObject.SetActive(false);
        pool.Enqueue(damageView);
    }

    /// <summary>
    /// �V�����I�u�W�F�N�g���쐬���܂�
    /// </summary>
    /// <returns></returns>
    private HitDamageView CreateNewDamageView()
    {
        HitDamageView damageView = Instantiate(damagePrefab, transform);
        damageView.gameObject.SetActive(false);
        pool.Enqueue(damageView);

        return damageView;
    }
}