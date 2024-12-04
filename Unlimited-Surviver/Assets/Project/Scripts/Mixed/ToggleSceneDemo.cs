using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleSceneDemo : MonoBehaviour
{
    private World defaultWorld;
    private World emptyWorld;

    private void Awake()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;

        // 空のWorldを作成
        emptyWorld = new World("EmptyWorld");

        World.DefaultGameObjectInjectionWorld = emptyWorld;
    }

    private void Start()
    {
        Invoke("Toggle", 3);
    }

    private void Toggle()
    {
        SceneManager.LoadScene("DemoScene");
        World.DefaultGameObjectInjectionWorld = defaultWorld;
    }
}