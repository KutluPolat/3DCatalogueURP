using UnityEngine;
using KPFramework;
using System.IO;
using System;
using System.Data;

public class SaveSystem<T> : MonoBehaviour
{
    private static string GetFinalPath(string path) => Path.Combine(Application.persistentDataPath, path + ".json");

    public static void DeleteAll()
    {
        string saveFolderPath = Application.persistentDataPath;
        if (Directory.Exists(saveFolderPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
            Debug.Log("All save files have been deleted from " + saveFolderPath);
        }
        else
        {
            Debug.LogWarning("Save folder does not exist: " + saveFolderPath);
        }
    }

    public static void DeleteKey(string key)
    {
        if (HasKey(key))
        {
            File.Delete(GetFinalPath(key));
        }
    }

    public static bool HasKey(string key)
    {
        return File.Exists(GetFinalPath(key));
    }

    public static void Save(string key, T data)
    {
        // create path
        string path = GetFinalPath(key);
        // convert data to json and save to path
        File.WriteAllText(path, JsonUtility.ToJson(data));

        DebugUtility.Log("Kayit tamamlandi: " + key, DebugType.EditorOnly);
    }

    public static T Load(string key, T defaultValue)
    {
        // return if save not exist
        if (!HasKey(key))
        {
            Debug.LogWarning($"Target savefile not exist! ({key})");
            return defaultValue;
        }

        string finalKey = GetFinalPath(key);

        // get data and convert from json
        var data = JsonUtility.FromJson<T>(File.ReadAllText(finalKey));

        DebugUtility.Log("Save Loaded: " + finalKey, DebugType.EditorOnly);

        return data;
    }
}

// This part is dedicated to special save properties
public static partial class SaveSystem
{
    private static bool _isOptionsDirty = true;
    private static Data_Options _options;
    public static Data_Options Options
    {
        get
        {
            if (_isOptionsDirty)
            {
                if (SaveSystem<Data_Options>.HasKey("Options"))
                    _options = SaveSystem<Data_Options>.Load("Options", new Data_Options(Application.systemLanguage));
                else
                    _options = new Data_Options(Application.systemLanguage);

                _isOptionsDirty = false;
            }

            return _options;
        }
        set
        {
            SaveSystem<Data_Options>.Save("Options", value);
            _isOptionsDirty = true;
        }
    }

    public static Data_SaveFile CurrentSF
    {
        get
        {
            if (_currentSF == null)
            {
                if (SaveSystem<Data_SaveFile>.HasKey("SaveFile"))
                    _currentSF = SaveSystem<Data_SaveFile>.Load("SaveFile", new());
                else
                {
                    _currentSF = new();
                    SaveSaveFile();
                }
            }

            return _currentSF;
        }
    }
    private static Data_SaveFile _currentSF;

    public static Data_SaveSlot CurrentSS { get; private set; }
    public static Data_SaveEntry CurrentSE { get; private set; }

    public static Data_SaveSlot MostRecentSS { get; private set; }
    public static Data_SaveEntry MostRecentSE { get; private set; }

    public static float GameTimeOfLastLoad { get; private set; }

    public static bool IsThereAnyEmptySS
    {
        get
        {
            foreach (var slot in CurrentSF.SaveSlots)
            {
                if (!slot.IsValid)
                    return true;
            }

            return false;
        }
    }
}

// This part dedicated to Saving and Loading of progress saves.
public static partial class SaveSystem
{
    public static void OnSaveSlotSelected(Data_SaveSlot ss, Data_SaveEntry se)
    {
        CurrentSS = ss;
        CurrentSE = se;
    }

    public static void GetFirstEmptySaveSlot(out Data_SaveSlot saveSlotData, out int index)
    {
        for (int i = 0; i < CurrentSF.SaveSlots.Length; i++)
        {
            var cacheSS = GetSSData(i);
            if (!cacheSS.IsValid)
            {
                saveSlotData = cacheSS;
                index = i;
                return;
            }
        }

        saveSlotData = new();
        index = -1;
    }

    public static void UpdateMostRecentSaveDatas()
    {
        foreach (var slot in CurrentSF.SaveSlots)
        {
            if (slot.IsValid)
            {
                foreach (var entry in slot.Entries)
                {
                    if (entry.IsValid)
                    {
                        if (MostRecentSE == null || !MostRecentSE.IsValid)
                        {
                            MostRecentSE = entry;
                            MostRecentSS = slot;
                        }
                        else if (entry.WhichDateIsMostRecent(MostRecentSE, entry) == 1)
                        {
                            MostRecentSE = entry;
                            MostRecentSS = slot;
                        }
                    }
                }
            }
        }
    }

    public static Data_SaveSlot GetSSData(int index)
    {
        return CurrentSF.SaveSlots[index];
    }

    private static void SaveSaveFile()
    {
        SaveSystem<Data_SaveFile>.Save("SaveFile", CurrentSF);
    }

    public static void SetCurrentSaveDatas(Data_SaveSlot ss, Data_SaveEntry se)
    {
        CurrentSS = ss;
        CurrentSE = se;
    }

    public static void SaveGame(SaveType saveType, bool triggerSavedScreenMessage)
    {
        if (CurrentSE.ProgressData == null)
            CurrentSE.ProgressData.Initialize(GameLoop.Instance.GetAreaFirst());

        Events.InvokeEvent(EventName.UpdateSaveEntry);

        var savedSaveEntry = new Data_SaveEntry();

        savedSaveEntry = JsonUtility.FromJson<Data_SaveEntry>(JsonUtility.ToJson(CurrentSE));  // we need to create diff instance thats why!
        savedSaveEntry.OnCreated(CurrentSS);
        savedSaveEntry.OnBeforeSaving(saveType, Time.time - GameTimeOfLastLoad);

        CurrentSS.AddSaveEntry(savedSaveEntry);

        SaveSaveFile();

        UpdateMostRecentSaveDatas();

        if (triggerSavedScreenMessage)
        {
            ScreenMessage.Instance.Show(Localization.Instance.GetLocalizedString("SAVED", "saved"), 2f, 3);
        }
    }

    // DONE TO-DO: Bu prefix ayni zamanda savefile indexide tutmalý yani SS_X_QUICK_X gibi
    [Obsolete("Use " + nameof(SaveManager) + "." + nameof(SaveManager.LoadGame))]
    public static bool LoadGame(Data_SaveSlot loadedSS, Data_SaveEntry loadedSE, SO_Area nameOfOverridedArea)
    {
        var loadedData = loadedSE;

        if (!loadedData.IsValid)
        {
            DebugUtility.LogError(ErrorType.MajorError, "Loaded data is not valid!!!");
            return false;
        }

        CurrentSS = loadedSS;
        CurrentSE = JsonUtility.FromJson<Data_SaveEntry>(JsonUtility.ToJson(loadedSE)); // we need to create diff instance thats why!
        GameTimeOfLastLoad = Time.time;

        if (string.IsNullOrEmpty(CurrentSE.ProgressData.CurrAreaName))
        {
            DebugUtility.LogError(ErrorType.MajorError, "CURRENT AREA NAME SHOULD NEVER BE NOT SELECTED!!!!");
            CurrentSE.ProgressData.Initialize(GameLoop.Instance.GetAreaFirst());
        }

        if (nameOfOverridedArea == null)
        {
            GameLoop.Instance.LoadArea(CurrentSE.ProgressData.CurrAreaName, SpawnPointName.NotSelected);
        }
        else
        {
            GameLoop.Instance.LoadArea(nameOfOverridedArea.AreaTitle, SpawnPointName.NotSelected);
        }

        return true;
    }
}