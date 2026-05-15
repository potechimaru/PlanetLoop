using UnityEngine;

public class FullScreenShaderDisabler : MonoBehaviour
{
    [SerializeField]
    private Material _fullScreenMaterial;

    private static readonly int OuterLimitRedStrengthId =
        Shader.PropertyToID("_OuterLimitRedStrength");

    private void Awake()
    {
        if (_fullScreenMaterial == null)
            return;

        _fullScreenMaterial.SetFloat(
            OuterLimitRedStrengthId,
            0f
        );
    }
}