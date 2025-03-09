using UnityEngine;
using UnityEngine.UI;
using KPFramework;
using Sirenix.OdinInspector;
using TMPro;

public partial class UIMainMenu : UIBase
{
    [SerializeField, BoxGroup("MAIN MENU")] private Button buttonContinue;
    [SerializeField, BoxGroup("MAIN MENU")] private Button buttonNewGame;
    [SerializeField, BoxGroup("MAIN MENU")] private Button buttonBonusLevelChristmas;
    [SerializeField, BoxGroup("MAIN MENU")] private Button buttonLoadGame;
    [SerializeField, BoxGroup("MAIN MENU")] private TextMeshProUGUI txtContinue, txtNewGame, txtLoadGame;
    [SerializeField, BoxGroup("MAIN MENU")] private Button buttonCredits;
    [SerializeField, BoxGroup("MAIN MENU")] private Button[] buttonsClose;

    [SerializeField, BoxGroup("MAIN MENU")] private UIPanel_SaveLoad saveLoadPanel;

    protected override void Awake()
    {
        base.Awake();

        buttonContinue.onClick.AddListener(() => SaveManager.Instance.ContinueGame());
        buttonNewGame.onClick.AddListener(() => SaveManager.Instance.StartNewGame());
        buttonBonusLevelChristmas.onClick.AddListener(() =>
        {
            SaveSystem.UpdateMostRecentSaveDatas();
            SaveManager.Instance.LoadGame(SaveSystem.MostRecentSS, SaveSystem.MostRecentSE, GameLoop.Instance.GetArea("Christmas"));
        });

        buttonLoadGame.onClick.AddListener(() => OpenSaveLoadPanel());
        buttonCredits.onClick.AddListener(() => InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Credits, true, false)));

        foreach (var button in buttonsClose)
        {
            button.onClick.AddListener(() => GoBack());
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckButtons();

        AddEvent(EventName.TriggerUIAction, (o) =>
        {
            if (o is UIAction action)
            {
                if (action.ActionType == UIPanel.Initial)
                {
                    CheckButtons();
                }
            }
            else
            {
                DebugUtility.LogError(ErrorType.WrongEventParameter, EventName.TriggerUIAction.ToString());
            }
        });
    }

    private void OpenSaveLoadPanel()
    {
        saveLoadPanel.DrawSaveSlots();
        InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.ManageSaveSlotsPanel, true, false));
    }

    private void CheckButtons()
    {
        SaveSystem.UpdateMostRecentSaveDatas();

        bool canContinue = SaveSystem.MostRecentSE != null;
        buttonContinue.interactable = canContinue;
        txtContinue.color = canContinue ? Color.white : Color.gray;

        bool canLoad = SaveSystem.MostRecentSS != null;
        buttonLoadGame.interactable = canLoad;
        txtLoadGame.color = canLoad ? Color.white : Color.gray;

        bool canStartNewGame = SaveSystem.IsThereAnyEmptySS;
        buttonNewGame.interactable = canStartNewGame;
        txtNewGame.color = canStartNewGame ? Color.white : Color.gray;
    }
}