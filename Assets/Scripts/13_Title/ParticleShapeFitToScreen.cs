using UnityEngine;

/// <summary>
/// タイトル画面背景でのパーティクルシステムの形状をカメラの画面サイズに合わせて調整するためのコンポーネント。
/// </summary>

[ExecuteAlways]
public class ParticleShapeFitToScreen : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private ParticleSystem targetParticleSystem;

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Depth")]
    [SerializeField, Min(0.01f)]
    private float distanceFromCamera = 10f;

    [Header("Options")]
    [SerializeField] private bool updateEveryFrame = false;

    private void Awake()
    {
        Setup();
        Fit();
    }

    private void Start()
    {
        Setup();
        Fit();
    }

    private void Update()
    {
        if (updateEveryFrame)
            Fit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Setup();
        Fit();
    }
#endif

    private void Setup()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetParticleSystem == null)
            targetParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void Fit()
    {
        if (targetParticleSystem == null)
        {
            Debug.LogWarning($"{nameof(ParticleShapeFitToScreen)}: targetParticleSystem が未設定です。", this);
            return;
        }

        if (targetCamera == null)
        {
            Debug.LogWarning($"{nameof(ParticleShapeFitToScreen)}: targetCamera が未設定です。", this);
            return;
        }

        float height;
        float width;

        if (targetCamera.orthographic)
        {
            height = targetCamera.orthographicSize * 2f;
            width = height * targetCamera.aspect;
        }
        else
        {
            height = 2f * distanceFromCamera * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            width = height * targetCamera.aspect;
        }

        var shape = targetParticleSystem.shape;

        shape.scale = new Vector3(width, height, shape.scale.z);
    }
}