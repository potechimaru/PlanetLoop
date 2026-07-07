using TMPro;
using UnityEngine;

/// <summary>
/// ゲーム説明画面で「次へ」と「閉じる」を切り替えるためのクラス
/// </summary>
public class TextSwitcher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;

    [Header("Texts")]
    [SerializeField] private string firstText;  // 「次へ」
    [SerializeField] private string secondText;  // 「閉じる」

    private void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 「次へ」を表示する
    /// </summary>
    public void ShowFirst()
    {
        SetText(firstText);
    }

    /// <summary>
    /// 「閉じる」を表示する
    /// </summary>
    public void ShowSecond()
    {
        SetText(secondText);
    }

    public void Switch(bool useFirst)
    {
        SetText(useFirst ? firstText : secondText);
    }

    private void SetText(string text)
    {
        if (targetText == null)
        {
            Debug.LogError("[TextSwitcher] TextMeshProUGUI が設定されていません。", this);
            return;
        }

        targetText.text = text;
    }
}