using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitDamageView : MonoBehaviour
{
    [SerializeField] private TMP_Text uiText;

    public void Show(float damage, Vector3 position)
    {
        uiText.text = damage.ToString();
        transform.position = position;
    }
}
