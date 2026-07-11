using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private const string Prefix = "ÉXÉRÉA : ";


    public void SetScore(int score)
    {
        if (_scoreText == null) return;
        _scoreText.text = $"{Prefix}{score}";
    }
}