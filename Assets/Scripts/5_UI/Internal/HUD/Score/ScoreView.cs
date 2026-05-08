using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private const string Prefix = "Score : ";


    public void SetScore(int score)
    {
        if (_scoreText == null) return;
        //Debug.Log($"ScoreView.SetScore({score})");
        _scoreText.text = $"{Prefix}{score}";
    }
}