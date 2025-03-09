using KPFramework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIOptionsElement : MonoBehaviour
{
    public UISlider UISlider => Slider;

    [System.Serializable]
    private enum OptionType
    {
        Other = 0,
        Language = 1,
        FPSTarget = 2,
    }

    private bool isEventsAdded;
    [SerializeField] private OptionType optionType;
    [SerializeField] private UISlider Slider;
    [SerializeField] private TextMeshProUGUI TMPSliderLabel;
    [SerializeField] private List<SerializableKeyValuePair<string, string>> Labels; //slider value, (label, labelLocalizationKey)
    private int LastValue = -1;

    public void OnValueChanged(AudioType targetAutioType, UnityAction<float> action)
    {
        if (isEventsAdded)
            return;

        isEventsAdded = true;

        var self = this; // I need this to use SetValue in a unityaction in a struct

        Slider.OnValueChanged((newValue) =>
        {
            int val = Mathf.FloorToInt(newValue);
            if (val == LastValue)
                return;

            action?.Invoke(newValue);

            if (UIOptions.IsOptionsPanelActive)
            {
                if (targetAutioType == AudioType.Music)
                    AudioManager.Instance.PlayMusic(AudioNames.Note, false);
                else if (targetAutioType == AudioType.Voice)
                    AudioManager.Instance.PlayVoice(GameLoop.Instance.GetVoice(VoiceType.NONE));
                else
                    AudioManager.Instance.PlaySFX(AudioNames.ButtonClicked);
            }

            self.SetValue(val);
        });
    }
    public void SetValue(int newValue)
    {
        Slider.SetValue(newValue);
        SerializableKeyValuePair<string, string> pair = null;

        if (Labels.Count > newValue && newValue >= 0)
        {
            pair = Labels[newValue];
        }

        if (pair != null && string.IsNullOrEmpty(pair.Key))
        {
            TMPSliderLabel.text = pair.Value;
        }
        else
        {
            switch (optionType)
            {
                case OptionType.Other:
                    Localization.Instance.LocalizeText(TMPSliderLabel, pair.Value, pair.Key);
                    break;
                case OptionType.Language:
                    TMPSliderLabel.text = $"<sprite name=\"{pair.Key}\"> " + Localization.Instance.GetLocalizedString(pair.Value, "Languages");
                    break;
                case OptionType.FPSTarget:
                    if (Slider.value == Slider.maxValue)
                    {
                        Localization.Instance.LocalizeText(TMPSliderLabel, "Unlimited", "unlimited");
                    }
                    else
                    {
                        TMPSliderLabel.text = Slider.value.ToString();
                    }

                    break;
                default:
                    DebugUtility.LogError(ErrorType.SwitchCaseNotFound, nameof(optionType));
                    break;
            }
        }

        LastValue = newValue;
    }
}
