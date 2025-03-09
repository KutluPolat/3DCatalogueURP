using DG.Tweening;
using KPFramework;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public partial class GameLoop : ScriptBase
{
    public Volume MainPostProccessProfile => pppMain;
    public static GameLoop Instance { get; private set; }

    public bool IsInGame {  get; private set; }
    private bool isLoading;
    [SerializeField, BoxGroup("Post Process")] private Volume pppMain;
    [SerializeField, BoxGroup("Post Process")] private Volume[] pppAccordingToGraphicSettings;

    partial void AwakePartial()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyQualitySettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void ApplyQualitySettings()
    {    
        // TO-DO: AI ceviri yapilan dilleri belirt
        // TO-DO: Cihaz testi yapabilmenin bir yolu varsa, default graphics degerini buna gore degistirelim
        var opts = SaveSystem.Options;
        QualitySettings.SetQualityLevel(opts.GraphicsQuality);
        QualitySettings.vSyncCount = opts.VSyncCount;
        Application.targetFrameRate = opts.TargetFrameRate;

        if (opts.TargetFrameRate == (int)Screen.currentResolution.refreshRateRatio.value && QualitySettings.vSyncCount == 0)
            QualitySettings.vSyncCount = 1;

        if (opts.GraphicsQuality >= 0 && opts.GraphicsQuality < pppAccordingToGraphicSettings.Length)
        {
            pppMain.sharedProfile = pppAccordingToGraphicSettings[opts.GraphicsQuality].sharedProfile;
        }
    }

    public void LoadMainMenu()
    {
        IsInGame = false;
        throw new System.NotImplementedException();
    }

    public void LoadArea(string areaName, SpawnPointName spawnPointName)
    {
        var saveEntry = SaveSystem.CurrentSE;

        if (isLoading)
        {
            Debug.LogWarning($"Already loading a scene ({saveEntry.ProgressData.CurrAreaName}), couldn't start to load: {areaName}");
            return;
        }

        isLoading = true;

        var nextArea = GetArea(areaName);
        if (nextArea == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, "Area_" + areaName);
            return;
        }
        CurrentArea = nextArea;
        Localization.Instance.LocalizeArea(CurrentArea.LocalizationResource);
        
        LoadScene(CurrentArea.AreaTitle);
    }

    public void LoadScene(string sceneName)
    {
        DOTween.KillAll();
        AudioManager.Instance.StopAllActiveAudios();
        InvokeEvent(EventName.LoadAreaStarted);

        StartCoroutine(LocalDelay());
        IEnumerator LocalDelay()
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!asyncOp.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            // assign area script
            var areaStoryBase = GameObject.FindGameObjectWithTag("AreaStoryBase");
            if (areaStoryBase != null)
            {
                IsInGame = true;
                CurrentAreaScript = areaStoryBase.GetComponent<AreaStoryBase>();
            }
            else
            {
                IsInGame = false;
                CurrentAreaScript = null;
            }

            // finalize
            isLoading = false;
            InvokeEvent(EventName.LoadAreaCompleted);
        }
    }
}