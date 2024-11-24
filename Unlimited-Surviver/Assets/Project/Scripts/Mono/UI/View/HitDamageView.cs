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

    private bool isShow;
    private Vector3 displayPosition;


    private void Update()
    {

    }

    public void Show(float damage, Vector3 position, IObjectPool<HitDamageView> objectPool)
    {
        isShow = true;
        displayPosition = position;

        uiText.text = damage.ToString();
        transform.position = position;
        StartCoroutine(DisplayTime(objectPool));
    }

    private IEnumerator DisplayTime(IObjectPool<HitDamageView> objectPool)
    {
        yield return new WaitForSeconds(displayTime);
        isShow = false;
        objectPool.Return(this);
    }
}
