using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitDamageView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text uiText;
    [SerializeField]
    private float displayTime;

    public void Show(float damage, Vector3 position, IObjectPool<HitDamageView> objectPool)
    {
        uiText.text = damage.ToString();
        transform.position = position;
        StartCoroutine(DisplayTime(objectPool));
    }

    private IEnumerator DisplayTime(IObjectPool<HitDamageView> objectPool)
    {
        yield return new WaitForSeconds(displayTime);
        objectPool.Return(this);
    }
}
