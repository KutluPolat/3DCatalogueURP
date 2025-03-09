using DG.Tweening;
using KPFramework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public struct UIAction
{
    public UIPanel ActionType;
    public bool HideAllExceptFinalAction;
    public bool OpenBackground;

    public UIAction(UIPanel collection, bool hideAllExceptFinalAction, bool background)
    {
        ActionType = collection;
        HideAllExceptFinalAction = hideAllExceptFinalAction;
        OpenBackground = background;
    }

    public bool IsEqualToThisAction(UIPanel action) => ActionType == action;
}

public class UIBase : ScriptBase
{
    [SerializeField, BoxGroup("UIBase")] private SerializableDictionary<UIElement, CanvasGroup> dictUIElements;
    [SerializeField, BoxGroup("UIBase")] private SerializableDictionary<UIPanel, List<UIElement>> dictActionCollection;
    [SerializeField, BoxGroup("UIBase")] private CanvasGroup background;

    private UIAction lastClickAction;

    private Stack<UIAction> clickActions = new();

    protected virtual void Awake()
    {
        dictUIElements.InitializeDict();
        dictActionCollection.InitializeDict();

        lastClickAction = new UIAction(UIPanel.Initial, true, false);
        ApplyAction(lastClickAction, true, 0f);

        if (!dictActionCollection.ContainsKey(UIPanel.Unselected))
        {
            var pair = new SerializableKeyValuePair<UIPanel, List<UIElement>>
            {
                Key = UIPanel.Unselected,
                Value = new List<UIElement>()
            };

            dictActionCollection.Add(pair);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        AddEvent(EventName.TriggerUIAction, (o) =>
        {
            if (o is UIAction clickAction)
            {
                ApplyAction(clickAction, true, Config.UI_ACT_DUR);
            }
            else
            {
                DebugUtility.LogError(ErrorType.WrongEventParameter, nameof(EventName.TriggerUIAction));
            }
        });

        AddEvent(EventName.HideInGame, (o) => LockAll());
        AddEvent(EventName.ShowInGame, (o) => UnlockAll());
        AddEvent(EventName.TriggerUIGoBack, GoBack);
    }

    protected void GoBack(object o = null)
    {
        if (clickActions.Count == 0)
            return;

        ApplyAction(clickActions.Pop(), false, Config.UI_ACT_DUR);
        // TO-DO: It will work when you press the back button on the phones. Learn how to do it.
    }

    protected void ReApplyLastValidAction()
    {
        ApplyAction(lastClickAction, false, Config.UI_ACT_DUR);
    }

    protected void ApplyAction(UIAction targetAction, bool saveToStack, float duration)
    {
        bool hasActionCollection = dictActionCollection.ContainsKey(targetAction.ActionType);
        if (!hasActionCollection)
        {
            targetAction.ActionType = UIPanel.Unselected;
            ApplyAction(targetAction, saveToStack, duration);
            return;
        }

        if (targetAction.OpenBackground)
        {
            ActivateCG(background, duration);
        }
        else
        {
            DeactivateCG(background, duration);
        }

        List<UIElement> targetActionCollection = dictActionCollection.GetValue(targetAction.ActionType);

        dictUIElements.ForEach((pair) =>
        {
            if (targetActionCollection.Contains(pair.Key))
            {
                ActivateCG(pair.Value, duration);
            }
            else
            {
                DeactivateCG(pair.Value, duration);
            }
        });

        if (saveToStack)
        {
            clickActions.Push(lastClickAction);
        }

        lastClickAction = targetAction;
    }

    protected void LockAll()
    {
        dictUIElements.ForEach((pair) =>
        {
            pair.Value.interactable = false;
        });

        ApplyAction(new UIAction(UIPanel.Unselected, true, false), false, Config.UI_ACT_DUR);
    }

    protected void UnlockAll()
    {
        dictUIElements.ForEach((pair) =>
        {
            pair.Value.interactable = true;
        });

        ReApplyLastValidAction();
    }

    private void ActivateCG(CanvasGroup cg, float duration)
    {
        if (cg == null)
            return;

        cg.DOKill();
        cg.gameObject.SetActive(true);
        cg.interactable = true;
        cg.blocksRaycasts = true;

        if (duration == 0f)
        {
            cg.alpha = 1f;
        }
        else
        {
            cg.DOFade(1f, duration);
        }
    }

    private void DeactivateCG(CanvasGroup cg, float duration)
    {
        if (cg == null)
            return;
        
        cg.DOKill();
        cg.interactable = false;
        cg.blocksRaycasts = false;

        if (duration == 0f)
        {
            cg.alpha = 0f;
            cg.gameObject.SetActive(false);
        }
        else
        {
            cg.DOFade(0f, duration).OnComplete(() => cg.gameObject.SetActive(false));
        }
    }
}
