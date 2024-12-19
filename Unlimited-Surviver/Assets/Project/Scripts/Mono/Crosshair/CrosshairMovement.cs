using Mono;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas))]
public class CrosshairMovement : MonoBehaviour
{
    [SerializeField]
    private RectTransform crosshairTransform;
    [SerializeField]
    private Vector2 initalPosition;
    [SerializeField]
    private Vector2 limitRangeMove;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Camera viewCamera;
    [SerializeField]
    private GameObject demoObject;

    private PlayerInputAction inputAction;
    private RectTransform canvasTransform;
    private Vector2 screenMaxSize;
    private Vector2 crosshairCenter;
    private TargetPointSubscriber targetPointSubscriber;

    void Start()
    {
        SetMovableSize();
        CrosshairPositionInitalize(true);
        targetPointSubscriber = new TargetPointSubscriber();
        // InputSystemの準備
        inputAction = new();

        inputAction.IngamePlayer.Look.performed += Look;
        inputAction.IngamePlayer.Look.canceled += Look;
        inputAction.IngamePlayer.CrosshairReset.started += OnCrosshairInitalize;

        inputAction.Enable();

        // カーソル固定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// InputSystemのマウス移動にて呼び出し
    /// </summary>
    /// <param name="context"></param>
    private void Look(InputAction.CallbackContext context)
    {
        // 時間が停止していたら処理をしない
        if (Time.timeScale == 0) { return; }

        // 現在の位置とマウス移動量を取得
        var position = crosshairTransform.position;
        Vector3 delta = context.ReadValue<Vector2>();
        position += delta;

        // 画面内に制限
        var crosshairSize = crosshairTransform.sizeDelta * 0.5f;
        var screenHalfSize = screenMaxSize * 0.5f;

        // 制限をかける
        position.x = Mathf.Max(crosshairCenter.x - screenHalfSize.x + crosshairSize.x, position.x);
        position.x = Mathf.Min(crosshairCenter.x + screenHalfSize.x - crosshairSize.x, position.x);
        position.y = Mathf.Max(crosshairCenter.y - screenHalfSize.y + crosshairSize.y, position.y);
        position.y = Mathf.Min(crosshairCenter.y + screenHalfSize.y - crosshairSize.y, position.y);

        crosshairTransform.position = position;

        var signedAngleRatio = GetScreenRatio(position.x, crosshairCenter.x, screenHalfSize.x, crosshairSize.x);
        var pitchRatio = GetScreenRatio(position.y, crosshairCenter.y, screenHalfSize.y, crosshairSize.y);
        var targetRay = viewCamera.ScreenPointToRay(position);

        // 標的を登録する
        targetPointSubscriber.SetSignedTargetRatio
        (
            signedAngleRatio,
            pitchRatio,
            targetRay.origin,
            targetRay.direction
        );
    }

    /// <summary>
    /// 画面に対する位置の割合を取得する
    /// </summary>
    private float GetScreenRatio(float position, float center, float halfSize, float crosshairSize)
    {
        // 符号を取り出し
        var positionSign = Mathf.Sign(position - center);
        // 割合いを取得
        var positionRatio = Mathf.InverseLerp
        (
            0,
            halfSize - crosshairSize,
            Mathf.Abs(position - center)
        );
        // 符号付きの割合いを返却
        return positionRatio * positionSign;
    }

    /// <summary>
    /// InputSystemのマウス中央ボタンにて呼び出し
    /// </summary>
    private void OnCrosshairInitalize(InputAction.CallbackContext context)
    {
        CrosshairPositionInitalize(false);
    }

    /// <summary>
    /// クロスヘアの位置を初期位置へ戻す
    /// </summary>
    private void CrosshairPositionInitalize(bool isInital)
    {
        // クロスヘアを初期位置へ戻す
        Vector2 screenPosition = canvasTransform.sizeDelta * initalPosition;
        crosshairTransform.position = crosshairCenter;
        if (isInital) { return; }
        var targetRay = viewCamera.ScreenPointToRay(crosshairCenter);
        targetPointSubscriber.SetSignedTargetRatio(0, 0, targetRay.origin, targetRay.direction);
    }

    /// <summary>
    /// スクリーン上の可動範囲と中心を設定する
    /// </summary>
    private void SetMovableSize()
    {
        if (canvasTransform == null)
        {
            canvasTransform = GetComponent<RectTransform>();
        }

        screenMaxSize = canvasTransform.sizeDelta * limitRangeMove;
        crosshairCenter = canvasTransform.sizeDelta * initalPosition;
    }

    /* これ以降はEditor専用 */
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 範囲を0～1に制限
        limitRangeMove.x = Mathf.Clamp01(limitRangeMove.x);
        limitRangeMove.y = Mathf.Clamp01(limitRangeMove.y);
        initalPosition.x = Mathf.Clamp01(initalPosition.x);
        initalPosition.y = Mathf.Clamp01(initalPosition.y);
    }

    private void OnDrawGizmos()
    {
        SetMovableSize();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(crosshairCenter, screenMaxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(canvasTransform.sizeDelta * initalPosition, 10);
    }
#endif
}