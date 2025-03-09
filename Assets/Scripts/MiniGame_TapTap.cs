using DG.Tweening;
using KPFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame_TapTap : MiniGame
{
    [SerializeField] private Transform parentOfButtons;
    private UIButton[] tapButtons;

    private void Awake()
    {
        tapButtons = parentOfButtons.GetComponentsInChildren<UIButton>(true);
    }

    public IEnumerator StartGame(string buttonTextLocalizaitonKey, AudioClip clip = null)
    {
        float durationDissolve = 0.5f;

        foreach (var button in tapButtons)
        {
            button.OverridedClickSound = clip;
            LocalizeText(button.GetComponentInChildren<UIText>(), buttonTextLocalizaitonKey);
        }

        foreach (var button in tapButtons)
            button.gameObject.SetActive(false);


        for (int i = 0; i < tapButtons.Length; i++)
        {
            bool isClicked = false;
            Button button = tapButtons[i].Button;
            button.interactable = true;
            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                isClicked = true;
                button.interactable = false;
                if (button.TryGetComponent(out CanvasGroup cg))
                {
                    cg.DOFade(0f, durationDissolve);
                }
                else
                {
                    Debug.LogWarning("You should add cg to buttons!", button.gameObject);
                }

                button.transform.DOScale(2f, durationDissolve).OnComplete(() => { button.gameObject.SetActive(false); });
            });

            while (!isClicked)
                yield return new WaitForEndOfFrame();

            //yield return new WaitForSeconds(durationDissolve);
        }
    }
}
