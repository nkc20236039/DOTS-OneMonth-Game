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
        if(SceneView.lastActiveSceneView == null) { return; }
/*
        if (SceneView.lastActiveSceneView.hasFocus)
        {
            // シーンビューにフォーカスされていればゲームビューのDisplayをカメラのDiplayに変える
            EditorWindow gameView = EditorWindow.GetWindow(GetTypeFromName("UnityEditor.GameView"));
            var displayField = gameView.GetType().GetField("m_TargetDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (displayField != null)
            {
                displayField.SetValue(gameView, gameCamera.targetDisplay);
                gameView.Repaint();  // Display変更後にビューを更新
            }
        }
*/
        gameCamera.transform.position = sceneCamera.transform.position;
        gameCamera.transform.rotation = sceneCamera.transform.rotation;
        gameCamera.fieldOfView = sceneCamera.fieldOfView;
    }

    private static System.Type GetTypeFromName(string name)
    {
        return System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(type => type.FullName == name);
    }
}
