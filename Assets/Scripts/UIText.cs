using KPFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UIText : ScriptBase
{
    public TMP_Text TextMeshPro => tmp;
    public string Key { get { return key; } set { key = value; } }
    public bool AutoTranslation { get { return automaticTranslation; } set { automaticTranslation = value; } }

    [SerializeField] private bool automaticTranslation = true;
    [SerializeField, ShowIf("@automaticTranslation")] private string key;
    [SerializeField] private TMP_Text tmp;

    private void Awake()
    {
        if (tmp == null)
            DebugUtility.LogError(ErrorType.NotImplemented, "TextMeshPro component");

        if (automaticTranslation)
        {
            if (string.IsNullOrEmpty(key))
                DebugUtility.LogError(ErrorType.NotImplemented, "Localization key");

            OverrideText(Localization.Instance.GetLocalizedString(tmp.text, key));
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (automaticTranslation)
        {
            AddEvent(EventName.LanguageChanged, (o) => { OverrideText(Localization.Instance.GetLocalizedString(tmp.text, key)); });
            OverrideText(Localization.Instance.GetLocalizedString(tmp.text, key));
        }
    }

    public void SetColor(Color color)
    {
        tmp.color = color;
    }

    public void OverrideText(string text)
    {
        tmp.text = text;
    }
}
