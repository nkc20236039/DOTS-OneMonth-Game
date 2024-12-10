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
    private Vector2 screenCenter;
    private CrosshairTargetConverter crosshairConverter;

    void Start()
    {
        SetMovableSize();
        CrosshairPositionInitalize();
        crosshairConverter = new CrosshairTargetConverter();
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

    private void Look(InputAction.CallbackContext context)
    {
        // 時間が停止していたらこのメソッドの処理をしない
        if (Time.timeScale == 0) { return; }

        // 現在の位置とマウス移動量を取得
        Vector3 position = crosshairTransform.position;
        Vector3 delta = context.ReadValue<Vector2>();
        delta.y = 0;
        position += delta;

        // 画面内に制限
        Vector2 crosshairSize = crosshairTransform.sizeDelta * 0.5f;
        Vector2 screenHalfSize = screenMaxSize * 0.5f;

        // 制限をかける
        position.x = Mathf.Max(screenCenter.x - screenHalfSize.x + crosshairSize.x, position.x);
        position.x = Mathf.Min(screenCenter.x + screenHalfSize.x - crosshairSize.x, position.x);
        position.y = Mathf.Max(screenCenter.y - screenHalfSize.y + crosshairSize.y, position.y);
        position.y = Mathf.Min(screenCenter.y + screenHalfSize.y - crosshairSize.y, position.y);

        crosshairTransform.position = position;

        // 可動領域に対する位置を-1～1に納めてDOTSsystemに渡す
        var angleSign = Mathf.Sign(position.x - screenCenter.x);
        var angleStrength = Mathf.InverseLerp
        (
            0,
            screenHalfSize.x - crosshairSize.x,
            Mathf.Abs(position.x - screenCenter.x)
        );

        crosshairConverter.SetAngle(angleStrength * angleSign);
    }

    public float GetAngleToTarget(Vector2 playerPosition, Vector2 targetPosition, Vector2 referencePoint)
    {
        // ターゲットへのベクトルを計算
        Vector2 directionToTarget = targetPosition - playerPosition;

        // 基準点からプレイヤーへのベクトルを計算
        Vector2 referenceToPlayer = playerPosition - referencePoint;

        // Mathf.Atan2を使用して角度を計算（ラジアン）
        float angle = Mathf.Atan2(referenceToPlayer.y, referenceToPlayer.x)
                      - Mathf.Atan2(directionToTarget.y, directionToTarget.x);

        // ラジアンを度に変換
        float angleDegrees = angle * Mathf.Rad2Deg;

        // 角度を0から360の範囲に正規化（時計回りが正）
        angleDegrees = Mathf.Repeat(angleDegrees, 360);

        return angleDegrees - 180;
    }

    private void OnCrosshairInitalize(InputAction.CallbackContext context)
    {
        CrosshairPositionInitalize();
    }

    private void CrosshairPositionInitalize()
    {
        // クロスヘアを初期位置へ戻す
        Vector2 screenPosition = canvasTransform.sizeDelta * initalPosition;
        crosshairTransform.position = screenPosition;
    }

    private void SetMovableSize()
    {
        if (canvasTransform == null)
        {
            canvasTransform = GetComponent<RectTransform>();
        }

        screenMaxSize = canvasTransform.sizeDelta * limitRangeMove;
        screenCenter = canvasTransform.sizeDelta * 0.5f;
    }

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
        Gizmos.DrawWireCube(screenCenter, screenMaxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(canvasTransform.sizeDelta * initalPosition, 10);
    }
#endif
}
