using System.Collections.Generic;
using UnityEngine;

public sealed class DamageUIPool : MonoBehaviour, IObjectPool<HitDamageView>
{
    [SerializeField]    // ダメージUIのプレハブ
    private HitDamageView damagePrefab;
    [SerializeField]    // プールの初期サイズ
    private int poolSize = 10;
    [SerializeField]
    private Camera viewCamera;

    // オブジェクトプール
    private Queue<HitDamageView> pool;

    private void Awake()
    {
        // プールの生成
        pool = new Queue<HitDamageView>();

        // プールを初期化
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewDamageView();
        }
    }

    /// <summary>
    /// 使用されていないビューを取得します
    /// </summary>
    /// <returns></returns>
    public HitDamageView Get()
    {
        if (pool.Count == 0)
        {
            // プールが空なら新しく作成
            CreateNewDamageView();
        }
        // オブジェクトを利用可能にする
        HitDamageView damageView = pool.Dequeue();
        damageView.gameObject.SetActive(true);

        return damageView;
    }

    /// <summary>
    /// オブジェクトをプールに返還します
    /// </summary>
    /// <param name="damageView"></param>
    public void Return(HitDamageView damageView)
    {
        // オブジェクトをプールに保管する
        damageView.gameObject.SetActive(false);
        pool.Enqueue(damageView);
    }

    /// <summary>
    /// 新しくオブジェクトを作成します
    /// </summary>
    /// <returns></returns>
    private HitDamageView CreateNewDamageView()
    {
        HitDamageView damageView = Instantiate(damagePrefab, transform);
        damageView.Initalize(viewCamera, this);
        damageView.gameObject.SetActive(false);
        pool.Enqueue(damageView);

        return damageView;
    }
}