using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (SceneView.lastActiveSceneView.camera == null) { return; }
        gameCamera.transform.position = sceneCamera.transform.position;
        gameCamera.transform.rotation = sceneCamera.transform.rotation;
        gameCamera.fieldOfView = sceneCamera.fieldOfView;
    }
}
