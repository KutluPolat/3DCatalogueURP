using System.Collections;
using UnityEngine;

public class CameraFocuser : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (MainCamera.vcam == null)
            yield return new WaitForEndOfFrame();

        MainCamera.vcam.SetFollowTarget(transform);
        MainCamera.vcam.SetAimTarget(transform);
        MainCamera.vcam.CancelDamping(true);
    }
}