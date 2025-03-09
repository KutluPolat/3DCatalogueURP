using DG.Tweening;
using UnityEngine;

public class ScreenMessage : MonoBehaviour
{
    public static ScreenMessage Instance {  get; private set; }

    [SerializeField] private CanvasGroup container;
    [SerializeField] private UIText uiText;
    private Sequence seq;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show(string text, float durationStay = 1f, int numBounce = 0)
    {
        CancelInvoke();

        container.gameObject.SetActive(true);
        container.alpha = 0f;
        container.DOFade(1f, 0.25f);

        if (seq.IsAlive())
            seq.Kill();
        uiText.gameObject.SetActive(true);
        uiText.transform.DOKill();
        uiText.transform.localScale = Vector3.zero;

        seq = DOTween.Sequence();
        seq.Append(uiText.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack, 2f));
        seq.AppendInterval(durationStay * 0.1f);

        for (int i = 0; i < numBounce; i++)
        {
            seq.Append(uiText.transform.DOPunchScale(Vector3.one * 0.1f, durationStay * 0.1f));
            seq.AppendInterval(durationStay * 0.1f);
        }

        uiText.OverrideText(text);

        Invoke(nameof(Hide), durationStay);
    }

    private void Hide()
    {
        if (seq.IsAlive())
            seq.Kill();

        uiText.transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.OutBack, 2f)
            .OnComplete(() =>
            {
                uiText.gameObject.SetActive(false);
                container.gameObject.SetActive(false);
            });
    }
}
