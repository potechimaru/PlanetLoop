using UnityEngine;

public class LinesRotator : MonoBehaviour
{
    [SerializeField] private Transform blackHole;
    [SerializeField] private float rotationSpeed = 20f;

    void Update()
    {
        transform.RotateAround(
            blackHole.position,
            Vector3.forward,
            rotationSpeed * Time.deltaTime);
    }
}
