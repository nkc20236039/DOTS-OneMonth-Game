using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    public class ConcentratedLine : MonoBehaviour
    {
        private enum OriginCorner
        {
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight,
        }

        [SerializeField]
        private OriginCorner originCorner;

        private Vector2 targetCornerPosition;

        private void Start()
        {
            switch (originCorner)
            {
                case OriginCorner.BottomLeft:
                    targetCornerPosition = Vector2.zero;
                    break;
                case OriginCorner.BottomRight:
                    targetCornerPosition = new Vector2(Screen.width, 0);
                    break;
                case OriginCorner.TopLeft:
                    targetCornerPosition = new Vector2(0, Screen.height);
                    break;
                case OriginCorner.TopRight:
                    targetCornerPosition = new Vector2(Screen.width, Screen.height);
                    break;
                default:
                    break;
            }

            // 長さを決定
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors
            (
                RectTransform.Axis.Horizontal,
                Screen.width + Screen.height
            );
        }

        private void Update()
        {
            Vector2 targetDirection = (Vector2)transform.position - targetCornerPosition;
            targetDirection = targetDirection.normalized;

            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}