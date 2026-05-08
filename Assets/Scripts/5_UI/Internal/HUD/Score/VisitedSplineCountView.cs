using TMPro;
using UnityEngine;

public class VisitedSplineCountView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _splineCountText;

    //private const string Prefix = "Visited : ";

    public void SetSplineCount(int visitedSplineCount, int allSplineCount)
    {
        if (_splineCountText == null) return;
        //Debug.Log($"VisitedSplineCountView.SetSplineCount called with visitedSplineCount: {visitedSplineCount}, allSplineCount: {allSplineCount}");
        _splineCountText.text = $"{visitedSplineCount}/{allSplineCount}";
    }

}