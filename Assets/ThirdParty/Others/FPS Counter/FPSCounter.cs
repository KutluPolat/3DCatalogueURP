using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsDisplay;
    public Text averageFPSDisplay;
    int framesPassed = 0;
    float fpsTotal = 0f;
    public Text minFPSDisplay, maxFPSDisplay;
    float minFPS = Mathf.Infinity;
    float maxFPS = 0f;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        float fps = 1 / Time.unscaledDeltaTime;
        if (fpsDisplay != null)
        {
            fpsDisplay.text = "" + fps;

        }

        fpsTotal += fps;
        framesPassed++;
        if (averageFPSDisplay != null)
        {
            averageFPSDisplay.text = "Avarage: " + (fpsTotal / framesPassed);

        }

        if (fps > maxFPS && framesPassed > 10 && maxFPSDisplay != null)
        {
            maxFPS = fps;
            maxFPSDisplay.text = "Max: " + maxFPS;
        }
        if (fps < minFPS && framesPassed > 10 && minFPSDisplay != null)
        {
            minFPS = fps;
            minFPSDisplay.text = "Min: " + minFPS;
        }
    }

}
