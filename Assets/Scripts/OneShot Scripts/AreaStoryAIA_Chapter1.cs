using DG.Tweening;
using KPFramework;
using System.Collections;
using TMPro;
using UnityEngine;

public class AreaStoryAIA_Chapter1 : AreaStoryBase
{
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private MiniGame_TapTap minigameWakeUp;
    [SerializeField] private MiniGame_TapTap minigameGetUp;
    [SerializeField] private MiniGame_TapHold minigameBreatheIn, minigameBreatheOut;
    [SerializeField] private MiniGame_TapHold minigameBreatheInMoving, minigameBreatheOutMoving;
    [SerializeField] private MiniGame_Slide minigameJump;
    [SerializeField] private MiniGame_Slide minigameSlide;
    [SerializeField] private MiniGame_TapTap minigameDodge;
    [SerializeField] private AudioClip sleepy_mmmmhm;
    [SerializeField] private AudioClip onWokeUp_huh;
    [SerializeField] private AudioClip getUpLoop;
    [SerializeField] private AudioClip breatheInLoop;
    [SerializeField] private AudioClip breatheOutLoop;
    [SerializeField] private AudioClip cheesyComedy;
    [SerializeField] private AudioClip spit1, spit2;

    protected override void ChapterStarted()
    {
        base.ChapterStarted();
        StartCoroutine(ChapterLoop());
    }

    public override void LoadVariables(object o = null)
    {
        base.LoadVariables(o);
        Debug.Log("Chapter1 variables loaded.");
    }

    public override void UpdateSaveEntry(object o = null)
    {
        base.UpdateSaveEntry(o);
        Debug.Log("Chapter1 variables saved.");
    }

    protected override IEnumerator ChapterLoop()
    {
        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Dialogue, true, false));
        AudioManager.Instance.PlayEnvironment(AudioNames.Environment_PeacefulWoods, true);

        yield return ShowSequence("sleeping", Sleeping());
        IEnumerator Sleeping()
        {
            // Pose - laying on ground

            ShowRender("sleeping", 1.5f);
            yield return new WaitForSeconds(1f);
            yield return mainTalkBalloon.WaitForClick();
        }

        yield return ShowSequence("sleepy", Sleepy());
        IEnumerator Sleepy()
        {
            // Pose - laying on ground
            // Panel - Sleepy
            ShowRender("sleepy");
            AudioManager.Instance.PlayVoice(sleepy_mmmmhm);
            yield return new WaitForSeconds(1f);
        }

        yield return ShowSequence("minigameWakeUp", MiniGameWakeUp());
        IEnumerator MiniGameWakeUp()
        {
            // Pose - laying on ground
            // Panel - Mini Game Start - WAKE UP
            ShowRenderInstantIfDeactive("sleepy");
            ShowRender("minigameWakeUp", 0f, false);
            yield return minigameWakeUp.StartGame("wakeup");
        }

        yield return ShowSequence("wakeup", WakeUp());
        IEnumerator WakeUp()
        {
            // Pose - laying on ground
            // Face - :O
            // Panel - Mini Game Success (Scared wake up) "HUH?"
            ShowRender("wakeup", 0f, true);
            AudioManager.Instance.PlayVoice(onWokeUp_huh);
            yield return ApplyRenderEffect("wakeup", RenderEffect.PunchScale);
            yield return mainTalkBalloon.WaitForClick();
        }

        yield return ShowSequence("what", What());
        IEnumerator What()
        {
            // Pose - laying on ground
            // Panel - Open your eyes (Still sleepy) "What?"
            ShowRender("what");
            yield return mainTalkBalloon.StartTalking("what", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("sit", GetUp());
        IEnumerator GetUp()
        {
            ShowRenderInstantIfDeactive("what");
            // Panel - Mini Game Start - GET UP
            ShowRender("minigameGetUp", 0f, false);
            yield return minigameGetUp.StartGame("getup");
            mainTalkBalloon.Clear();

            // Pose - sitting on ground
            // Panel - Mini Game Success (Sitting)
            ShowRender("sit");
            yield return mainTalkBalloon.StartTalking("whatsHappening", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("whereAmI", WhereAmI());
        IEnumerator WhereAmI()
        {
            // Pose - sitting on ground
            // Panel - "where am i" 
            ShowRender("whereAmI");
            yield return mainTalkBalloon.StartTalking("whereAmI", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("whoAmI", WhoAmI());
        IEnumerator WhoAmI()
        {
            // Pose - sitting on ground
            // Panel - Really Scared "who am i"
            ShowRender("whoAmI");
            yield return mainTalkBalloon.StartTalking("whoAmI", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("chapter1", ChapterIntro());
        IEnumerator ChapterIntro()
        {
            ShowRenderInstantIfDeactive("whoAmI");
            ShowRender("chapter1", 0.15f, false);

            float durationIntro = 5f;
            AudioManager.Instance.StopSpecificAudioFirst(AudioType.Environment, AudioNames.Environment_PeacefulWoods, 0.5f);
            StartCoroutine(ApplyRenderEffect(string.Empty, RenderEffect.Particle_WindBlowing));
            StartCoroutine(ApplyRenderEffect(string.Empty, RenderEffect.Particle_LeavesBlowing));

            tmpTitle.alpha = 0f;
            tmpTitle.DOFade(1f, durationIntro * 0.8f);
            tmpTitle.transform.localScale = Vector3.one * 0.5f;
            tmpTitle.transform.DOScale(1.25f, durationIntro);
            AudioManager.Instance.PlayEnvironment(AudioNames.Environment_CreepyWoods, false);
            InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Unselected, true, false));
            yield return new WaitForSeconds(durationIntro);

            StartCoroutine(StopRenderEffect(RenderEffect.Particle_WindBlowing, 2f));
            StartCoroutine(StopRenderEffect(RenderEffect.Particle_LeavesBlowing, 2f));
        }

        AudioManager.Instance.PlayEnvironment(AudioNames.Environment_PeacefulWoods, true);

        yield return ShowSequence("relaxingGame", RelaxingGame());
        IEnumerator RelaxingGame()
        {
            ShowRenderInstantIfDeactive("whoAmI");
            HideRender("chapter1", 0.5f);
            yield return mainTalkBalloon.StartTalking("iNeedToCalmDown", SplashArtPosition.Closed, true);

            // Pose - sitting on ground
            // Panel - Mini Game Start - RELAX
            ShowRender("minigameBreatheIn", 0.1f, false);
            yield return minigameBreatheIn.StartGame("breatheIn", null, 1f, breatheInLoop, 0.8f, 1.2f);
            yield return ApplyRenderEffect(string.Empty, RenderEffect.Particle_Heal);

            ShowRender("minigameBreatheOut", 0.1f, false);
            yield return minigameBreatheOut.StartGame("breatheOut", null, 1f, breatheOutLoop, 1.2f, 0.8f);
            yield return ApplyRenderEffect(string.Empty, RenderEffect.Particle_Heal);

            ShowRender("minigameBreatheInMoving", 0.1f, false);
            yield return minigameBreatheInMoving.StartGame("breatheIn", null, 1f, breatheInLoop, 0.8f, 1.2f);
            yield return ApplyRenderEffect(string.Empty, RenderEffect.Particle_Heal);

            ShowRender("minigameBreatheOutMoving", 0.1f, false);
            yield return minigameBreatheOutMoving.StartGame("breatheOut", null, 1f, breatheOutLoop, 1.2f, 0.8f);
            yield return ApplyRenderEffect(string.Empty, RenderEffect.Particle_Heal);
        }

        yield return ShowSequence("relaxed", OnRelaxed());
        IEnumerator OnRelaxed()
        {
            // Pose - sitting on ground
            // Panel - Mini Game Success (relaxed)
            ShowRender("relaxed");
            yield return mainTalkBalloon.StartTalking("okayCalmDownMyself", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("allright", Allright());
        IEnumerator Allright()
        {
            // Pose - standing up half
            // Panel - allright...
            ShowRender("allright");
            yield return mainTalkBalloon.StartTalking("allright", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("timeToExplore", TimeToExplore());
        IEnumerator TimeToExplore()
        {
            // Camera - from below
            // Pose - standing
            // Panel - ...time to explore
            ShowRender("timeToExplore");
            yield return mainTalkBalloon.StartTalking("timeToExplore", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("hmmm", Hmm());
        IEnumerator Hmm()
        {
            // Pose - standing
            // Panel - hmmm
            ShowRender("hmmm");
            yield return mainTalkBalloon.StartTalking("hmmm", SplashArtPosition.Closed, true);
            yield return mainTalkBalloon.StartTalking("whatIsThisHugeThing", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("closerBigFungus", CloserToBigFungus());
        IEnumerator CloserToBigFungus()
        {
            // Pose - walking
            // Panel - *moves closer to giant strange tree.*
            ShowRender("closerBigFungus");
            yield return mainTalkBalloon.StartTalking("closerBigFungus", SplashArtPosition.Closed, true);
            ShowRender("approachBigFungus");
            yield return mainTalkBalloon.StartTalking("approachBigFungus", SplashArtPosition.Closed, true);
            ShowRender("fascinating");
            yield return mainTalkBalloon.StartTalking("fascinating", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("stepOnLittleFungus", StepOnLittleFungus());
        IEnumerator StepOnLittleFungus()
        {
            // küçük bir mantar dikkatini çeker ve tutmaya çalýþýr.
            ShowRender("hmmWhatIsThis");
            yield return mainTalkBalloon.StartTalking("hmmWhatIsThis", SplashArtPosition.Closed, true);
            // Panel - Hmm what is this? A mini mushroom? Hey mini friend! Could you possibly be the child of this monster friend?
            yield return mainTalkBalloon.StartTalking("areUTheChild", SplashArtPosition.Closed, true);
            yield return mainTalkBalloon.StartTalking("triesToGrabMushroom", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("littleMonsterCryRoar", LittleMonsterCryRoar());
        IEnumerator LittleMonsterCryRoar()
        {
            // Pose - squat sitting, looking the mini mushroom
            // Panel - (mini mushroom is scared) *cute* rrr!
            ShowRender("littleMonsterCryRoar");
            yield return mainTalkBalloon.StartTalking("littleMonsterCryRoar", SplashArtPosition.Closed, true);

            // Pose - suddenly stopped, scared
            // Panel - *mini mushroom activates a tiny gas attack!*
            ShowRender("littleMonsterAttacks");
            StartCoroutine(ApplyRenderEffect("littleMonsterAttacks", RenderEffect.PunchPosition));
            yield return mainTalkBalloon.StartTalking("littleMonsterAttacks", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("areUTheChild", AreUTheChild());
        IEnumerator AreUTheChild()
        {
            // Pose - squat sitting, looking the mini mushroom
            ShowRender("areUTheChild");
            // Althea: he he he, you are so cute!
            yield return mainTalkBalloon.StartTalking("youreCute", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("monsterRoarShort", MonsterRoarShort());
        IEnumerator MonsterRoarShort()
        {
            // Pose - squat sitting, covering her face with arms
            // Panel - *roars* RAaagh! (althea scares and wind scatters away clothes etc.)
            ShowRender("monsterRoarShort");
            StartCoroutine(ApplyRenderEffect("monsterRoarShort", RenderEffect.Earthquake));
            yield return mainTalkBalloon.StartTalking("monsterRoarShort", SplashArtPosition.Closed, true);
            ShowRender("stumbled", 0f);
            AudioManager.Instance.PlayVoice(onWokeUp_huh);
            yield return mainTalkBalloon.StartTalking("stumbled", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("sorryMonster", SorryMonster());
        IEnumerator SorryMonster()
        {
            // Pose - squat sitting, looking towards big mushroom
            // Panel - Althea: I apologize dear monster, I didn't wanted to hurt your kids. Really!
            ShowRender("sorryMonster");
            yield return mainTalkBalloon.StartTalking("sorryMonster", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("areUAngryMonster", AreUAngry());
        IEnumerator AreUAngry()
        {
            // Pose - squat sitting, looking towards big mushroom
            // Panel - Althea: Dear monster, are you mad at me?
            ShowRender("areUAngryMonster");
            yield return mainTalkBalloon.StartTalking("areUAngryMonster", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("monsterRoarLong", MonsterRoarLong());
        IEnumerator MonsterRoarLong()
        {
            // Pose - stumbled to ground
            // Panel - (Monster roars harder)
            ShowRender("monsterRoarLong");
            StartCoroutine(ApplyRenderEffect("monsterRoarLong", RenderEffect.Earthquake));
            yield return mainTalkBalloon.StartTalking("monsterRoarLong", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("iGuessThatsYes", IGuessThatsYes());
        IEnumerator IGuessThatsYes()
        {
            // Pose - stumbled to ground
            // Panel - I guess that means yes!
            ShowRender("iGuessThatsYes");
            yield return mainTalkBalloon.StartTalking("iGuessThatsYes", SplashArtPosition.Closed, true);
        }

        //AudioSource cheesyComedy = null;
        void StartCheesyComedyMusic()
        {
            //if (cheesyComedy == null)
            //{
            //    cheesyComedy = AudioManager.Instance.PlayMusic(AudioNames.Music_CheesyComedy, true);
            //}
        }

        void StopCheesyComedyMusic()
        {
            //if (cheesyComedy != null)
            //{
            //    cheesyComedy.DOFade(0f, 0.25f).OnComplete(() => cheesyComedy.Stop());
            //}
        }

        yield return ShowSequence("soSorryMonster", SoSorryMonster());
        IEnumerator SoSorryMonster()
        {
            // Pose - running while her hands over her head
            // Panel - (same image, continues to run) *running* I'm sooooooorry

            StartCheesyComedyMusic();
            ShowRender("runFromMonster");
            yield return mainTalkBalloon.StartTalking("soSorryMonster", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("minigameJumpToDodge", DodgeMonsterSpitsJump());
        IEnumerator DodgeMonsterSpitsJump()
        {
            // Pose - while running, looks back and see a spit coming
            // Panel - (Althea looks back and see a spit coming aimed her legs. !! signs on her head)
            StartCheesyComedyMusic();
            ShowRender("spitsComingFromBelow", 0.25f);
            yield return new WaitForSeconds(0.25f);
            AudioManager.Instance.PlaySFX(spit1);
            yield return new WaitForSeconds(1f);
            ShowRender("minigameJumpToDodge", 0.1f, false);
            yield return minigameJump.StartGame("jump", 0.8f, 1.2f); // @@ ADD LOCALIZATION

            // Pose - jumped
            // Panel - (spits passes under althea)

            ShowRender("spitsComingFromBelowDodged", 0f); // @@ ADD RENDER
            AudioManager.Instance.PlaySFX(AudioNames.VFX_Whoosh);
            yield return mainTalkBalloon.WaitForClick();
        }

        yield return ShowSequence("minigameSlideToDodge", DodgeMonsterSpitsSlide());
        IEnumerator DodgeMonsterSpitsSlide()
        {
            // Pose - while on air, looks back and see a spit coming
            // Panel - (Althea looks back and see a spit coming aimed to her while on air. !! signs on her head)

            StartCheesyComedyMusic();
            ShowRender("spitsComingFromTop"); // @@ ADD RENDER
            yield return new WaitForSeconds(0.25f);
            AudioManager.Instance.PlaySFX(spit2);
            yield return new WaitForSeconds(0.5f);
            ShowRender("minigameSlideToDodge", 0.1f, false);
            yield return minigameSlide.StartGame("slide", 0.8f, 1.2f); // ADD LOCALIZATON

            // Pose - slided
            // Panel - (spits passes over althea)

            ShowRender("slidedAndDodged", 0f);
            AudioManager.Instance.PlaySFX(AudioNames.VFX_SlideOnDirt); // @@ ADD SOUND
            yield return mainTalkBalloon.WaitForClick();
        }

        //yield return ShowSequence("monsterTripleSpit", DodgeMonsterSpitsShunpo());
        //IEnumerator DodgeMonsterSpitsShunpo()
        //{           
        //    // Pose - while running, looks back and see a couple spit coming
        //    // Panel - (monster will spit 3-4 acid) Mini Game Start (DODGE)

        //    StartCheesyComedyMusic();
        //    ShowRender("monsterTripleSpit"); // @@ ADD RENDER
        //    yield return new WaitForSeconds(0.25f);
        //    AudioManager.Instance.PlayVFX(spit);
        //    yield return new WaitForSeconds(0.15f);
        //    AudioManager.Instance.PlayVFX(spit);
        //    yield return new WaitForSeconds(0.15f);
        //    AudioManager.Instance.PlayVFX(spit);
        //    yield return new WaitForSeconds(0.15f);
        //    yield return minigameDodge.StartGame("dodge");

        //    // Pose - really running fast (there is 2-3 after images of her like she used shunpo)
        //    // Panel - (spits passes by althea)

        //    ShowRender("dodgedSuccess");
        //    yield return mainTalkBalloon.StartTalking("dodgedSuccess", SplashArtPosition.Closed, true);
        //}

        yield return ShowSequence("whatAreThese", WhatAreThese());
        IEnumerator WhatAreThese()
        {
            // Pose - running
            // Panel - Althea: what are these?
            ShowRender("whatAreThese");
            yield return mainTalkBalloon.StartTalking("whatAreThese", SplashArtPosition.Closed, true);
        }

        StopCheesyComedyMusic();

        yield return ShowSequence("phew", Phew());
        IEnumerator Phew()
        {
            // Pose - swiping sweat from her head. Leaned againts tree.
            // Panel - (Phew) Althea: "Phew"

            ShowRender("phew");
            yield return mainTalkBalloon.StartTalking("phew", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("needToRest", NeedToRest());
        IEnumerator NeedToRest()
        {
            // Pose - swiping sweat from her head, Leaned againts tree but lower (half way down)
            // Panel - (Stand againts tree) Althea: I'm so tired from running, need to take some rest
            ShowRender("needToRest");
            yield return mainTalkBalloon.StartTalking("needToRest", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("howLongSleep", HowLongDidISleep());
        IEnumerator HowLongDidISleep()
        {
            // Pose - leaned againts tree sitting.
            // Panel - (Sitting againts tree) Althea: *sad* I wonder how long I’ve been sleeping?
            ShowRender("howLongSleep");
            yield return mainTalkBalloon.StartTalking("howLongSleep", SplashArtPosition.Closed, true);
        }

        yield return OnLevelEnd("Christmas");
    }
}