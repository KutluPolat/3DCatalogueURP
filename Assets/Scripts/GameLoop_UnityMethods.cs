using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLoop : ScriptBase
{
    private void Awake()
    {
        AwakePartial();
    }

    private void Start()
    {
        StartPartial();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnEnablePartial();
    }

    //protected override void OnDisable()
    //{
    //    base.OnDisable();
    //}

    partial void AwakePartial();
    partial void StartPartial();
    partial void OnEnablePartial();
}
