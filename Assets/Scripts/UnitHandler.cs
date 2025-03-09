using KPFramework;
using UnityEngine;

// Initializations & Fields & Properties & AutoRotation
public partial class UnitHandler : ScriptBase, ISave
{
    //public Rigidbody Rigidbody;
    [SerializeField] private SO_Character unit;

    private SO_Class unitClass;

    private void Start()
    {
        LoadVariables();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        AddEvent(EventName.UpdateSaveEntry, UpdateSaveEntry);
        AddEvent(EventName.LoadVariablesFromSE, LoadVariables);

        unitClass = unit.Class;

        var dataUnitProgression = unit.Data_Unit.Progression;
    }

    public void UpdateSaveEntry(object o = null)
    {

        DebugUtility.LogError(ErrorType.NotImplemented, "");
    }

    public void LoadVariables(object o = null)
    {

        DebugUtility.LogError(ErrorType.NotImplemented, "");
    }
}