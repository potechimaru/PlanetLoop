using UnityEngine;

public class SplinePointData : MonoBehaviour
{
    [SerializeField]
    private float baseDistance;

    public float BaseDistance
    {
        get => baseDistance;
        set => baseDistance = value;
    }
}
