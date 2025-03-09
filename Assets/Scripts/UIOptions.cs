using KPFramework;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIOptions : UIBase
{
    public static bool IsOptionsPanelActive { get; private set; }
    [SerializeField, BoxGroup("BUTTONS")] private Button openOptions, closeOptions;
    [SerializeField, BoxGroup("OPTION ELEMENTS")] private UIOptionsElement music, sounds, voice, graphicQuality, vsync, vibrations, language, fpsLimit;


    private Data_Options options;

    private void OpenOptions()
    {
        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Options, true, false));
        LoadOptions();
        IsOptionsPanelActive = true;
    }

    private void CloseOptions()
    {
        InvokeEvent(EventName.TriggerUIGoBack);
        SaveOptions();
        GameLoop.Instance.ApplyQualitySettings();
        IsOptionsPanelActive = false;
    }

    protected override void Awake()
    {
        base.Awake();

        openOptions.onClick.AddListener(OpenOptions);
        closeOptions.onClick.AddListener(CloseOptions);

        // TO-DO: GAMELOOP AWAKE'inde tüm ayarlari optionsa gore setlemek gerekebilir
        // TO-DO: Eger quality settings gibi ayarlar otomatik kaydediliyorsa, setlemeye gerek yok arastir

        fpsLimit.OnValueChanged(AudioType.SFX, (newValue) =>
        {
            int value = Mathf.FloorToInt(newValue);
            if (value != options.TargetFrameRate)
            {
                options.TargetFrameRate = value;
                Application.targetFrameRate = value;
            }
        });
        vsync.OnValueChanged(AudioType.SFX, (newValue) =>
        {
            int value = Mathf.FloorToInt(newValue);
            options.VSyncCount = value;
        });
        graphicQuality.OnValueChanged(AudioType.SFX, (newValue) =>
        {
            int value = Mathf.FloorToInt(newValue);
            if (value != options.GraphicsQuality)
            {
                options.GraphicsQuality = value;
            }
        });
        sounds.OnValueChanged(AudioType.SFX, (newValue) =>
        {
            //  * 0.1f because newValue is [0, 10] but actual value should be [0f, 1f]
            options.SoundsVolume = Mathf.FloorToInt(newValue) * 0.1f;
            InvokeEvent(EventName.AudioOptionsChanged, options);
        });
        music.OnValueChanged(AudioType.Music, (newValue) =>
        {
            //  * 0.1f because newValue is [0, 10] but actual value should be [0f, 1f]
            options.MusicVolume = Mathf.FloorToInt(newValue) * 0.1f;
            InvokeEvent(EventName.AudioOptionsChanged, options);
        });
        voice.OnValueChanged(AudioType.Voice, (newValue) =>
        {
            //  * 0.1f because newValue is [0, 10] but actual value should be [0f, 1f]
            options.VoiceVolume = Mathf.FloorToInt(newValue) * 0.1f;
            InvokeEvent(EventName.AudioOptionsChanged, options);
        });
        vibrations.OnValueChanged(AudioType.SFX, (newValue) =>
        {
            options.Vibrations = Mathf.FloorToInt(newValue);
            HapticHandler.HapticStrength = Mathf.FloorToInt(newValue);
        });

        fpsLimit.UISlider.SetMax((int)Screen.currentResolution.refreshRateRatio.value);

        language.UISlider.SetMin(0);
        language.UISlider.SetMax(System.Enum.GetValues(typeof(Language)).Length - 1);
        language.OnValueChanged(AudioType.SFX, (newValue) =>
        {
            options.Language = (Language)Mathf.FloorToInt(newValue);
            SaveOptions();
            InvokeEvent(EventName.LanguageChanged);
        });
    }

    public void LoadOptions()
    {
        options = SaveSystem.Options;
        QualitySettings.vSyncCount = options.VSyncCount;

        graphicQuality.SetValue(options.GraphicsQuality);
        vsync.SetValue(options.VSyncCount);
        vibrations.SetValue(options.Vibrations);
        language.SetValue((int)options.Language);

        int applicationTargetFrameRate = Application.targetFrameRate;
        fpsLimit.SetValue(applicationTargetFrameRate == -1 ? (int)fpsLimit.UISlider.maxValue : options.TargetFrameRate);

        // * 10f because actual value is [0f, 1f] but we're showing on label [0, 10]
        sounds.SetValue(Mathf.FloorToInt(options.SoundsVolume * 10f));
        music.SetValue(Mathf.FloorToInt(options.MusicVolume * 10f));
        voice.SetValue(Mathf.FloorToInt(options.VoiceVolume * 10f));
    }

    public void SaveOptions()
    {
        SaveSystem.Options = options;
    }
}