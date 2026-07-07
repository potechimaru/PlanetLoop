using TMPro;
using UnityEngine;

/// <summary>
/// TMPのテキストを等幅フォントにするためのクラス
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPMonospaceFixer : MonoBehaviour
{
    [SerializeField]
    private float characterWidth = 40f;

    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();

        ApplyMonospace();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_tmp == null)
            _tmp = GetComponent<TextMeshProUGUI>();

        ApplyMonospace();
    }
#endif

    public void ApplyMonospace()
    {
        if (_tmp == null) return;

        string raw = _tmp.text;

        // すでにmspace付いてる場合除去
        raw = RemoveMspace(raw);

        _tmp.text =
            $"<mspace={characterWidth}>{raw}</mspace>";
    }

    private string RemoveMspace(string text)
    {
        int start = text.IndexOf('>');
        int end = text.LastIndexOf("</mspace>");

        if (text.StartsWith("<mspace=") &&
            start >= 0 &&
            end >= 0)
        {
            return text.Substring(start + 1, end - start - 1);
        }

        return text;
    }
}