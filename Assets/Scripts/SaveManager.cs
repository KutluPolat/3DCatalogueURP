using KPFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private GameObject graphicRaycasterBlocker;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void StartNewGame()
    {
        if (!SaveSystem.IsThereAnyEmptySS)
        {
            DebugUtility.LogError(ErrorType.MajorError, "YOU SHOULDN'T BE ABLE TO PRESS NEW GAME WHILE THERE IS NO EMPTY SLOT!");
            return;
        }

        SaveSystem.GetFirstEmptySaveSlot(out Data_SaveSlot ss, out int index);

        if (index != -1)
        {
            ss.OnCreated(index);

            var se = new Data_SaveEntry();
            se.ProgressData = new Data_Progress();
            se.ProgressData.Initialize(GameLoop.Instance.GetAreaFirst());
            se.OnCreated(ss);

            SaveSystem.SetCurrentSaveDatas(ss, se);

            SaveSystem.SaveGame(SaveType.AutoSave, false);
            LoadGame(SaveSystem.CurrentSS, SaveSystem.CurrentSE);
        }
    }

    public void ContinueGame()
    {
        SaveSystem.UpdateMostRecentSaveDatas();
        LoadGame(SaveSystem.MostRecentSS, SaveSystem.MostRecentSE);
    }

    public void LoadGame(Data_SaveSlot ss, Data_SaveEntry se, SO_Area overridedAreaToLoad = null)
    {
        graphicRaycasterBlocker.SetActive(true);

#pragma warning disable CS0618 // Type or member is obsolete
        if (GameLoop.Instance.IsInGame)
        {
            StartCoroutine(LocalDelay());
            IEnumerator LocalDelay()
            {
                UIFade.Instance.FadeToBlack(4.4f);
                AudioManager.Instance.PlaySFX(AudioNames.VFX_GameStart);
                yield return new WaitForSeconds(2.2f);
                SaveSystem.LoadGame(ss, se, overridedAreaToLoad);
                graphicRaycasterBlocker.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(LocalDelay());
            IEnumerator LocalDelay()
            {
                var mainMenu = FindObjectOfType<MenuLoader>();

                if (mainMenu != null)
                {
                    yield return mainMenu.PlayLoadGameAnimation();
                }
                
                SaveSystem.LoadGame(ss, se, overridedAreaToLoad);

                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();

                graphicRaycasterBlocker.SetActive(false);
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}