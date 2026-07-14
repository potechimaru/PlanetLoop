using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// 設定画面の音量スライダーを制御するクラス。BGMとSEで使い分ける。
/// </summary>
public class AudioVolumeSlider : MonoBehaviour
{
    public enum VolumeType
    {
        BGM,
        SE
    }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;
    [SerializeField] private TextMeshProUGUI valueText;

    [Inject] private AudioManager _audioManager;

    private void Start()
    {
        if (slider == null || _audioManager == null)
            return;


        slider.onValueChanged.RemoveListener(OnSliderChanged);

        float value = volumeType == VolumeType.BGM
            ? _audioManager.GetBGMVolume()
            : _audioManager.GetSEVolume();

        SetValue(value);

        // リスナー登録
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        RefreshText(value);

        switch (volumeType)
        {
            case VolumeType.BGM:
                _audioManager.SetBGMVolume(value);
                break;

            case VolumeType.SE:
                _audioManager.SetSEVolume(value);
                break;
        }
    }

    public Slider Slider => slider;

    public void SetValue(float normalizedValue)
    {
        slider.SetValueWithoutNotify(normalizedValue);
        RefreshText(normalizedValue);
    }

    /// <summary>
    /// 0～99の整数値に変換してテキストを更新する
    /// </summary>
    /// <param name="normalizedValue"></param>
    public void RefreshText(float normalizedValue)
    {
        int value = Mathf.RoundToInt(normalizedValue * 99f);
        valueText.text = value.ToString();
    }
}