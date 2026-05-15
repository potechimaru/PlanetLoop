using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FullScreenRendererFeatureController : MonoBehaviour
{
    [Header("Renderer Data")]
    [SerializeField] private UniversalRendererData _rendererData;

    [Header("Feature Name")]
    [SerializeField] private string _featureName = "Full Screen Pass Renderer Feature";

    private ScriptableRendererFeature _targetFeature;

    private void Awake()
    {
        FindFeature();
    }

    private void FindFeature()
    {
        if (_rendererData == null)
        {
            Debug.LogError("[FullScreenRendererFeatureController] RendererData is null.", this);
            return;
        }

        foreach (var feature in _rendererData.rendererFeatures)
        {
            if (feature == null) continue;

            if (feature.name == _featureName)
            {
                _targetFeature = feature;
                return;
            }
        }

        Debug.LogError($"[FullScreenRendererFeatureController] Feature not found: {_featureName}", this);
    }

    public void SetActive(bool active)
    {
        if (_targetFeature == null)
            return;

        _targetFeature.SetActive(active);
    }
}