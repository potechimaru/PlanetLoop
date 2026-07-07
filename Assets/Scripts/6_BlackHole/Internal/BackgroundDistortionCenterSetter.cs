using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BlackHoleのDistortionShaderに対し、画面上でのBlackHoleの位置を教えるクラス
/// </summary>
public class BackgroundDistortionCenterSetter : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform blackHole;
    [SerializeField] private Image backgroundImage;

    private Material _mat;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        _mat = backgroundImage.material;
    }

    private void LateUpdate()
    {
        Vector3 viewportPos = targetCamera.WorldToViewportPoint(blackHole.position);

        _mat.SetVector("_Center", new Vector4(viewportPos.x, viewportPos.y, 0f, 0f));
    }
}