using System.Collections;
using System.Collections.Generic;
using Unity.Scenes;
using UnityEngine;

public class InitalizeLoadSubScene : MonoBehaviour
{
    private void Awake()
    {
        var subScene = GetComponent<SubScene>();
    }
}
