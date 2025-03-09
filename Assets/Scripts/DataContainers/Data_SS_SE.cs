using KPFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

// Hierarchy;
// SaveFile
//  SaveSlot
//   SaveEntry

[System.Serializable]
public class Data_SaveFile
{
    public Data_SaveSlot[] SaveSlots = new Data_SaveSlot[5];

    public Data_SaveFile()
    {
        for (int i = 0; i < SaveSlots.Length; i++)
        {
            SaveSlots[i] = new Data_SaveSlot();
        }
    }
}

[System.Serializable]
public class Data_SaveSlot
{
    public int SlotIndex => slotIndex;
    public Data_SaveEntry LastEntry => Entries != null && Entries.Count > 0 ? Entries[^1] : null;

    public bool IsValid;
    public List<Data_SaveEntry> Entries;

    [SerializeField] private int slotIndex;
    [SerializeField] private int newSlotIndex;

    public void OnCreated(int index)
    {
        IsValid = true;
        slotIndex = index;
        newSlotIndex = 0;
    }

    public int GetNewSlotIndex() => newSlotIndex++;

    public void AddSaveEntry(Data_SaveEntry entryData)
    {
        if (Entries == null)
        {
            Entries = new List<Data_SaveEntry>();
        }

        Entries.Add(entryData);

        // make sure num autosave will max be 10
        Queue<Data_SaveEntry> autoSaves = new Queue<Data_SaveEntry>();
        foreach (Data_SaveEntry entry in Entries)
        {
            if (entry.SaveType == SaveType.AutoSave)
            {
                autoSaves.Enqueue(entry);
            }
        }

        while (autoSaves.Count > 10)
        {
            Entries.Remove(autoSaves.Dequeue());
        }
    }
}

[System.Serializable]
public class Data_SaveEntry
{
    public int EntryIndex => entryIndex;

    public bool IsValid;
    public SaveType SaveType;
    public float TotalGameTime; // TO-DO: Total game time hesabýný tutman lazým
    [SerializeField] private int entryIndex;

    public Data_Progress ProgressData;

    public SerializableDictionary<QuestName, Data_Quest> QuestDatas;
    public SerializableDictionary<CharacterName, Data_Unit> UnitDatas;

    /// <summary>
    /// int = Collectable Index
    /// </summary>
    public SerializableDictionary<int, Data_Collectable> CollectableDatas;

    // TO-DO: Inventory data yapman lazým. Gridler içerisinde hangi itemlarýn olduðunu falan tutacak.

    public Data_SaveEntry()
    {
        if (QuestDatas == null)
        {
            QuestDatas = new();
            QuestDatas.InitializeDict();
        }

        if (UnitDatas == null)
        {
            UnitDatas = new();
            UnitDatas.InitializeDict();
        }

        if (CollectableDatas == null)
        {
            CollectableDatas = new();
            CollectableDatas.InitializeDict();
        }
    }

    public void OnCreated(Data_SaveSlot ss)
    {
        IsValid = true;
        entryIndex = ss.GetNewSlotIndex();
    }

    public void OnBeforeSaving(SaveType saveType, float gameTimeSinceStart)
    {
        SaveType = saveType;
        TotalGameTime += gameTimeSinceStart;
        UpdateSaveTime();
    }

    [SerializeField] private int SaveYear, SaveMonth, SaveDay, SaveHour, SaveMinute, SaveSecond;

    public void UpdateSaveTime()
    {
        var dateTime = DateTime.UtcNow;

        SaveYear = dateTime.Year;
        SaveMonth = dateTime.Month;
        SaveDay = dateTime.Day;
        SaveHour = dateTime.Hour;
        SaveMinute = dateTime.Minute;
        SaveSecond = dateTime.Second;
    }
    public int WhichDateIsMostRecent(params Data_SaveEntry[] entries)
    {
        double mostRecentDate = 0.00;
        int mostRecentDateInt = -1;

        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            double date = 
                entry.SaveYear +
                (double) entry.SaveMonth / 12 +
                (double) entry.SaveDay / 365 +
                (double) entry.SaveHour / 8760 +
                (double) entry.SaveMinute / 525600 +
                (double) entry.SaveSecond / 31536000;

            if (date > mostRecentDate)
            {
                mostRecentDate = date;
                mostRecentDateInt = i;
            }
        }

        return mostRecentDateInt;
    }

    private string GetString_PlayTime()
    {
        var gametime = TimeSpan.FromSeconds(TotalGameTime);

        string localizedString;
        string localizedTimeWord = Localization.Instance.GetLocalizedString("Time", "time");
        string localizedAbbrSec = Localization.Instance.GetLocalizedString("s", "abbr_second");
        string localizedAbbrMin = Localization.Instance.GetLocalizedString("m", "abbr_minute");
        string localizedAbbrHr = Localization.Instance.GetLocalizedString("h", "abbr_hour");
        string finalString;

        int hours = Mathf.FloorToInt((float)gametime.TotalHours);
        int minutes = gametime.Minutes;
        int seconds = gametime.Seconds;

        if (gametime.Hours > 0)
        {
            localizedString = "{0}: {5}{6}:{3:D2}{4}:{1:D2}{2}";
            finalString = string.Format(localizedString, localizedTimeWord, seconds, localizedAbbrSec, minutes, localizedAbbrMin, hours, localizedAbbrHr);
        }
        else if (gametime.Minutes > 0)
        {
            localizedString = "{0}: {3:D2}{4}:{1:D2}{2}";
            finalString = string.Format(localizedString, localizedTimeWord, seconds, localizedAbbrSec, minutes, localizedAbbrMin);
        }
        else
        {
            localizedString = "{0}: {1:D2}{2}";
            finalString = string.Format(localizedString, localizedTimeWord, seconds, localizedAbbrSec);
        }

        return finalString;
    }

    private string ChapterName()
    {
        return ProgressData.CurrChapter.ToString();
    }

    private string SaveName()
    {
        return $"{SaveType}_{EntryIndex}";
    }

    public string GetSSString()
    {
        return $"{ChapterName()} | {GetString_PlayTime()}";
    }

    public string GetSEString()
    {
        return $"{SaveName()} | {ChapterName()} | {GetString_PlayTime()}";
    }
}