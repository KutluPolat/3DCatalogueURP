using TMPro;
using UnityEngine;

public class DiscountLocalizer : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private int discountAmount;

    private void OnEnable()
    {
        GetComponent<TextMeshProUGUI>().text = $"{prefix} ({string.Format(Localization.Instance.GetLocalizedString("{0}% OFF", "discount"), discountAmount.ToString())})";
    }
}
