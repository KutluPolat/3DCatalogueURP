using KPFramework;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button Button => button;
    public UIText UIText => uiText;
    public Image Img => img;
    public AudioClip OverridedClickSound { get; set; }

    [SerializeField] private UIText uiText;
    [SerializeField] private Image img;
    [SerializeField] private Button button;
    [SerializeField] private bool useHoverIndicator;
    [SerializeField] private GameObject hoverObject;
    [SerializeField] private RectTransform leftArrow, rightArrow;
    public void ButtonPointerEnter()
    {
        if (!button.interactable)
            return;

        AudioManager.Instance.PlaySFX(AudioNames.ButtonPointerEnter);

        if (!useHoverIndicator || hoverObject == null)
            return;

        hoverObject.SetActive(true);
        var tmp = uiText.TextMeshPro;
        var textInfo = tmp.textInfo;

        int lastCharIndex = textInfo.characterCount - 1;
        Vector3 worldPositionRight = tmp.transform.TransformPoint(textInfo.characterInfo[lastCharIndex].topRight);
        Vector3 worldPositionLeft = tmp.transform.TransformPoint(textInfo.characterInfo[0].topLeft);

        SetArrowPos(worldPositionRight, rightArrow);
        SetArrowPos(worldPositionLeft, leftArrow);
    }

    public void ButtonPointerExit()
    {
        if (!useHoverIndicator || hoverObject == null)
            return;

        hoverObject.SetActive(false);
    }

    public void ButtonClicked()
    {
        if (OverridedClickSound != null)
        {
            if (button.interactable)
                AudioManager.Instance.PlaySFX(OverridedClickSound);
            else
                AudioManager.Instance.PlaySFX(OverridedClickSound, 0.5f);
        }
        else
        {
            if (button.interactable)
                AudioManager.Instance.PlaySFX(AudioNames.ButtonClicked);
            else
                AudioManager.Instance.PlaySFX(AudioNames.ButtonClicked, 0.5f);
        }
    }

    private void SetArrowPos(Vector3 worldPosition, RectTransform arrow)
    {
        Vector3 localPosition = arrow.parent.InverseTransformPoint(worldPosition);
        arrow.localPosition = new Vector3(localPosition.x, arrow.localPosition.y, arrow.localPosition.z);
    }
}
