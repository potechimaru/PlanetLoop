using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class JumpNormalGuide : MonoBehaviour
{
    [SerializeField] private float _length = 5.0f;
    [SerializeField] private float _dashTiling = 1f;

    private LineRenderer _line;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();

        _line.positionCount = 2;
        _line.useWorldSpace = true;
        _line.enabled = false;

        _line.textureMode = LineTextureMode.Tile;
        _line.widthMultiplier = 0.2f;
    }

    public void Show(Vector3 start, Vector3 normal)
    {
        _line.enabled = true;

        Vector3 end = start + normal.normalized * _length;
        _line.SetPosition(0, start);
        _line.SetPosition(1, end);

        // îjê¸ÇÃñßìxí≤êÆ
        _line.material.mainTextureScale =
            new Vector2(_dashTiling, 1f);
    }

    public void Hide()
    {
        _line.enabled = false;
    }
}
