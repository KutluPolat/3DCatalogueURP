using KPFramework;
using System.Collections;
using UnityEngine;

public partial class AreaStory_Veltenburg : AreaStoryBase
{
    protected override void ChapterStarted()
    {
        base.ChapterStarted();
        StartCoroutine(ChapterLoop());
    }

    public override void LoadVariables(object o = null)
    {
        base.LoadVariables(o);
        Debug.Log("Veltenburg variables loaded.");
    }

    public override void UpdateSaveEntry(object o = null)
    {
        base.UpdateSaveEntry(o);
        Debug.Log("Veltenburg variables saved.");
    }

    protected override IEnumerator ChapterLoop()
    {
        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Unselected, true, false));

        yield return new WaitForSeconds(1f);

        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Dialogue, true, false));
        mainTalkBalloon.InitializeSplashArts("key_0", null);

        yield return new WaitForSeconds(Config.UI_ACT_DUR);
        yield return new WaitForSeconds(Config.UI_ACT_DUR);

        yield return mainTalkBalloon.StartTalking("key_0", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_1", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_2", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_3", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_4", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_5", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_6", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_7", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_8", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_9", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_10", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_11", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_12", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_13", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_14", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_15", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_16", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_17", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_18", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_19", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_20", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_21", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_22", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_23", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_24", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_25", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_26", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_27", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_28", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_29", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_30", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_31", SplashArtPosition.Left);
        yield return mainTalkBalloon.StartTalking("key_32", SplashArtPosition.Left);
    }
}