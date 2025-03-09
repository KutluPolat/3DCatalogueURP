using KPFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIIngameMenu : UIBase
{
    [SerializeField] private Button quickSave, mainMenu, quit, menuOpener, menuCloser;

    protected override void Awake()
    {
        base.Awake();
        menuOpener.onClick?.AddListener(() => InvokeEvent(KPFramework.EventName.TriggerUIAction, new UIAction(KPFramework.UIPanel.Menu, true, true)));
        menuCloser.onClick?.AddListener(() => InvokeEvent(KPFramework.EventName.TriggerUIGoBack));
        quickSave.onClick?.AddListener(() =>
        {
            SaveSystem.SaveGame(SaveType.QuickSave, true);
        });
        mainMenu.onClick?.AddListener(() =>
        {
            Confirmation.Instance.Ask(() => GameLoop.Instance.LoadScene("MainMenu"), null);
        });
        quit.onClick?.AddListener(() =>
        {
            Confirmation.Instance.Ask(() => Application.Quit(), null);
        });
    }
}
