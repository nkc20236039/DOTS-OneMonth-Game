#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EditorCameraSync : MonoBehaviour
{
    private Camera gameCamera;
    private Camera sceneCamera;

    private void Start()
    {
        gameCamera = GetComponent<Camera>();
        sceneCamera = SceneView.lastActiveSceneView.camera;
    }

    void Update()
    {
        // シーンビューが無ければ実行しない
        if (SceneView.lastActiveSceneView == null) { return; }
        if (SceneView.lastActiveSceneView.camera != sceneCamera)
        {
            // カメラのインスタンスが異なれば新しく代入
            sceneCamera = SceneView.lastActiveSceneView.camera;
        }

        gameCamera.transform.position = sceneCamera.transform.position;
        gameCamera.transform.rotation = sceneCamera.transform.rotation;
        gameCamera.fieldOfView = sceneCamera.fieldOfView;
    }
}
#endif