using KPFramework;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    protected void LocalizeText(UIText uiText, string localizationKey)
    {
        if (!string.IsNullOrEmpty(localizationKey))
        {
            if (uiText == null)
            {
                DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(UIText));
                return;
            }

            uiText.Key = localizationKey;
            uiText.AutoTranslation = true;
            uiText.OverrideText(Localization.Instance.GetLocalizedString(string.Empty, localizationKey));
        }
        else
        {
            Debug.LogWarning("key (" + localizationKey + ") is null");
        }
    }
}
