using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticHandler : MonoBehaviour
{
    public static int HapticStrength;

    // TO-DO: Haptic eklenecek
    // TO-DO: Controller vibration haptici eklenecek

    private void Awake()
    {
        HapticStrength = SaveSystem.Options.Vibrations;
    }
}