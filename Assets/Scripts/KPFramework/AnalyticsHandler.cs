#define SDK_ACTIVE

#if SDK_ACTIVE

using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;

public class AnalyticsHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("ADD EVENT HERE");
    }

    public void TestUnityAnalytics()
    {
#if !UNITY_EDITOR
        AnalyticsResult analyticsResult = Analytics.CustomEvent("LevelLoad", new Dictionary<string, object> 
        {
            { "Level", SaveSystem.CurrentSE.ProgressData.CurrAreaName.ToString() },
            { "ChapterName", SaveSystem.CurrentSE.ProgressData.CurrChapter.ToString() } 
        });
#else
        Debug.Log("Load UA");
#endif
    }
}
#endif