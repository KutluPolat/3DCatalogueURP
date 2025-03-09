using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    public static Confirmation Instance { get; private set; }
    [SerializeField] private Button buttonConfirm, buttonDeny;
    [SerializeField] private CanvasGroup cgConfirmation;
    private System.Action onConfirm, onDeny;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            buttonDeny.onClick?.AddListener(OnDenied);
            buttonConfirm.onClick?.AddListener(OnConfirmed);
            return;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Ask(System.Action onConfirmed, System.Action onDenied)
    {
        OpenConfirmation();
        onDeny = onDenied;
        onConfirm = onConfirmed;
    }

    private void OnConfirmed()
    {
        onConfirm?.Invoke();
        CloseConfirmation();
    }

    private void OnDenied()
    {
        onDeny?.Invoke();
        CloseConfirmation();
    }

    private void OpenConfirmation()
    {
        cgConfirmation.gameObject.SetActive(true);
        cgConfirmation.alpha = 0f;
        cgConfirmation.DOFade(1f, 0.25f);
        cgConfirmation.blocksRaycasts = true;
    }

    private void CloseConfirmation()
    {
        cgConfirmation.blocksRaycasts = false;
        cgConfirmation.DOFade(0f, 0.25f).OnComplete(() => cgConfirmation.gameObject.SetActive(false));
    }
}
