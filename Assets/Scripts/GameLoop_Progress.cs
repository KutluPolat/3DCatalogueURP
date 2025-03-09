using KPFramework;
using UnityEngine;

public partial class GameLoop : ScriptBase
{
    public SO_Area CurrentArea { get; private set; }
    public AreaStoryBase CurrentAreaScript { get; private set; }
}
