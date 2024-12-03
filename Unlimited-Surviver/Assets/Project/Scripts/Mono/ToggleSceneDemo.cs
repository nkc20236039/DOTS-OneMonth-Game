using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleSceneDemo : MonoBehaviour
{
    private void Start()
    {
        Invoke("Toggle", 3);
    }

    private void Toggle()
    {
        SceneManager.LoadScene("DemoScene");
    }
}
