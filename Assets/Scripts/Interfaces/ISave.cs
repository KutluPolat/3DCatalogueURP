using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISave
{
    void UpdateSaveEntry(object o = null);
    void LoadVariables(object o = null);
}