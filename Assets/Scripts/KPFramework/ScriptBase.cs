// #define LOG_ENABLED

using KPFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptBase : MonoBehaviour
{
    public struct EventRegistry
    {
        public EventName name;
        public UnityAction<object> action;
    }
    List<EventRegistry> eventList = new List<EventRegistry>();
    private bool safeToGo = false;
    protected void AddEvent(EventName name, UnityAction<object> action)
    {
        if (action == null)
            return;

        EventRegistry eventRegistry;
        eventRegistry.name = name;
        eventRegistry.action = action;
        eventList.Add(eventRegistry);

        Events.AddEvent(name, action);
#if LOG_ENABLED
		    Debug.Log("(" + transform.name + ") " + action.Method.Name + " is added to " + name.ToString());
#endif
    }
    protected void RemoveEvent(EventName name, UnityAction<object> action)
    {
        EventRegistry eventRegistry;
        eventRegistry.name = name;
        eventRegistry.action = action;
        eventList.Remove(eventRegistry);

        Events.RemoveEvent(name, action);
#if LOG_ENABLED
		    Debug.Log("(" + transform.name + ") " + action.Method.Name + " is removed from " + name.ToString());
#endif
    }
    protected void RemoveAllEvents()
    {
        foreach (var eventRegistry in eventList)
        {
            Events.RemoveEvent(eventRegistry.name, eventRegistry.action);
        }

        eventList.Clear();

#if LOG_ENABLED
		Debug.Log("(" + transform.name + ") All events removed");
#endif
    }
    protected void InvokeEvent(EventName name, object param = null)
    {
        Events.InvokeEvent(name, param);
    }

    protected virtual void OnEnable()
    {
        safeToGo = false;
    }

    protected virtual void OnDisable()
    {
        if (safeToGo)
            return;

        RemoveAllEvents();
        StopAllCoroutines();
        safeToGo = true;
    }

    protected virtual void OnDestroy()
    {
        if (safeToGo)
            return;

        RemoveAllEvents();
        StopAllCoroutines();
        safeToGo = true;
    }

    protected void PrintAllRegisteredEvents()
    {
        foreach (var eventRegistry in eventList)
        {
            Debug.Log("Event name: " + eventRegistry.name + ", action: " + eventRegistry.action.Method.Name);
        }
    }
}