using UnityEngine;
using TMPro;

public class DefeatEnemyCountView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _defeatEnemyCountText;

    //private const string Prefix = "Enemy : ";

    public void SetEnemyCount(int defeatEnemyCount, int allEnemyCount)
    {
        if (_defeatEnemyCountText == null) return;
        Debug.Log($"DefeatEnemyCountView.SetEnemyCount called with defeatEnemyCount: {defeatEnemyCount}, allEnemyCount: {allEnemyCount}");
        _defeatEnemyCountText.text = $"{defeatEnemyCount}/{allEnemyCount}";
    }

}