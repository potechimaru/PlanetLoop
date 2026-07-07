using UnityEngine;
using TMPro;

public interface IDefeatEnemyCountView : IHUDCountView { }

/// <summary>
/// 倒した敵の数を表示するViewクラス。
/// </summary>
public class DefeatEnemyCountView : MonoBehaviour, IDefeatEnemyCountView
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetCount(int current, int max)
    {
        if (_text == null) return;
        _text.text = $"{current}";
    }
}