using DG.Tweening;
using KPFramework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkBalloon : ScriptBase
{
    // TO-DO: SECIM EKLENMELI, A B C D GIBI VE BU SECIMLERDE MEVCUT ILISKILER DIKKATE ALINMALI FALAN AYRICA CLASSA OZEL VEYA STATA OZEL DIYALOG OLMALI BG3 GIBI
    [SerializeField] private SplashArtHelper splashArtHelper;
    [SerializeField] private GameObject clickToNext;
    [SerializeField] private Button buttonClickToNext;
    [SerializeField] private TextMeshProUGUI tmpComponent;
    [SerializeField] private Button button;
    private RectTransform clickToNextHand;

    private bool canGoNext;
    private bool isClicked;

    private void Awake()
    {
        splashArtHelper.Initialize();
        if (button == null)
        {
            button = GetComponentInChildren<Button>(true);
        }

        buttonClickToNext.onClick?.AddListener(() => isClicked = true);
        clickToNextHand = clickToNext.GetComponentInChildren<Animator>().transform.GetChild(0).GetComponent<RectTransform>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        clickToNextHand.DOKill();
        clickToNext.transform.DOKill();
    }

    public void InitializeSplashArts(string localizationKeyLeft, string localizationKeyRight)
        => splashArtHelper.InitializeSplashArts(localizationKeyLeft, localizationKeyRight);

    private readonly Color colorInvisible = new Color(1f, 1f, 1f, 0f);
    public IEnumerator StartTalking(string localizationKey, SplashArtPosition splashArtPosition, bool hideName = false, bool waitForClick = true, bool cleanOnComplete = true)
    {
        if (button == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, "Button");
            yield break;
        }
        button.gameObject.SetActive(true);

        var localizationProps = Localization.Instance.GetLocalizedProperties(localizationKey);
        SO_Character character;

        if (localizationProps.IsCharacterText)
        {
            character = GameLoop.Instance.GetCharacter((CharacterName)localizationProps.CharacterID);
        }
        else
        {
            character = null;
        }

#if UNITY_EDITOR

        if (character != null && character.CharacterName.ToString() != localizationProps.CharacterName)
        {
            DebugUtility.LogError(ErrorType.MajorError, "Character names doesn't add up!! Something is wrong on localization file.");
        }
#endif

        if (character != null)
        {
            splashArtHelper.OpenSplashArt(splashArtPosition, character);
        }

        canGoNext = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (TypeWriter.Instance.IsWriting)
                TypeWriter.Instance.FastForward();
            else
                canGoNext = true;
        });

        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Dialogue, true, false));
        if (character != null)
        {
            string charName = hideName ? "???" : character.CharacterName.ToString();
            TypeWriter.Instance.Write(tmpComponent, $"<color=#FF00FF>{charName}:</color> ", localizationProps.TranslatedText, character.Voice);
        }
        else
        {
            TypeWriter.Instance.Write(tmpComponent, string.Empty, localizationProps.TranslatedText, GameLoop.Instance.GetVoice(VoiceType.NONE));
        }
        

        while (TypeWriter.Instance.IsWriting)
            yield return new WaitForEndOfFrame();

        while (waitForClick && !canGoNext)
            yield return new WaitForEndOfFrame();

        button.onClick.RemoveAllListeners();

        if (cleanOnComplete)
        {
            splashArtHelper.CloseSplashArt(splashArtPosition);
            Clear();
        }

        AudioManager.Instance.OnNextPanel();
    }

    public void Clear()
    {
        button.gameObject.SetActive(false);
        TypeWriter.Instance.Clear(tmpComponent);
    }

    bool isFirstWaitForClick = true;
    public IEnumerator WaitForClick()
    {
        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Dialogue, true, false));
        float timeTrigger = Time.time;
        float waitDuration = isFirstWaitForClick ? 0f : 5f;

        clickToNext.SetActive(true);
        clickToNextHand.gameObject.SetActive(false);

        yield return new WaitUntil(() =>
        {
            if (Time.time - timeTrigger > waitDuration)
                ShowClickToNextHand();

            return isClicked;
        });

        AudioManager.Instance.OnNextPanel();
        isFirstWaitForClick = false;
        isClicked = false;
        HideClickToNext();
    }

    private void ShowClickToNextHand()
    {
        if (clickToNextHand.gameObject.activeInHierarchy)
            return;

        clickToNextHand.gameObject.SetActive(true);
        clickToNextHand.DOKill();
        clickToNextHand.anchoredPosition = 0.05f * Screen.height * Vector3.up;
        clickToNextHand.DOAnchorPos(Vector3.zero, 0.5f);
    }

    private void HideClickToNext()
    {
        clickToNext.SetActive(false);
        clickToNextHand.gameObject.SetActive(false);
    }

    [System.Serializable]
    private class SplashArtHelper
    {
        [SerializeField] private Image splashImgLeft, splashImgRight;
        private Sprite splashArtUnknown;

        public void Initialize()
        {
            splashArtUnknown = Resources.Load<Sprite>("Textures/SplashArtUnknown");
        }

        public void InitializeSplashArts(string dialogueKeyLeft, string dialogueKeyRight)
        {
            splashImgLeft.color = Color.gray;
            splashImgRight.color = Color.gray;

            if (string.IsNullOrEmpty(dialogueKeyLeft))
            {
                splashImgLeft.sprite = splashArtUnknown;
            }
            else
            {
                var localizationProps = Localization.Instance.GetLocalizedProperties(dialogueKeyLeft);
                var character = GameLoop.Instance.GetCharacter((CharacterName)localizationProps.CharacterID);

                if (character == null)
                {
                    InitializeSplashArts(null, dialogueKeyRight);
                    return;
                }

                splashImgLeft.sprite = character.SplashArt;
            }

            if (string.IsNullOrEmpty(dialogueKeyRight))
            {
                splashImgRight.sprite = splashArtUnknown;
            }
            else
            {

                var localizationProps = Localization.Instance.GetLocalizedProperties(dialogueKeyRight);
                var character = GameLoop.Instance.GetCharacter((CharacterName)localizationProps.CharacterID);

                if (character == null)
                {
                    InitializeSplashArts(dialogueKeyLeft, null);
                    return;
                }

                splashImgRight.sprite = character.SplashArt;
            }
        }

        public void OpenSplashArt(SplashArtPosition splashArtPosition, SO_Character character)
        {
            if (TryGetSplashArtImage(splashArtPosition, out Image img))
            {
                img.sprite = character.SplashArt;
                img.DOColor(Color.white, 0.5f);
                img.transform.DOScale(1.15f, 0.5f).ChangeStartValue(Vector3.one);
            }
        }

        public void CloseSplashArt(SplashArtPosition splashArtPosition)
        {
            if (TryGetSplashArtImage(splashArtPosition, out Image img))
            {
                img.DOColor(Color.gray, 0.5f);
                img.transform.DOScale(1f, 0.5f);
            }
        }

        private bool TryGetSplashArtImage(SplashArtPosition splashArtPosition, out Image img)
        {
            switch (splashArtPosition)
            {
                case SplashArtPosition.Closed:
                    // do nothing
                    img = null;
                    break;
                case SplashArtPosition.Left:
                    img = splashImgLeft;
                    break;
                case SplashArtPosition.Right:
                    img = splashImgRight;
                    break;
                default:
                    DebugUtility.LogError(ErrorType.SwitchCaseNotFound, splashArtPosition.ToString());
                    img = null;
                    break;
            }

            return img != null;
        }
    }
}