using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas))]
public class CrosshairMovement : MonoBehaviour
{
    [SerializeField]
    private RectTransform crosshairTransform;

    private PlayerInputAction inputAction;
    private Vector2 screenMaxSize;

    void Start()
    {
        screenMaxSize = new Vector2(Screen.width, Screen.height);

        // InputSystemの準備
        inputAction = new();

        inputAction.IngamePlayer.Look.performed += Look;
        inputAction.IngamePlayer.Look.canceled += Look;

        inputAction.Enable();

        // カーソル固定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Look(InputAction.CallbackContext context)
    {
        // 時間が停止していたらこのメソッドの処理をしない
        if (Time.timeScale == 0) { return; }

        // 現在の位置とマウス移動量を取得
        Vector3 position = crosshairTransform.position;
        Vector3 delta = context.ReadValue<Vector2>();
        position += delta;

        // 画面内に制限
        Vector2 offset = crosshairTransform.sizeDelta * 0.5f;

        position.x = Mathf.Max(offset.x, position.x);
        position.x = Mathf.Min(screenMaxSize.x - offset.x, position.x);
        position.y = Mathf.Max(offset.y, position.y);
        position.y = Mathf.Min(screenMaxSize.y - offset.y, position.y);

        crosshairTransform.position = position;
    }
}
