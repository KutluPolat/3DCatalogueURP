using DG.Tweening;
using KPFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaStoryAIA_Christmas : AreaStoryBase
{
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private MiniGame_TapTap minigameJumpPortal;
    [SerializeField] private MiniGame_TapTap minigameLetMeOut;
    [SerializeField] private MiniGame_TapTap minigameLetMeOutPls;
    [SerializeField] private MiniGame_TapHold minigamePunch;
    [SerializeField] private Decision decisionGetInHouseOrLookAround;
    [SerializeField] private Decision decisionInFrontOfDoor;
    [SerializeField] private Decision decisionHall;

    [SerializeField] private AudioClip sfx_breatheInLoop;
    [SerializeField] private AudioClip sfx_thrillerMusic;
    [SerializeField] private AudioClip sfx_landingToGround;
    [SerializeField] private AudioClip sfx_knockKnockKnock;
    [SerializeField] private AudioClip sfx_alert;
    [SerializeField] private AudioClip sfx_bellRinged;
    [SerializeField] private AudioClip sfx_woodDestroyed;
    [SerializeField] private AudioClip sfx_ironDestroyed;
    [SerializeField] private AudioClip sfx_ironHitSoft;
    [SerializeField] private AudioClip sfx_ironHitHard;

    [SerializeField] private AudioClip sfx_robotExplosion;

    [SerializeField] private AudioClip sfx_portalOpening;
    [SerializeField] private AudioClip sfx_portalContinious;
    [SerializeField] private AudioClip sfx_interactedWithPortal;

    private readonly string decisionKey_GetInHouse = "getInHouse";
    private readonly string decisionKey_Snowman = "examineSnowman";

    protected override void ChapterStarted()
    {
        base.ChapterStarted();
        StartCoroutine(ChapterLoop());
    }

    public override void LoadVariables(object o = null)
    {
        base.LoadVariables(o);
        Debug.Log("Chapter2 variables loaded.");
    }

    public override void UpdateSaveEntry(object o = null)
    {
        base.UpdateSaveEntry(o);
        Debug.Log("Chapter2 variables saved.");
    }

    private IEnumerator MagicalHouseLoop()
    {
        yield return ShowSequence("getInMagicalHouse", GetInHouse());

        const string key_GuestRoom = "guestRoom";
        const string key_Kitchen = "kitchen";
        const string key_ExperimentRoom = "unknownRoom";


        while (!IsAllKeysFinished())
        {
            bool isGuestRoomDecidedButNotFinished = dataArea.IsDecisionMade(key_GuestRoom) && !dataArea.IsKeyFinished(key_GuestRoom);
            if (isGuestRoomDecidedButNotFinished)
            {
                yield return ShowSequence(key_GuestRoom, GuestRoom());
            }

            bool isExperimentRoomDecidedButNotFinished = dataArea.IsDecisionMade(key_ExperimentRoom) && !dataArea.IsKeyFinished(key_ExperimentRoom);
            if (isExperimentRoomDecidedButNotFinished)
            {
                yield return ShowSequence(key_ExperimentRoom, ExperimentRoom());
            }

            bool isKitchenDecidedButNotFinished = dataArea.IsDecisionMade(key_Kitchen) && !dataArea.IsKeyFinished(key_Kitchen);
            if (isKitchenDecidedButNotFinished)
            {
                yield return ShowSequence(key_Kitchen, Kitchen());
            }

            if (IsAllKeysFinished())
            {
                break;
            }

            yield return Hall();
        }
        cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);
        bool hideName = !isSheKnowRealName;

        ShowRender("inFrontOfBrokenDoor");
        // Dialogue Althea - I think I've explored this house completely. Nothing left to do.
        yield return mainTalkBalloon.StartTalking("houseIsCompleted", SplashArtPosition.Closed, hideName);

        bool wasThisFirstPick = dataArea.IsDecisionMade(decisionKey_GetInHouse) && !dataArea.IsDecisionMade(decisionKey_Snowman);
        if (wasThisFirstPick)
        {
            // Dialogue Althea: I should check the snowmans too. They were cute!
            yield return mainTalkBalloon.StartTalking("betterCheckTheSnowmans", SplashArtPosition.Closed, hideName);
        }

        bool IsAllKeysFinished()
        {
            return dataArea.IsKeyFinished(key_GuestRoom) && dataArea.IsKeyFinished(key_Kitchen) && dataArea.IsKeyFinished(key_ExperimentRoom);
        }

        IEnumerator GetInHouse()
        {
            // Background - side view of the house (a buzzer visible, a knocker visible)
            // Foreground - side view of the althea
            ShowRender("inFrontOfHouse");
            // Dialogue - Oh! Maybe I find someone!
            yield return mainTalkBalloon.StartTalking("maybeIFindSomeone", SplashArtPosition.Closed, true);

            bool isDoorPunched = false;
            while (!isDoorPunched)
            {
                string keyDesicionPanel = "desicionInFrontOfDoor";

                ShowRender(keyDesicionPanel, 0.15f, false);
                yield return decisionInFrontOfDoor.StartDesicion("knock", "ringTheBell", "tryOpenTheGate", "punchTheDoor");
                HideRender(keyDesicionPanel, 0f);

                switch (decisionInFrontOfDoor.FinalDecision)
                {
                    case "knock":
                        yield return Knock();
                        IEnumerator Knock()
                        {
                            AudioManager.Instance.PlaySFX(sfx_knockKnockKnock);
                            yield return new WaitForSeconds(1.5f);
                            // Dialogue Althea - Hmm. *pauses* Nothing happened.
                            yield return mainTalkBalloon.StartTalking("hmmNothingHappens", SplashArtPosition.Closed, true);
                        }
                        break;

                    case "ringTheBell":
                        yield return RingTheBell();
                        IEnumerator RingTheBell()
                        {
                            AudioManager.Instance.PlaySFX(sfx_bellRinged);
                            yield return new WaitForSeconds(1.5f);
                            // Dialogue Althea - Hmm. *pauses* Nothing happened.
                            yield return mainTalkBalloon.StartTalking("hmmNothingHappens", SplashArtPosition.Closed, true);
                        }
                        break;

                    case "tryOpenTheGate":
                        yield return TryOpenGate();
                        IEnumerator TryOpenGate()
                        {
                            // Dialogue Althea - Hmm. *pauses* It's locked.
                            yield return mainTalkBalloon.StartTalking("hmmmItsLocked", SplashArtPosition.Closed, true);
                        }
                        break;

                    case "punchTheDoor":
                        isDoorPunched = true;
                        break;

                    default:
                        DebugUtility.LogError(ErrorType.SwitchCaseNotFound, decisionGetInHouseOrLookAround.FinalDecision);
                        break;
                }
            }

            ShowRender("pullBackToThrowPunch");
            yield return new WaitForSeconds(1f);
            ShowRender("minigamePunch", 0.15f, false);
            yield return minigamePunch.StartGame("punch", null, 2f, sfx_breatheInLoop, 0.8f, 1.4f);

            AudioManager.Instance.PlaySFX(sfx_woodDestroyed);

            ShowRender("inFrontOfBrokenDoor", 0f);
            StartCoroutine(ApplyRenderEffect("inFrontOfBrokenDoor", RenderEffect.PunchPosition));
            yield return new WaitForSeconds(1f);
            // Dialogue Althea - Wow
            yield return mainTalkBalloon.StartTalking("wow", SplashArtPosition.Closed, true);
            // Dialogue Althea - Have I always been this strong?
            yield return mainTalkBalloon.StartTalking("haveIStrong", SplashArtPosition.Closed, true);
        }
        IEnumerator Hall()
        {
            bool isExperimentRoomDecidedAndFinished = dataArea.IsDecisionMade(key_ExperimentRoom) && dataArea.IsKeyFinished(key_ExperimentRoom);

            if (isExperimentRoomDecidedAndFinished)
            {
                // Background - 3 door is visible, A kitchen door, a guest room door and a mystic door (broken)
                ShowRender("hallBrokenMysticDoor");
            }
            else
            {
                // Background - 3 door is visible, A kitchen door, a guest room door and a mystic door
                ShowRender("hall");
            }

            cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);
            // Dialogue Althea - Which door do I pick?
            yield return mainTalkBalloon.StartTalking("whichDoorShouldIPick", SplashArtPosition.Closed, !isSheKnowRealName);

            List<string> selectableKeys = new List<string>();

            if (!dataArea.IsDecisionMade(key_GuestRoom))
                selectableKeys.Add(key_GuestRoom);

            if (!dataArea.IsDecisionMade(key_ExperimentRoom))
                selectableKeys.Add(key_ExperimentRoom);

            if (!dataArea.IsDecisionMade(key_Kitchen))
                selectableKeys.Add(key_Kitchen);

            string keyDecisionPanel = "decisionHall";

            ShowRender(keyDecisionPanel, 0.15f, false);
            yield return decisionHall.StartDesicion(selectableKeys.ToArray());
            HideRender(keyDecisionPanel, 0f);

            if (!string.IsNullOrEmpty(decisionHall.FinalDecision))
            {
                dataArea.OnDecisionMade(decisionHall.FinalDecision);
            }
        }

        IEnumerator GuestRoom()
        {
            cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);
            // Background - A christmas themed room with a mirror and a lot of toys
            // Foreground - althea looking around
            ShowRender("guestRoom");
            // Dialogue Narrative - *She enters the House*
            yield return mainTalkBalloon.StartTalking("altheaEntersHouse", SplashArtPosition.Closed, !isSheKnowRealName);
            // Dialogue Althea - What an amazing place!
            yield return mainTalkBalloon.StartTalking("whatAnAmazingPlace", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - The place is filled with presents. I wonder how many children lived here?
            yield return mainTalkBalloon.StartTalking("filledWithPresents", SplashArtPosition.Closed, !isSheKnowRealName);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea
            ShowRender("isThisMirrorAToy");
            // Dialogue Althea - Is this mirror also a toy?
            yield return mainTalkBalloon.StartTalking("isThisMirrorAPresent", SplashArtPosition.Closed, !isSheKnowRealName);
            float durCloseEyes = 1.5f;
            float durWaitClosed = 0.5f;
            float durOpenEyes = 1.5f;

            UIFade.Instance.FadeToBlack(0.25f);
            yield return new WaitForSeconds(0.25f);
            ShowRender("inFrontOfMirror");
            UIFade.Instance.FadeToTransparent(0.25f);

            yield return new WaitForSeconds(1f);
            // Background - POV Althea - A fancy mirror. Mirror reflects althea, vignette 25%
            ShowRender("closeEyes25p", 0f);
            yield return new WaitForSeconds(durCloseEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea, vignette 50%
            ShowRender("closeEyes50p", 0f);
            yield return new WaitForSeconds(durCloseEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea, vignette 75%
            ShowRender("closeEyes75p", 0f);
            yield return new WaitForSeconds(durCloseEyes * 0.333f);

            // Background - black screen
            ShowRender("closeEyes100p", 0f);
            yield return new WaitForSeconds(durWaitClosed);

            // Background - POV Althea - A fancy mirror. Mirror reflects a monster, vignette 75%
            ShowRender("openEyes25p", 0f);
            yield return new WaitForSeconds(durOpenEyes * 0.333f);

            AudioManager.Instance.PlayMusic(sfx_thrillerMusic, false);

            // Background - POV Althea - A fancy mirror. Mirror reflects a monster, vignette 50%
            ShowRender("openEyes50p", 0f);
            yield return new WaitForSeconds(durOpenEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects a monster, vignette 25%
            ShowRender("openEyes75p", 0f);
            yield return new WaitForSeconds(durOpenEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects a monster
            ShowRender("inFrontOfShowsAnEldavoid");
            StartCoroutine(ApplyRenderEffect("inFrontOfShowsAnEldavoid", RenderEffect.PunchPosition));
            yield return new WaitForSeconds(2f);

            // Dialogue Althea - Who is this monster?
            yield return mainTalkBalloon.StartTalking("whoIsThisMonster", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - *Scared* Am I a monster?
            yield return mainTalkBalloon.StartTalking("amIAMonster", SplashArtPosition.Closed, !isSheKnowRealName);


            // Background - POV Althea - A fancy mirror. Mirror reflects a monster, vignette 25%
            ShowRender("openEyes75p", 0f);
            yield return new WaitForSeconds(durOpenEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects a monster, vignette 50%
            ShowRender("openEyes50p", 0f);
            yield return new WaitForSeconds(durOpenEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects a monster, vignette 75%
            ShowRender("openEyes25p", 0f);
            yield return new WaitForSeconds(durOpenEyes * 0.333f);

            // Background - black screen
            ShowRender("closeEyes100p", 0f);
            yield return new WaitForSeconds(durWaitClosed);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea, vignette 75%
            ShowRender("closeEyes75p", 0f);
            yield return new WaitForSeconds(durCloseEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea, vignette 50%
            ShowRender("closeEyes50p", 0f);
            yield return new WaitForSeconds(durCloseEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea, vignette 25%
            ShowRender("closeEyes25p", 0f);
            yield return new WaitForSeconds(durCloseEyes * 0.333f);

            // Background - POV Althea - A fancy mirror. Mirror reflects althea
            ShowRender("inFrontOfMirror");
            yield return new WaitForSeconds(0.5f);
            yield return ApplyRenderEffect(string.Empty, RenderEffect.Particle_Heal);

            // Dialogue Althea - Of course I am not a monster! What was I thinking? 
            yield return mainTalkBalloon.StartTalking("ofCourseImNotAMonster", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - This magical place must be the reason for it.
            yield return mainTalkBalloon.StartTalking("magicalPlaceIsReason", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - Anyway, I guess there’s nothing left to do in this room.
            yield return mainTalkBalloon.StartTalking("nothingLeftInThisRoom", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - I’d better head back to the hallway.
            yield return mainTalkBalloon.StartTalking("backToTheHallway", SplashArtPosition.Closed, !isSheKnowRealName);
        }

        IEnumerator Kitchen()
        {
            cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);
            // background - perfectly clean christmas themed kitchen. A lot of fresh food. They are cold, but extremely fresh.
            // foreground - althea is looking around.
            ShowRender("kitchen", 0.5f);
            yield return new WaitForSeconds(0.75f);

            // Dialogue Althea - This kitchen is ridiculously clean!
            yield return mainTalkBalloon.StartTalking("kitchenSoClean", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - Look at all this food! There's so much, and it's all so fresh!
            yield return mainTalkBalloon.StartTalking("aLotOfFood", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - But something's off. Everything looks so fresh and perfect, yet it's all ice-cold. How long have these meals been here?
            yield return mainTalkBalloon.StartTalking("somethingWrongFood", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - Anyway, there’s not much to do in the kitchen, I guess.
            yield return mainTalkBalloon.StartTalking("nothingToDoKitchen", SplashArtPosition.Closed, !isSheKnowRealName);

            // Dialogue Althea - I’d better head back to the hallway.
            yield return mainTalkBalloon.StartTalking("backToTheHallway", SplashArtPosition.Closed, !isSheKnowRealName);
        }

        IEnumerator ExperimentRoom()
        {
            // Background - pov althea - This room looks like an old portal room.
            ShowRender("expRoom_SomethingBad", 1.5f);
            yield return new WaitForSeconds(2f);

            // Dialogue Althea - I think something bad happened here.
            yield return mainTalkBalloon.StartTalking("expRoom_SomethingBad", SplashArtPosition.Closed, true);
            // Dialogue Althea - What is this feeling, excitement?
            yield return mainTalkBalloon.StartTalking("expRoom_WhatFeeling", SplashArtPosition.Closed, true);

            ShowRender("expRoom_ItsNotExciting");
            // Dialogue Althea - No, it’s not excitement. I feel… bad.
            yield return mainTalkBalloon.StartTalking("expRoom_ItsNotExciting", SplashArtPosition.Closed, true);
            ShowRender("expRoom_IsItFear");
            // Dialogue Althea - Wait, could this feeling be… fear?
            yield return mainTalkBalloon.StartTalking("expRoom_IsItFear", SplashArtPosition.Closed, true);

            // Background - Same room but this time camera is focused to open door.
            // Foreground - Althea turned towards door. Walking, scared
            ShowRender("experimentTurnedBack");
            // Dialogue Althea - I need to get out of here, now!
            yield return mainTalkBalloon.StartTalking("expRoom_NeedToGetOut", SplashArtPosition.Closed, true);

            AudioManager.Instance.PlaySFX(sfx_alert);
            // Background - Same room, doors shut immediately.
            // Foreground - Althea shocked
            ShowRender("experimentAmbush");
            // Dialogue Althea - DANGER! Experiment #0003 (Code Name: ALTHEA) is attempting to escape!
            yield return mainTalkBalloon.StartTalking("expRoom_SubjectIsEscaping", SplashArtPosition.Closed, true);

            // Dialogue Althea - Althea? Who is Althea? Is it my name?
            yield return mainTalkBalloon.StartTalking("expRoom_WhoIsAlthea", SplashArtPosition.Closed, true);

            cacheProgressData.FactDatas.Modify(Fact.RealName, true);
            cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);

            // Dialogue Althea - What? What’s happening?!
            yield return mainTalkBalloon.StartTalking("expRoom_WhatWhatIsHappening", SplashArtPosition.Closed, !isSheKnowRealName);

            ShowRender("experimentLetMeOut");
            // Dialogue Althea - *scared* Let me out!
            yield return mainTalkBalloon.StartTalking("expRoom_letmeout", SplashArtPosition.Closed, !isSheKnowRealName);

            ShowRender("minigameLetMeOut", 0.15f, false);
            yield return minigameLetMeOut.StartGame("punch", sfx_ironHitSoft);

            // Dialogue Althea - *scared* *crying* Let me out! Please, let me out!
            yield return mainTalkBalloon.StartTalking("expRoom_letmeoutpls", SplashArtPosition.Closed, !isSheKnowRealName);

            ShowRender("minigameLetMeOutPls", 0.15f, false);
            yield return minigameLetMeOutPls.StartGame("punch", sfx_ironHitHard);

            yield return new WaitForSeconds(.5f);

            // Background - experiment room
            // Foreground - Althea is extremely angry, dragon ball vibe
            ShowRender("experimentISaidLetMeOut");
            // Dialogue Althea - *angry* I SAID LET-ME-OUT! 
            yield return mainTalkBalloon.StartTalking("expRoom_ISaidLETMEOUT", SplashArtPosition.Closed, !isSheKnowRealName);

            AudioManager.Instance.PlaySFX(sfx_ironDestroyed);
            StartCoroutine(ApplyRenderEffect("escapeExperimentRoom", RenderEffect.Earthquake));

            // Background - broken iron door, experiment room with red lights
            // Foreground - Althea punching
            ShowRender("escapeExperimentRoom", 0f);
            // Dialogue Narrative: *You broke the iron door.*
            yield return mainTalkBalloon.StartTalking("expRoom_youBrokeIronDoor", SplashArtPosition.Closed, !isSheKnowRealName);

            // Background - outside of experiment room, broken door visible
            // Foreground - althea relaxed
            // Dialogue Althea: That was so scary!
            yield return mainTalkBalloon.StartTalking("expRoom_thatWasScary", SplashArtPosition.Closed, !isSheKnowRealName);
            // Dialogue Althea: I thought this place was supposed to be calm and peaceful. Guess I was wrong.
            yield return mainTalkBalloon.StartTalking("expRoom_calmPeaceful", SplashArtPosition.Closed, !isSheKnowRealName);
        }
    }

    IEnumerator ExamineSnowmans()
    {
        cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out bool isNameKnown);
        bool hideName = !isNameKnown;

        // background snowman
        // foreground althea
        ShowRender("LookSnowman");

        // Dialogue Althea: They look so beautiful!
        yield return mainTalkBalloon.StartTalking("theyLookSoBeautiful", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: They almost look alive!
        yield return mainTalkBalloon.StartTalking("theyLookAlmostAlive", SplashArtPosition.Closed, hideName);
        // Dialogue Narrative: *pauses*
        yield return mainTalkBalloon.StartTalking("narrativePauses", SplashArtPosition.Closed);
        // Dialogue Althea: Wait, are they...alive?!
        yield return mainTalkBalloon.StartTalking("areTheyAlive", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: Especially this one. it looks extremely smooth.
        yield return mainTalkBalloon.StartTalking("extremelySmooth", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: It’s like it’s not even made of snow.
        yield return mainTalkBalloon.StartTalking("notEvenSnow", SplashArtPosition.Closed, hideName);

        // background snowman
        // foreground althea - reaching snowman
        ShowRender("TouchSnowman");
        // Dialogue Narrative: *touches*
        yield return mainTalkBalloon.StartTalking("touches", SplashArtPosition.Closed);

        // background snowman - red eyes mechanical mouth opened
        // foreground althea - scared away
        ShowRender("SnowmanAlert");

        // Dialogue Snowman: DANGER! DANGER! The experiment has been detected escaping.
        yield return mainTalkBalloon.StartTalking("snowmanDangerDetect", SplashArtPosition.Closed);
        // Dialogue Snowman: To prevent further escape, the self destruction protocol has been initiated.
        yield return mainTalkBalloon.StartTalking("snowmanSelfDestruct", SplashArtPosition.Closed);

        // Dialogue Althea: wait. Wait! Hold on! What do you mean self destruction?!
        yield return mainTalkBalloon.StartTalking("althWaitSelfDestruct", SplashArtPosition.Closed, hideName);

        ShowRender("SnowmanExplodes");
        // Dialogue Narrative: *The snowman explodes.*
        AudioManager.Instance.PlaySFX(sfx_robotExplosion);
        yield return mainTalkBalloon.StartTalking("narrativeSnowmanExplodes", SplashArtPosition.Closed);

        // Dialogue Narrative: *She feel as though she is about to faint. Not from pain, but from fear!*
        yield return mainTalkBalloon.StartTalking("narrativeFearFaint", SplashArtPosition.Closed);
        // Dialogue Narrative: *This is a level of fear she has never experienced before.*
        yield return mainTalkBalloon.StartTalking("narrativeFearNeverExp", SplashArtPosition.Closed);

        // Dialogue Narrative: *She is going into shock.*
        yield return mainTalkBalloon.StartTalking("narrativeGoingIntoShock", SplashArtPosition.Closed);

        // Dialogue Althea: OH NO! I’M ON FIRE!
        yield return mainTalkBalloon.StartTalking("althImOnFire", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: THIS CAN'T BE HAPPENING!
        yield return mainTalkBalloon.StartTalking("althThisCantBeHappening", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: this can't-
        yield return mainTalkBalloon.StartTalking("althThisCant", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: Wait a second. I’m on fire, but it doesn’t hurt at all. How interesting.
        yield return mainTalkBalloon.StartTalking("althItsNotHurting", SplashArtPosition.Closed, hideName);

        ShowRender("AltheaFireExtinguishes");
        // Dialogue Narrative: *The flames extinguish shortly after.*
        yield return mainTalkBalloon.StartTalking("narrativeExtinguish", SplashArtPosition.Closed);

        // Dialogue Althea: The fire didn’t hurt at all and went out quickly. I guess there was no need to be so scared.
        yield return mainTalkBalloon.StartTalking("althNoNeedToScare", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: This magical place is both protecting me and trying to harm me. So strange.
        yield return mainTalkBalloon.StartTalking("althStrangeMagicalPlace", SplashArtPosition.Closed, hideName);
        // Dialogue Althea: Anyway, it’s best not to mess with the other snowmen.
        yield return mainTalkBalloon.StartTalking("althNotToMessOthers", SplashArtPosition.Closed, hideName);

        bool wasThisFirstPick = dataArea.IsDecisionMade(decisionKey_Snowman) && !dataArea.IsDecisionMade(decisionKey_GetInHouse);
        if (wasThisFirstPick)
        {
            // Dialogue Althea: I should check the house too.
            yield return mainTalkBalloon.StartTalking("betterCheckTheHouse", SplashArtPosition.Closed, hideName);
        }
    }

    protected override IEnumerator ChapterLoop()
    {
        AudioManager.Instance.PlayEnvironment(AudioNames.Environment_PeacefulWoods, true);
        yield return ShowSequence("seqWalkingBored", WalkingBored());
        IEnumerator WalkingBored()
        {
            // Background - Forest
            // Foreground - Althea walking, bored
            ShowRender("walkingBored");
            yield return new WaitForSeconds(1f);
            yield return mainTalkBalloon.WaitForClick();
        }

        yield return ShowSequence("seqWalkingBored2", WalkingBored2());
        IEnumerator WalkingBored2()
        {
            ShowRenderInstantIfDeactive("walkingBored");
            // Dialogue Althea - Hmm. I’m so bored right now.
            yield return mainTalkBalloon.StartTalking("imReallyBored", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqWalkingBored3", WalkingBored3());
        IEnumerator WalkingBored3()
        {
            ShowRender("walkingBored2");
            // Dialogue Althea - How long should i walk before something happens?
            yield return mainTalkBalloon.StartTalking("howLongShouldIWalk", SplashArtPosition.Closed, true);
        }

        AudioSource audioSourcePortalContinious = null;
        void PortalContiniousSound_Start()
        {
            if (audioSourcePortalContinious == null)
            {
                AudioManager.Instance.PlaySFX(sfx_portalOpening);
                audioSourcePortalContinious = AudioManager.Instance.PlaySound(AudioType.SFX, AudioNames.None, sfx_portalContinious, true, 1f, 0.5f);
            }
        }

        void PortalContiniousSound_Increase(float duration)
        {
            if (audioSourcePortalContinious != null)
            {
                audioSourcePortalContinious.DOFade(1f, duration);
            }
        }

        void PortalContiniousSound_Stop()
        {
            if (audioSourcePortalContinious != null)
            {
                audioSourcePortalContinious.DOFade(0f, 0.25f).OnComplete(() =>
                {
                    audioSourcePortalContinious.Stop();
                });
            }
        }

        yield return ShowSequence("seqPortalOpens1", PortalOpens1());
        IEnumerator PortalOpens1()
        {
            // Background - Forest, a portal pops up, opened to another dimension.
            // Foreground - Althea surprised and exited.
            // Animation - Portal will suddenly appear and scales up (ANOTHER IMAGE!!!! DOSCALE)

            PortalContiniousSound_Start();
            ShowRender("portalOpens");
            yield return new WaitForSeconds(2f);
            // Dialogue Narrative - *A portal opens to another dimension*
            yield return mainTalkBalloon.StartTalking("portalOpens", SplashArtPosition.Closed, true);
            // Dialogue Althea - What is this? A picture?
            yield return mainTalkBalloon.StartTalking("isThatAPicture", SplashArtPosition.Closed, true);
            // Dialogue Althea - No wait! I know! This is not a picture!
            yield return mainTalkBalloon.StartTalking("noWaitIKnow", SplashArtPosition.Closed, true);

            // Background - Forest
            // Foreground - Althea excited & portal glows to her face.
            ShowRender("iKnowItsMagicalPic");
            // Dialogue Althea - *Slowly* It's a magical pictureee!
            yield return mainTalkBalloon.StartTalking("itsMagicalPicture", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqPortalOpens2", PortalOpens2());
        IEnumerator PortalOpens2()
        {
            // Background - Forest
            // Midground - a portal, opened to another dimension. glows to althea's face
            // Foreground - Althea has a silly and happy face

            PortalContiniousSound_Start();
            PortalContiniousSound_Increase(0.2f);

            ShowRender("closerToPortal");
            // Dialogue Narrative - *Get's closer to portal*
            yield return mainTalkBalloon.StartTalking("closerToPortal", SplashArtPosition.Closed, true);
            // Dialogue Althea - *happy* I was definitely right. It's definitely a magical picture!
            yield return mainTalkBalloon.StartTalking("itsDefinitelyMagical", SplashArtPosition.Closed, true);
            // Dialogue Narrative - *She is definitely wrong*
            yield return mainTalkBalloon.StartTalking("sheIsDefinitelyWrong", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqPortalOpens3", PortalOpens3());
        IEnumerator PortalOpens3()
        {
            // Deep Background - Forest
            // Midground & Focus - a portal, opened to another dimension. glows to althea's face
            // Foreground - Althea has a curious face

            PortalContiniousSound_Start();
            PortalContiniousSound_Increase(0.2f);

            ShowRenderInstantIfDeactive("closerToPortal");
            // Dialogue Althea - It looks so real!
            yield return mainTalkBalloon.StartTalking("looksReal", SplashArtPosition.Closed, true);
            // Dialogue Althea - Wait, something moving! Is it real?
            yield return mainTalkBalloon.StartTalking("itIsReal", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqPortalOpens4", PortalOpens4());
        IEnumerator PortalOpens4()
        {
            // Deep Background - Forest
            // Midground & Focus - a portal, opened to another dimension. glows to althea's face

            /* Foreground - Althea has a surprised expression. She is reaching in portal and
             * half of her hand in portal
             */

            PortalContiniousSound_Start();
            PortalContiniousSound_Increase(0.2f);
            AudioManager.Instance.PlaySFX(sfx_interactedWithPortal, 1.2f);

            ShowRender("reachingPortal");
            yield return mainTalkBalloon.StartTalking("wow", SplashArtPosition.Closed, true);
            ShowRender("closerToPortal");
            yield return new WaitForSeconds(0.25f);
            ShowRender("minigameJumpPortal", 0.15f, false);
            yield return minigameJumpPortal.StartGame("jump");
        }

        yield return ShowSequence("seqJumpToPortal", JumpToPortal());
        IEnumerator JumpToPortal()
        {
            PortalContiniousSound_Stop();
            AudioManager.Instance.PlaySFX(sfx_portalOpening, 1.5f);

            // Background - Blurry lights as if we're hyperjumping.
            // Foreground - Althea scared and falling.
            ShowRender("jumpingToPortal");
            // Dialogue Narrative - *Screaming*
            yield return mainTalkBalloon.StartTalking("screaming", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqLanding", Landing());
        IEnumerator Landing()
        {
            AudioManager.Instance.PlaySFX(sfx_landingToGround, 1f);

            // Background - snowy place
            // Foreground - Althea landed, there is some crackings in the ground
            ShowRender("landed");
            yield return new WaitForSeconds(0.75f);
            // Dialogue Narrative - *landed*
            yield return mainTalkBalloon.StartTalking("landed", SplashArtPosition.Closed, true);
            // Dialogue Althea - Wow! I fell so fast, but it didn’t even hurt!
            yield return mainTalkBalloon.StartTalking("landedSoFast", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqLanding2", WhatIsThisPlace());
        IEnumerator WhatIsThisPlace()
        {
            // Background - snowy place
            // Foreground - Althea looking around
            ShowRender("landedLookingAround");

            // Dialogue - *Looking Around* What is this place?
            yield return mainTalkBalloon.StartTalking("whatIsThisPlace", SplashArtPosition.Closed, true);
            // Dialogue - It's snowy but i'm not cold. I wonder why?
            yield return mainTalkBalloon.StartTalking("snowyNotCold", SplashArtPosition.Closed, true);
        }

        yield return ShowSequence("seqDecisionHouse", WhatShouldIDoNext());
        IEnumerator WhatShouldIDoNext()
        {
            cacheProgressData.FactDatas.TryGetValue(Fact.RealName, out var isSheKnowRealName);
            bool hideName = !isSheKnowRealName;
            // Background - there is house in right, and another place in left
            // Foreground - althea in middle.

            ShowRender("whatShouldIDo");
            // Dialogue Althea - Hmm what should i do?
            yield return mainTalkBalloon.StartTalking("hmmWhatShouldIDoNext", SplashArtPosition.Closed, hideName);

            // choice => Get in house or examine strange stone.
            string keyDecisionPanel = "decisionGetInHouseOrLookAround";

            ShowRender(keyDecisionPanel, 0.15f, false);
            yield return decisionGetInHouseOrLookAround.StartDesicion(decisionKey_GetInHouse, decisionKey_Snowman);
            HideRender(keyDecisionPanel, 0f);

            if (!string.IsNullOrEmpty(decisionGetInHouseOrLookAround.FinalDecision))
            {
                dataArea.OnDecisionMade(decisionGetInHouseOrLookAround.FinalDecision);
            }
        }

        if (dataArea.IsDecisionMade(decisionKey_GetInHouse))
        {
            yield return ShowSequence("seqMagicalHouseLoop", MagicalHouseLoop());
            yield return ShowSequence("seqExamineStrangeStone", ExamineSnowmans());
        }
        else
        {
            yield return ShowSequence("seqExamineStrangeStone", ExamineSnowmans());
            yield return ShowSequence("seqMagicalHouseLoop", MagicalHouseLoop());
        }

        yield return OnLevelEnd("AIA_Chapter3_PrimitiveVillage");
    }
}