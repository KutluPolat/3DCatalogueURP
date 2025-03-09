using KPFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkOpenerButton : MonoBehaviour
{
    [SerializeField] private string link;

    private void Awake()
    {
        if (string.IsNullOrEmpty(link))
            return;

        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(() => Application.OpenURL(link));
        }
        else
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, "link opener button");
        }
    }


#if UNITY_EDITOR
    [Button]
    private void SetTextType()
    {
        var tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.fontStyle = FontStyles.Underline;
            tmp.color = Color.cyan;
        }
    }
#endif
}
