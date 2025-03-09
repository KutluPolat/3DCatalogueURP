using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFade : ScriptBase
{
    public static UIFade Instance { get; private set; }

    [SerializeField] private Image blackIMG;
    private Color opaqueColor = Color.black;
    private Color transparentColor = new Color(0f, 0f, 0f, 0f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        AddEvent(KPFramework.EventName.LoadAreaCompleted, (o) =>
        {
            if (blackIMG.gameObject.activeInHierarchy)
            {
                FadeToTransparent(1f);
            };
        });
    }

    public void FadeToBlack(float duration, UnityAction onComplete = null)
    {
        blackIMG.gameObject.SetActive(true);

        blackIMG.DOKill();
        blackIMG.color = transparentColor;

        blackIMG.DOColor(opaqueColor, duration * 0.5f)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public void FadeToTransparent(float duration, UnityAction onCompleted = null)
    {
        blackIMG.DOColor(transparentColor, duration * 0.5f).OnComplete(() =>
        {
            blackIMG.gameObject.SetActive(false);
            onCompleted?.Invoke();
        });
    }
}
