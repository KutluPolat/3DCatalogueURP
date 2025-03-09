using DG.Tweening;
using KPFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AreaStoryBase : ScriptBase, ISave
{
    protected Data_Area dataArea;

    [SerializeField] private ParticleSystem particleHeal;
    [SerializeField] private ParticleSystem particleWind;
    [SerializeField] private ParticleSystem particleLeaves;

    public enum RenderEffect
    {
        PunchScale = 0,
        Particle_Heal = 1,
        PunchPosition = 2,
        Earthquake = 3,
        Particle_WindBlowing = 4,
        Particle_LeavesBlowing = 5,
    }

    [System.Serializable]
    protected struct DialogueParams
    {
        public string key;
        public Sprite sprite;
    }

    [SerializeField] private Transform parentOfRenders;
    
    private Dictionary<string, CanvasGroup> dictRenders = new();
    protected Data_Progress cacheProgressData;
    protected TalkBalloon mainTalkBalloon;
    private Stack<CanvasGroup> oldRenders = new();


    protected virtual void Awake()
    {
        if (GameLoop.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (parentOfRenders != null)
        {
            foreach (Transform child in parentOfRenders)
            {
                if (child.TryGetComponent(out CanvasGroup cg))
                {
                    dictRenders.Add(child.name, cg);
                }
            }
        }
        else
        {
            DebugUtility.LogError(ErrorType.MajorError, "PARENT OF RENDERS IS NULL!");
        }

        mainTalkBalloon = FindFirstObjectByType<TalkBalloon>(FindObjectsInactive.Include);

        if (!CompareTag("AreaStoryBase"))
        {
            tag = "AreaStoryBase";
            DebugUtility.LogError(ErrorType.TagShouldBe, "AreaStoryBase");
        }

        LoadVariables();
        ChapterInitialized();
    }

    protected virtual void Start()
    {
        ChapterStarted();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        AddEvent(EventName.UpdateSaveEntry, UpdateSaveEntry);
    }

    protected virtual void ChapterInitialized()
    {

    }

    protected virtual void ChapterStarted()
    {

    }

    protected abstract IEnumerator ChapterLoop();

    protected virtual void ChapterFinished()
    {

    }

    public virtual void UpdateSaveEntry(object o = null)
    {
        var currSOArea = GameLoop.Instance.CurrentArea;
        cacheProgressData.Initialize(currSOArea);
        cacheProgressData.ModifyData(dataArea);

        SaveSystem.CurrentSE.ProgressData = JsonUtility.FromJson<Data_Progress>(JsonUtility.ToJson(cacheProgressData));
    }

    public virtual void LoadVariables(object o = null)
    {
        var currSOArea = GameLoop.Instance.CurrentArea;
        cacheProgressData = JsonUtility.FromJson<Data_Progress>(JsonUtility.ToJson(SaveSystem.CurrentSE.ProgressData));
        cacheProgressData.Initialize(currSOArea);

        dataArea = cacheProgressData.GetAreaData();
        dataArea.Initialize();

        bool isFirstLevel = SceneManager.GetActiveScene().buildIndex == 1;
        bool isOldSaveAvailable = !string.IsNullOrEmpty(cacheProgressData.storyLoopFinishedKeys);
        bool isNewSaveEmpty = string.IsNullOrEmpty(dataArea.FinishedKeys);

        if (isFirstLevel && isOldSaveAvailable && isNewSaveEmpty)
        {
            dataArea.AddFinishedKeys(cacheProgressData.storyLoopFinishedKeys);
            cacheProgressData.storyLoopFinishedKeys = string.Empty;
        }
    }

    public IEnumerator ApplyRenderEffect(string localizationKey, RenderEffect renderEffect)
    {
        switch (renderEffect)
        {
            case RenderEffect.PunchScale:
                var cg = GetRender(localizationKey);
                float duration = 0.2f;
                if (cg != null)
                {
                    
                    cg.transform.DOScale(1.3f, duration * 0.5f)
                        .OnComplete(() => cg.transform.DOScale(1f, duration * 0.5f));
                    yield return new WaitForSeconds(duration);
                }
                break;
            case RenderEffect.PunchPosition:
                cg = GetRender(localizationKey);
                if (cg)
                {
                    var rectTransform = cg.GetComponent<RectTransform>();

                    duration = 1f;
                    rectTransform.DOPunchAnchorPos(0.025f * Screen.width * Vector3.one, duration * 0.9f)
                        .OnComplete(() => rectTransform.DOAnchorPos(Vector3.zero, duration * 0.1f));
                    yield return new WaitForSeconds(duration);
                }
                break;
            case RenderEffect.Earthquake:
                cg = GetRender(localizationKey);
                if (cg)
                {
                    var rectTransform = cg.GetComponent<RectTransform>();

                    duration = 2f;
                    rectTransform.DOPunchAnchorPos(0.06f * Screen.width * Vector3.one, duration * 0.9f)
                        .OnComplete(() => rectTransform.DOAnchorPos(Vector3.zero, duration * 0.1f));
                    yield return new WaitForSeconds(duration);
                }
                break;
            case RenderEffect.Particle_Heal:
                AudioManager.Instance.PlaySFX(AudioNames.VFX_Success, 1.3f);
                particleHeal.gameObject.SetActive(true);
                particleHeal.Play();
                yield return new WaitForSeconds(1f);
                particleHeal.gameObject.SetActive(false);
                break;
            case RenderEffect.Particle_WindBlowing:
                particleWind.gameObject.SetActive(true);
                particleWind.Play();
                break;
            case RenderEffect.Particle_LeavesBlowing:
                particleLeaves.gameObject.SetActive(true);
                particleLeaves.Play();
                break;
            default:
                DebugUtility.LogError(ErrorType.SwitchCaseNotFound, renderEffect.ToString());
                break;
        }
    }

    public IEnumerator StopRenderEffect(RenderEffect renderEffect, float duration)
    {
        switch (renderEffect)
        {
            case RenderEffect.Particle_WindBlowing:
                particleWind.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                yield return new WaitForSeconds(duration);
                particleWind.gameObject.SetActive(false);
                break;
            case RenderEffect.Particle_LeavesBlowing:
                particleLeaves.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                yield return new WaitForSeconds(duration);
                particleLeaves.gameObject.SetActive(false);
                break;
            default:
                DebugUtility.LogError(ErrorType.SwitchCaseNotFound, renderEffect.ToString());
                break;
        }
    }

    public void ShowRenderInstantIfDeactive(string localizationKey, bool deactivateOlds = true)
    {
        var cg = GetRender(localizationKey);
        if (cg != null && !cg.gameObject.activeInHierarchy)
            ShowRender(localizationKey, 0f, deactivateOlds);
    }

    public void ShowRender(string localizationKey, float duration = 0.15f, bool deactivateOlds = true)
    {
        var cg = GetRender(localizationKey);
        if (cg != null)
        {
            cg.gameObject.SetActive(true);

            if (duration <= 0f)
            {
                cg.DOKill();
                cg.alpha = 1f;
                OnRenderActivated(cg, deactivateOlds);
            }
            else
            {
                cg.alpha = 0f;
                cg.DOFade(1f, duration).OnComplete(() => OnRenderActivated(cg, deactivateOlds));
            }
        }
    }

    public void HideRender(string localizationKey, float duration)
    {
        var cg = GetRender(localizationKey);
        if (cg != null)
        {

            if (duration <= 0f)
            {
                cg.DOKill();
                cg.gameObject.SetActive(false);
            }
            else
            {
                cg.DOFade(0f, duration).OnComplete(() => cg.gameObject.SetActive(false));
            }
        }
    }

    private CanvasGroup GetRender(string localizationKey)
    {
        if (dictRenders.TryGetValue(localizationKey, out CanvasGroup cg))
        {
            return cg;
        }
        else
        {
            DebugUtility.LogError(ErrorType.MajorError, $"RENDER ({localizationKey}) IS NOT EXIST");
            return null;
        }
    }

    public IEnumerator ShowSequence(string key, IEnumerator routine)
    {
        if (dataArea.IsKeyFinished(key))
            yield break;

        yield return routine;

        dataArea.OnKeyFinished(key);
    }

    private void OnRenderActivated(CanvasGroup newRender, bool deactivateOlds)
    {
        if (deactivateOlds)
        {
            while (oldRenders.Count > 0)
                oldRenders.Pop().gameObject.SetActive(false);
        }

        oldRenders.Push(newRender);
    }

    protected IEnumerator OnLevelEnd(string nextAreaName)
    {
        if (GameLoop.Instance == null || SaveManager.Instance == null)
        {
            Debug.LogError("Critical instance is null.");
            yield break;
        }
        
        var nextArea = GameLoop.Instance.GetArea(nextAreaName);
        if (nextArea == null)
        {
            Debug.LogWarning($"Failed to load area: {nextAreaName} not found.");
            yield return ToBeContinue();
            yield break;
        }

        SaveManager.Instance.LoadGame(SaveSystem.CurrentSS, SaveSystem.CurrentSE, nextArea);
    }

    private IEnumerator ToBeContinue()
    {
        ShowRender("toBeContinue", 0.5f, false);
        AudioManager.Instance.PlaySFX(AudioNames.Environment_CreepyWoods);
        StartCoroutine(ApplyRenderEffect(string.Empty, RenderEffect.Particle_WindBlowing));
        StartCoroutine(ApplyRenderEffect(string.Empty, RenderEffect.Particle_LeavesBlowing));

        yield return new WaitForSeconds(1f);


        StartCoroutine(StopRenderEffect(RenderEffect.Particle_WindBlowing, 6f));
        StartCoroutine(StopRenderEffect(RenderEffect.Particle_LeavesBlowing, 6f));

        yield return mainTalkBalloon.WaitForClick();
        GameLoop.Instance.LoadScene("MainMenu");
    }
}