using UnityEngine;
using TMPro;

/// <summary>
/// 一番ベースとなるスコア表示のUIのViewクラス。
/// </summary>
public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private const string Prefix = "スコア : ";


    public void SetScore(int score)
    {
        if (_scoreText == null) return;
        _scoreText.text = $"{Prefix}{score}";
    }
}