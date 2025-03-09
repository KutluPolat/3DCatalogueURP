using DG.Tweening;
using KPFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame_TapHold : MiniGame
{
    [SerializeField] private Button holdButton;
    [SerializeField] private DOTweenAnimation doAnimbreathe;
    private bool isTouchDown = false;
    private bool isHoldDown = false;
    private float timeHold = 0f;
    private System.Action onClicked;

    public void OnPointerDown()
    {
        if (doAnimbreathe != null && doAnimbreathe.isActive)
        {
            doAnimbreathe.DOPause();    
        }

        isTouchDown = true;
        onClicked?.Invoke();
    }

    public void OnPointerUp()
    {
        isTouchDown = false;
    }

    public IEnumerator StartGame(string buttonTextLocalizaitonKey, System.Action onClickedButton, float targetHoldDuration, AudioClip clip, float minPitch, float maxPitch)
    {
        if (doAnimbreathe != null)
        {
            doAnimbreathe.DOPlay();
        }

        holdButton.image.DOFade(1f, 0.1f);
        holdButton.transform.localScale = Vector3.one;
        holdButton.gameObject.SetActive(true);

        float durationDissolve = 0.5f;

        LocalizeText(holdButton.GetComponentInChildren<UIText>(), buttonTextLocalizaitonKey);

        isTouchDown = false;
        isHoldDown = false;
        timeHold = 0f;
        onClicked = onClickedButton;
        holdButton.interactable = true;

        AudioSource audioSource = null;
        float normalizedHold = 0f;

        if (clip != null)
        {
            audioSource = AudioManager.Instance.PlaySound(AudioType.SFX, AudioNames.None, clip, true, 0.75f);
        }

        while (!isHoldDown)
        {
            yield return new WaitForEndOfFrame();
            if (isTouchDown)
            {
                timeHold = Mathf.Min(targetHoldDuration, timeHold + Time.deltaTime);
            }
            else
            {
                timeHold = Mathf.Max(0f, timeHold - Time.deltaTime);
            }


            normalizedHold = timeHold / targetHoldDuration;
            holdButton.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2f, normalizedHold);


            if (audioSource != null)
            {
                audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedHold);
            }

            if (timeHold >= targetHoldDuration)
            {
                isHoldDown = true;
                holdButton.interactable = false;

                if (holdButton.TryGetComponent(out CanvasGroup cg))
                {
                    cg.DOFade(0f, durationDissolve);
                }
                else
                {
                    Debug.LogWarning("You should add canvas group to button", holdButton.gameObject);
                }

                holdButton.transform.DOScale(2f, durationDissolve);
                audioSource.Stop();
                AudioManager.Instance.PlaySFX(AudioNames.VFX_Success);
                yield return new WaitForSeconds(durationDissolve);
                yield break;
            }
        }
    }
}
