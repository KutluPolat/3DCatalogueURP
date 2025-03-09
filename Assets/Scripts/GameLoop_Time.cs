using KPFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameLoop : ScriptBase
{
    public float TimeScale { get; set; } = 1f;

    public void StopGame()
    {
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        Time.timeScale = TimeScale;
    }
}