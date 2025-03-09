using KPFramework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cheats : MonoBehaviour
{
#if UNITY_EDITOR
    private static Cheats Instance;
    [SerializeField] private GameObject cheatPanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI textCheatSheet;
    [SerializeField] private RectTransform rectTransformOfContent;
    // Dictionary to hold cheat codes and their corresponding actions
    private Dictionary<string, System.Action> cheatCodes = new Dictionary<string, System.Action>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Define cheat codes and their actions
        CreateCheat("t", "instant continue game", textCheatSheet, ContinueGameInstant);
        CreateCheat("timeScale1", null, Instantiate(textCheatSheet, rectTransformOfContent), TimeScaleOne);
        CreateCheat("timescale3", null, Instantiate(textCheatSheet, rectTransformOfContent), TimeScaleThree);
        CreateCheat("timescale9", null, Instantiate(textCheatSheet, rectTransformOfContent), TimeScaleNine);

        for (int i = 0; i < GameLoop.Instance.NumArea; i++)
        {
            int index = i;
            CreateCheat($"chapter{index + 1}", null, Instantiate(textCheatSheet, rectTransformOfContent), () => SaveManager.Instance.LoadGame(SaveSystem.CurrentSS, SaveSystem.CurrentSE, GameLoop.Instance.GetAreaByIndex(index)));
        }   

        void CreateCheat(string key, string description, TextMeshProUGUI tmp, System.Action action)
        {
            tmp.text = key;

            if (!string.IsNullOrEmpty(description))
                tmp.text += " => " + description;

            cheatCodes.Add(key, action);


            // Set the height to 100 while maintaining the current left/right offset.
            float newHeight = 100 * cheatCodes.Count;

            // Adjust the offsetMin (bottom) and offsetMax (top) to change height
            rectTransformOfContent.offsetMin = new Vector2(rectTransformOfContent.offsetMin.x, 0f);
            rectTransformOfContent.offsetMax = new Vector2(rectTransformOfContent.offsetMax.x, newHeight);

        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            cheatPanel.SetActive(!cheatPanel.activeInHierarchy);
            inputField.text = string.Empty;
            inputField.Select();
        }

        if (cheatPanel.activeInHierarchy)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (cheatCodes.ContainsKey(inputField.text))
                {
                    cheatCodes[inputField.text].Invoke();
                }

                inputField.text = string.Empty;
            }
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.SaveGame(SaveType.QuickSave, true);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            SaveManager.Instance.ContinueGame();
        }
    }

    // Define your cheat code methods here
    private void TimeScaleOne()
    {
        Debug.Log("Time Scale 1!");
        Time.timeScale = 1f;
    }

    private void TimeScaleThree()
    {
        Debug.Log("Time Scale 3!");
        Time.timeScale = 3f;
    }

    private void TimeScaleNine()
    {
        Debug.Log("Time Scale 9!");
        Time.timeScale = 9f;
    }

    private void ContinueGameInstant()
    {
        SaveSystem.UpdateMostRecentSaveDatas();
        SaveManager.Instance.LoadGame(SaveSystem.MostRecentSS, SaveSystem.MostRecentSE);
    }
#endif
}