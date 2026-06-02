using UnityEngine;

public class BlackHoleDirectionCompass : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform blackHole;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private RectTransform arrowRect;

    [Header("Settings")]
    [SerializeField] private float angleOffset = 0f;
    [SerializeField] private bool hideWhenBlackHoleIsBehind = true;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        if (arrowRect == null)
            arrowRect = GetComponent<RectTransform>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (blackHole == null || targetCamera == null || arrowRect == null)
            return;

        Vector3 screenPos = targetCamera.WorldToScreenPoint(blackHole.position);

        if (hideWhenBlackHoleIsBehind && screenPos.z < 0f)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = (Vector2)screenPos - screenCenter;

        if (dir.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // ‰æ‘œ‚ªuãŒü‚«v‚ðŠî€‚Éì‚ç‚ê‚Ä‚¢‚é‚Ì‚Å -90
        arrowRect.localEulerAngles = new Vector3(
            0f,
            0f,
            angle - 90f + angleOffset
        );
    }

    private void SetVisible(bool visible)
    {
        if (_canvasGroup == null) return;

        _canvasGroup.alpha = visible ? 1f : 0f;
    }
}