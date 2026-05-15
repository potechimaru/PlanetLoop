using UnityEngine;
using TMPro;

public class LongJumpedCountView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _longJumpedCountText;

    //private const string Prefix = "LongJump : ";

    public void SetLongJumpedCount(int longJumpedCount, int maxJumpedCount)
    {
        if (_longJumpedCountText == null) return;
        //Debug.Log($"LongJumpedCountView.SetLongJumpedCount called with longJumpedCount: {longJumpedCount}, maxJumpedCount: {maxJumpedCount}");
        _longJumpedCountText.text = $"{longJumpedCount}/{maxJumpedCount}";
    }

}