using UnityEngine;
using TMPro;

public interface IResetConditionView : IHUDCountView { }

/// <summary>
/// 既訪問のSpline数のリセットやPointObjectのリセットを行う条件を表示するViewクラス。
/// </summary>
public class ResetConditionView : MonoBehaviour, IResetConditionView
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetCount(int current, int max)
    {
        if (_text == null) return;
        if (max == 0) return;

        int remaining = Mathf.Max(0, max - current % max);
        _text.text = $"{remaining}";
    }
}