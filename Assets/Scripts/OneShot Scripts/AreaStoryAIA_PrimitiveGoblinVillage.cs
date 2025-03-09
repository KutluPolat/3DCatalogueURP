using DG.Tweening;
using KPFramework;
using System.Collections;
using UnityEngine;

public class AreaStoryAIA_PrimitiveGoblinVillage : AreaStoryBase
{
    [SerializeField] private AudioClip audioStomachRumble;
    [SerializeField] private AudioClip audioPortalOpening;
    [SerializeField] private AudioClip audioPortalContinious;

    protected override void ChapterStarted()
    {
        base.ChapterStarted();
        StartCoroutine(ChapterLoop());
    }

    public override void LoadVariables(object o = null)
    {
        base.LoadVariables(o);
        Debug.Log("PrimitiveVillage variables loaded.");
    }

    public override void UpdateSaveEntry(object o = null)
    {
        base.UpdateSaveEntry(o);
        Debug.Log("PrimitiveVillage variables saved.");
    }

    protected override IEnumerator ChapterLoop()
    {
        cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);
        bool hideName = !isSheKnowRealName;

        yield return ShowSequence("seqIntro", Intro());
        IEnumerator Intro()
        {
            ShowRender("0_infrontOfBrokenDoor");
            yield return new WaitForSeconds(1f);
            yield return mainTalkBalloon.WaitForClick();

            AudioSource audioSourcePortalContinious = AudioManager.Instance.PlaySound(AudioType.SFX, AudioNames.None, audioPortalContinious, true);
            AudioManager.Instance.PlaySFX(audioPortalOpening);

            ShowRender("0_infrontofBrokenDoorPortalOpens");
            // *Return Portal Activated*
            yield return mainTalkBalloon.StartTalking("narrative1_infrontOfBrokenDoor", SplashArtPosition.Closed);


            // Time to go, I guess.
            yield return mainTalkBalloon.StartTalking("althea1_infrontOfBrokenDoor", SplashArtPosition.Closed);

            if (audioSourcePortalContinious != null)
            {
                audioSourcePortalContinious.DOFade(0f, 0.25f).OnComplete(() =>
                {
                    audioSourcePortalContinious.Stop();
                });
            }
            AudioManager.Instance.PlaySFX(audioPortalOpening);
            ShowRender("0_goesBackToForest");
            // *Screaming*
            yield return mainTalkBalloon.StartTalking("narrative2_infrontOfBrokenDoor", SplashArtPosition.Closed);
        }
        AudioManager.Instance.StopSpecificAudioAll(AudioType.Environment, AudioNames.Environment_PeacefulWoods);
        AudioManager.Instance.PlayEnvironment(AudioNames.Environment_PeacefulWoods, true);
        yield return ShowSequence("seqReturnForest", ReturnForest());
        IEnumerator ReturnForest()
        {
            // Background - Forest
            // Foreground - Althea walking looking to the portal
            ShowRender("1_portalOpens");
            yield return new WaitForSeconds(1f);
            yield return mainTalkBalloon.WaitForClick();
            // Dialogue: If the portal hadn’t reopened, I don’t know what I would’ve done. But… who opened it? I didn’t see anyone.
            yield return mainTalkBalloon.StartTalking("txt1_PortalReopened", SplashArtPosition.Closed, hideName);
            // Dialogue: Anyway, I’m glad to be back.
            yield return mainTalkBalloon.StartTalking("txt2_PortalReopened", SplashArtPosition.Closed, hideName);
        }

        yield return ShowSequence("seqHungry", Hungry());
        IEnumerator Hungry()
        {
            AudioManager.Instance.PlaySFX(audioStomachRumble);
            // Background - Forest
            // Foreground - Althea holds her stomach
            ShowRender("2_ABitHungry");
            yield return new WaitForSeconds(0.35f);
            // Dialogue Althea: I wish I had eaten something in that kitchen earlier. I’m feeling a bit hungry now.
            yield return mainTalkBalloon.StartTalking("txt1_seqHungry", SplashArtPosition.Closed, hideName);
            // Dialogue Althea: I’d better try to find something to eat.
            yield return mainTalkBalloon.StartTalking("txt2_seqHungry", SplashArtPosition.Closed, hideName);
        }

        yield return ShowSequence("seqHitToHerHead", TakeHitToHead());
        IEnumerator TakeHitToHead()
        {
            // Background - Forest, a rock bounced off althea's head
            // Foreground - Althea shocked, eyes closed
            ShowRender("3_AltheaTakesHitToHead");
            yield return ApplyRenderEffect("3_AltheaTakesHitToHead", RenderEffect.PunchPosition);
            // Dialogue Narrative: *A pebble hits to Althea's head*
            yield return mainTalkBalloon.StartTalking("narrative_seqHitToHerHead", SplashArtPosition.Closed);
            // Dialogue Althea: Wha-what was that!
            yield return mainTalkBalloon.StartTalking("althea1_seqHitToHerHead", SplashArtPosition.Closed, hideName);
        }

        yield return ShowSequence("seqGoblinKidTalks", GoblinsTalk());
        IEnumerator GoblinsTalk()
        {
            // Background - Forest, a child goblin and parent goblin stands.
            // Foreground - Child goblin is angry, parent goblin is shocked
            ShowRender("4_seqGoblinKidTalks");

            // Dialogue Buggha: Calm down, Uggha. Use violance only if it’s necessary.
            yield return mainTalkBalloon.StartTalking("buggha1_seqGoblinKidTalks", SplashArtPosition.Closed);

            // Dialogue Uggha: But grandpa, she looks like them!
            yield return mainTalkBalloon.StartTalking("uggha1_seqGoblinKidTalks", SplashArtPosition.Closed);

            // Dialogue Uggha: Did you forget what they did? It was terrible!
            yield return mainTalkBalloon.StartTalking("uggha2_seqGoblinKidTalks", SplashArtPosition.Closed);

            // Dialogue Buggha: I didn’t forget. But it seems you’ve forgotten our principles!
            yield return mainTalkBalloon.StartTalking("buggha2_seqGoblinKidTalks", SplashArtPosition.Closed);

            // Dialogue Buggha: Do good, avoid evil, work hard, and think before you act. Now, guess who didn’t follow any of those.
            yield return mainTalkBalloon.StartTalking("buggha3_seqGoblinKidTalks", SplashArtPosition.Closed);
        }

        yield return ShowSequence("seqSorryToInterrupt", SorryToInterrupt());
        IEnumerator SorryToInterrupt()
        {
            // Background - Forest
            // Foreground - Althea curious, holding her head
            ShowRender("5_SorryToInterrupt");
            // Dialogue Althea: Hey, hold on! Sorry to interrupt, but I didn’t mean to harm you!
            yield return mainTalkBalloon.StartTalking("althea1_seqSorryToInterrupt", SplashArtPosition.Closed, hideName);
            // Dialogue Althea: I’m just hungry, searching for something to eat.
            yield return mainTalkBalloon.StartTalking("althea2_seqSorryToInterrupt", SplashArtPosition.Closed, hideName);
        }

        yield return ShowSequence("seqGoblinKidAngry", GoblinsAngry());
        IEnumerator GoblinsAngry()
        {
            // Background - Forest, a child goblin and parent goblin stands.
            // Foreground - Child goblin is angry, parent goblin is shocked
            ShowRender("4_seqGoblinKidTalks");

            // Dialogue Uggha: DON'T LIE TO US!
            yield return mainTalkBalloon.StartTalking("uggha1_seqGoblinKidAngry", SplashArtPosition.Closed);

            // Dialogue Buggha: Calm down, Uggha, calm down. What my grandson means is that you’re using portals, just like them.
            yield return mainTalkBalloon.StartTalking("buggha1_seqGoblinKidAngry", SplashArtPosition.Closed);

            // Dialogue Buggha: We saw you coming out of a portal. That’s why we’re suspicious of you.
            yield return mainTalkBalloon.StartTalking("buggha2_seqGoblinKidAngry", SplashArtPosition.Closed);
        }

        yield return ShowSequence("seqIDontLie", IDontLie());
        IEnumerator IDontLie()
        {
            // Background - Forest
            // Foreground - Althea curious, holding her head
            ShowRender("5_SorryToInterrupt");

            // Dialogue Althea: I didn’t lie! I’m really hungry!
            yield return mainTalkBalloon.StartTalking("althea1_seqIDontLie", SplashArtPosition.Closed, hideName);
            // Dialogue Althea: Also, are you saying that I opened those portals???  
            yield return mainTalkBalloon.StartTalking("althea2_seqIDontLie", SplashArtPosition.Closed, hideName);

            // Dialogue Althea: *Slowly* Waiit a minutee...
            yield return mainTalkBalloon.StartTalking("althea3_seqIDontLie", SplashArtPosition.Closed, hideName);

            // Background - Forest
            // Foreground - Althea extremely happy. Her eyes is sparkling and she hes extremely happy expression.
            ShowRender("6_SorryToInterruptHappy");
            // Dialogue Althea: YOU-YOU CAN TALK?!
            yield return mainTalkBalloon.StartTalking("althea4_seqIDontLie", SplashArtPosition.Closed, hideName);
        }

        yield return OnLevelEnd("ShimmeringLake");
    }
}
