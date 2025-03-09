using KPFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public struct ManualLocalizedTextProps
{
    public TMP_Text Tmp;
    public string DefaultText;
    public string Key;

    public ManualLocalizedTextProps(TMP_Text tmp, string defaultText, string key)
    {
        Tmp = tmp;
        DefaultText = defaultText;
        Key = key;
    }
}

public struct LocalizedProperties
{
    public bool IsValid => isInitialized && !string.IsNullOrEmpty(TranslatedText);
    public bool IsCharacterText { get; private set; }
    public string Key;
    public string CharacterName;
    public int CharacterID;
    public Language TranslatedLanguage;
    private string[] rawTextData;

    public string TranslatedText
    {
        get
        {
            if (!isInitialized)
                return null;

            var language = SaveSystem.Options.Language;

            if (language == TranslatedLanguage)
                return _translatedText;

            if (dictTranslation.ContainsKey(language))
            {
                _translatedText = dictTranslation[language];
                TranslatedLanguage = language;
            }
            else if (dictTranslation.ContainsKey(Language.English))
            {
                DebugUtility.LogError(ErrorType.KeyNotFound, $"Language: {language}, LocalizationKey: {Key}");
                _translatedText = dictTranslation[Language.English];
            }
            else
            {
                DebugUtility.LogError(ErrorType.KeyNotFound, $"Language: {language}, LocalizationKey: {Key}");
                return null;
            }

            return _translatedText;
        }
    }

    private string _translatedText;
    private bool isInitialized;
    private Dictionary<Language, string> dictTranslation;

    public LocalizedProperties(string key, string characterName, int characterID, string[] rawTextData)
    {
        isInitialized = true;
        Key = key;
        CharacterName = characterName;
        CharacterID = characterID;
        IsCharacterText = !string.IsNullOrEmpty(characterName) && characterID != -1;
        this.rawTextData = rawTextData;
        _translatedText = "";
        TranslatedLanguage = (Language)999999;

        dictTranslation = new Dictionary<Language, string>();
        foreach (var langIndexPair in Localization.Instance.dictLanguageIndexes)
        {
            dictTranslation.Add(langIndexPair.Key, rawTextData[langIndexPair.Value]);
        }
    }
}

public partial class Localization : ScriptBase
{
    public static Localization Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Localize(Resources.Load<TextAsset>("AlwaysLocalize"), LP_Always);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private Dictionary<string, LocalizedProperties> LP_Always = new();
    private Dictionary<string, LocalizedProperties> LP_AreaSpecific = new();
    public Dictionary<Language, int> dictLanguageIndexes = new();

    public void LocalizeArea(TextAsset textAsset)
    {
        Localize(textAsset, LP_AreaSpecific);
    }

    private void Localize(TextAsset textAsset, Dictionary<string, LocalizedProperties> dict)
    {
        var dictLines = new Dictionary<string, LocalizedProperties>();
        using (StringReader strReader = new StringReader(textAsset.text))
        {
            string firstLine = strReader.ReadLine();
            if (string.IsNullOrEmpty(firstLine))
            {
                Debug.LogError("First line is empty!");
            }
            else
            {
                string[] firstLineColumns = firstLine.Split(";");
                Dictionary<string, Language> dictLanguages = new();
                foreach (Language language in System.Enum.GetValues(typeof(Language)))
                {
                    dictLanguages.Add(language.ToString(), language);
                }

                for (int i = 0; i < firstLineColumns.Length; i++)
                {
                    string column = firstLineColumns[i];
                    if (dictLanguages.ContainsKey(column.ToString()) && !dictLanguageIndexes.ContainsKey(dictLanguages[column]))
                    {
                        dictLanguageIndexes.Add(dictLanguages[column], i);
                    }
                }
            }

            while (true)
            {
                string line = strReader.ReadLine();

                if (line == null)
                {
                    break;
                }

                var columns = line.Split(';');

                if (columns.Length > 2)
                {
                    var charID = -1;
                    if (int.TryParse(columns[0], out int parsedCharID))
                        charID = parsedCharID;

                    var charName = columns[1];
                    var key = columns[2];

                    if (!dictLines.TryAdd(key, new LocalizedProperties(key, charName, charID, columns)))
                    {
                        Debug.LogWarning("DUPLICATED KEY DETECTED! KEY: " + key);
                    }
                }
            }
        }

        dict.Clear();
        foreach (var item in dictLines)
        {
            dict.Add(item.Key, item.Value);   
        }
    }

    private LocalizedProperties SafeGetLocalizedProperties(string key)
    {
        if (LP_Always.ContainsKey(key))
        {
            return LP_Always[key];
        }
        else if (LP_AreaSpecific.ContainsKey(key))
        {
            return LP_AreaSpecific[key];
        }
        else
        {
            return new();
        }
    }

    public LocalizedProperties GetLocalizedProperties(string key)
    {
        return SafeGetLocalizedProperties(key);
    }

    public string GetLocalizedString(string defaultText, string key)
    {
        var prop = SafeGetLocalizedProperties(key);

        if (prop.IsValid)
        {
            return prop.TranslatedText;
        }
        else
        {
            DebugUtility.LogError(ErrorType.Localization, key);
            return defaultText;
        }
    }
}

public partial class Localization : ScriptBase
{
    private Dictionary<int, ManualLocalizedTextProps> dictTexts = new();
    private int limitDictTexts = 25;

    protected override void OnEnable()
    {
        base.OnEnable();
        AddEvent(EventName.LanguageChanged, LocalizeAllTexts);
    }

    private void LocalizeAllTexts(object o = null)
    {
        RemoveNullsFromDict();
        UpdateDictLimit();

        foreach (var pair in dictTexts.Values)
        {
            pair.Tmp.text = GetLocalizedString(pair.DefaultText, pair.Key);
        }
    }

    public void LocalizeText(TMP_Text txt, string defaultText, string key)
    {
        var instanceID = txt.GetInstanceID();
        var localizedProps = new ManualLocalizedTextProps(txt, defaultText, key);

        if (dictTexts.ContainsKey(instanceID))
        {
            dictTexts[instanceID] = localizedProps;
        }
        else
        {
            dictTexts.Add(instanceID, localizedProps);

            if (dictTexts.Count > limitDictTexts)
            {
                RemoveNullsFromDict();
                UpdateDictLimit();
            }
        }

        txt.text = GetLocalizedString(defaultText, key);
    }

    private void RemoveNullsFromDict()
    {
        var stackOfNulls = new Stack<int>();
        foreach (var pair in dictTexts)
        {
            if (pair.Value.Tmp == null)
            {
                stackOfNulls.Push(pair.Key);
            }
        }

        while (stackOfNulls.Count > 0)
        {
            dictTexts.Remove(stackOfNulls.Pop());
        }
    }

    private void UpdateDictLimit()
    {
        limitDictTexts = dictTexts.Count + 15;
    }
}