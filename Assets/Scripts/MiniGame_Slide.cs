using DG.Tweening;
using KPFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame_Slide : MiniGame
{
    [SerializeField] private Slider slider;
    [SerializeField] private AudioClip clip;

    public IEnumerator StartGame(string buttonTextLocalizaitonKey, float minPitch, float maxPitch)
    {
        if (clip == null)
        {
            Debug.LogWarning("CLIP NOT FOUND!", gameObject);
        }

        LocalizeText(slider.GetComponentInChildren<UIText>(), buttonTextLocalizaitonKey);


        float touchDownTime = 0f;
        float currentSliderValue = 0f;
        float lastTimeSoundPlayed = 0f;
        float sliderValueNormalized = 0f;

        bool sliderFulled = false;

        slider.interactable = true;
        slider.gameObject.SetActive(true);
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((newValue) =>
        {
            sliderValueNormalized = currentSliderValue / 1f;

            if (newValue > currentSliderValue)
            {
                touchDownTime = Time.time;
            }

            if (newValue != currentSliderValue)
            {
                if (Time.time - lastTimeSoundPlayed > Config.DelayForRepeatingSounds)
                {
                    lastTimeSoundPlayed = Time.time;
                    AudioManager.Instance.PlaySFX(clip, Mathf.Lerp(minPitch, maxPitch, sliderValueNormalized));
                }
            }

            currentSliderValue = newValue;
        });


        while (!sliderFulled)
        {
            yield return new WaitForEndOfFrame();

            if (Time.time - touchDownTime > 0.1f)
            {
                slider.value -= Time.deltaTime * slider.maxValue;
            }

            if (slider.value >= 0.98f * slider.maxValue)
            {
                float durationDissolve = 0.5f;
                sliderFulled = true;
                slider.interactable = false;
                slider.image.DOFade(0f, durationDissolve);
                slider.transform.DOScale(1.25f, durationDissolve * 0.25f).OnComplete(() =>
                {
                    slider.transform.DOScale(0f, durationDissolve * 0.75f);
                });

                AudioManager.Instance.PlaySFX(AudioNames.VFX_Success);
                yield return new WaitForSeconds(durationDissolve);
                yield break;
            }
        }
    }
}
