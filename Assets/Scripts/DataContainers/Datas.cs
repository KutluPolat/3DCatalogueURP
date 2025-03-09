using KPFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Data_Unit
{
    public Data_VitalProps State;
    public Data_UnitProgression Progression;
    public Data_TransformProps TransformProps;

    public Data_Unit(Data_VitalProps defState, Data_UnitProgression defProgression)
    {
        State = defState;
        Progression = defProgression;
        TransformProps = new();
    }
}

[System.Serializable]
public class Data_UnitProgression
{
    public int ExtToNextLevel => Config.GetExpToReach(Level + 1);

    public int Level;
    public int CurrEXP;

    public int BaseStat_Vitality;
    public int BaseStat_Stamina;
    public int BaseStat_Mental;

    public Data_UnitProgression(int startLevel)
    {
        Level = startLevel;
    }

    // TO-DO: Save activated skills here.
}

[System.Serializable]
public class Data_VitalProps
{
    public bool IsDead;

    public int CurrHP;
    public int CurrStamina;
}

public class Data_TransformProps
{
    public Vector3 CurrPosition;
    public Vector3 CurrRotation;
    public Vector3 CurrScale;

    public Vector3 CurrDestination;
}

[System.Serializable]
public class Data_Quest
{

}

[System.Serializable]
public class Data_Area
{
    public string FinishedKeys => finishedKeys;

    public string decisions = string.Empty;
    public string finishedKeys = string.Empty;

    private string[] cacheArrayFinishedKeys;

    public void Initialize()
    {
        decisions ??= string.Empty;
        finishedKeys ??= string.Empty;
    }

    public bool IsDecisionMade(string decisionKey)
    {
        return decisions.Contains(decisionKey);
    }

    public void OnDecisionMade(string decisionKey)
    {
        decisions += decisionKey + ";";
    }

    public void OnKeyFinished(string key)
    {
        finishedKeys += key + ";";
    }

    public bool IsKeyFinished(string key)
    {
        return finishedKeys.Contains(key);
    }

    public void AddFinishedKeys(string overridedFinishedKeys)
    {
        if (!string.IsNullOrEmpty(overridedFinishedKeys))
        {
            finishedKeys += overridedFinishedKeys;
        }
    }
}

[System.Serializable]
public class Data_Progress
{
    public string CurrAreaName => CurrentAreaName;
    public Chapter CurrChapter => CurrentChapterName;

    public SerializableDictionary<Fact, bool> FactDatas;

    [SerializeField] private Chapter CurrentChapterName;
    [SerializeField] private string CurrentAreaName;
    public string storyLoopFinishedKeys;
    public SerializableDictionary<string, Data_Area> AreaDatas = new();
    private bool isInitialized;

    public void Initialize(SO_Area soArea)
    {
        CurrentAreaName = soArea.AreaTitle;
        CurrentChapterName = soArea.Chapter;
        
        AreaDatas ??= new();
        AreaDatas.InitializeDict();
        FactDatas ??= new();
        FactDatas.InitializeDict();

        isInitialized = true;
    }

    public void ModifyData(Data_Area area)
    {
        if (!isInitialized)
        {
            DebugUtility.LogError(ErrorType.MajorError, $"{nameof(Data_Progress)} USED BEFORE INITIALIZED!");
            return;
        }

        AreaDatas.Modify(CurrentAreaName, area);
    }

    public Data_Area GetAreaData()
    {
        if (!isInitialized)
        {
            DebugUtility.LogError(ErrorType.MajorError, $"{nameof(Data_Progress)} USED BEFORE INITIALIZED!");
            return new Data_Area();
        }

        if (AreaDatas.TryGetValue(CurrentAreaName, out Data_Area data))
        {
            return data;
        }
        else
        {
            return new Data_Area();
        }
    }
}

[System.Serializable]
public class Data_Collectable
{

}
