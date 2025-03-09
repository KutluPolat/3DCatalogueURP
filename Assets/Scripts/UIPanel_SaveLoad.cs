using KPFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel_SaveLoad : MonoBehaviour
{
    private ObjectPool<UIButton> opSaveHistory;

    [SerializeField] private CanvasGroup cgSaveLoadPanelContainer;
    [SerializeField] private UIButton prefabSaveHistory;
    [SerializeField] private RectTransform parentSaveHistory;

    [SerializeField] private UIButton[] saveSlots; 

    private Data_SaveSlot selectedSaveSlot;
    private List<UIButton> activeSaveHistories = new();

    private void Awake()
    {
        opSaveHistory = new(1, parentSaveHistory, prefabSaveHistory);
    }

    private void OnDisable()
    {
        opSaveHistory.CollectAll();
    }

    public void DrawSaveSlots()
    {
        // TO-DO: yeni savesystem'a gecince buradaki cacheleme gereksiz olacak

        for (int i = 0; i < saveSlots.Length; i++)
        {
            DrawSaveSlot(i);
        }

        void DrawSaveSlot(int index)
        {
            var ss = SaveSystem.GetSSData(index);
            var ssButton = saveSlots[index];

            ssButton.Button.onClick.RemoveAllListeners();

            if (ss.IsValid)
            {
                var se = ss.LastEntry;

                SaveSystem.OnSaveSlotSelected(ss, se);
                
                ssButton.UIText.gameObject.SetActive(true);
                ssButton.Img.gameObject.SetActive(false);

                ssButton.UIText.OverrideText(se.GetSSString());
                ssButton.Button.onClick.AddListener(() => DrawSaveHistories(ss));
            }
            else
            {
                ssButton.UIText.gameObject.SetActive(false);
                ssButton.Img.gameObject.SetActive(true);
                ssButton.Button.onClick.AddListener(() => OnLoadSceneButtonClicked(() => SaveManager.Instance.StartNewGame()));
            }
        }
    }

    private void DrawSaveHistories(Data_SaveSlot ss)
    {
        opSaveHistory.CollectAll();

        if (ss == null || !ss.IsValid)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, "Saveslot");
            return;
        }

        if (ss.LastEntry == null)
        {
            DebugUtility.LogError(ErrorType.MajorError, "THERE IS NO ENTRY IN SAVE SLOT!");
            return;
        }

        foreach(var entry in ss.Entries)
        {
            var shButton = opSaveHistory.Pop();
            
            shButton.UIText.gameObject.SetActive(true);
            shButton.UIText.OverrideText(entry.GetSEString());

            shButton.Button.onClick.AddListener(() => OnLoadSceneButtonClicked(() => SaveManager.Instance.LoadGame(ss, entry)));
            shButton.transform.SetAsFirstSibling();
        }

        parentSaveHistory.sizeDelta = new Vector2(parentSaveHistory.sizeDelta.x, Mathf.Max(ss.Entries.Count * Screen.height * 0.12f, Screen.height * 0.5f));
    }

    private void OnLoadSceneButtonClicked(Action onClicked)
    {
        cgSaveLoadPanelContainer.blocksRaycasts = false;
        onClicked?.Invoke();
    }

    public void Clear()
    {

    }

    public void DeleteHistory(int index)
    {

    }
}