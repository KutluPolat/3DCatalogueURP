using DG.Tweening;
using KPFramework;
using Unity.Cinemachine;
using UnityEngine;

public static class Extensions
{
    public static bool IsInRange(this int num, int minIncluded, int maxIncluded)
    {
        return num >= minIncluded && num <= maxIncluded;
    }

    public static void SetAimTarget(this CinemachineCamera vcam, Transform target)
    {
        if (vcam == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, "VCAM"); 
            return;
        }

        vcam.LookAt = target;
    }

    public static void SetFollowTarget(this CinemachineCamera vcam, Transform target)
    {
        if (vcam == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, "VCAM");
            return;
        }

        vcam.Follow = target;
    }

    public static void Shake(this CinemachineCamera vcam, float duration = 0.5f, float strength = 2f, CinemachineBasicMultiChannelPerlin perlin = null)
    {
        if (perlin == null)
            vcam.TryGetComponent(out perlin);

        if (perlin == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(CinemachineBasicMultiChannelPerlin));
        }
        else
        {
            vcam.Shake(duration, strength, perlin);
        }
    }

    public static Tween SetShake(this CinemachineCamera vcam, float duration, float strength, CinemachineBasicMultiChannelPerlin perlin = null)
    {
        if (perlin == null)
            vcam.TryGetComponent(out perlin);

        if (perlin == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(CinemachineBasicMultiChannelPerlin));
            return null;
        }
        else
        {
            return DOTween.To(() => perlin.AmplitudeGain, x => { perlin.AmplitudeGain = x; perlin.FrequencyGain = x; }, strength, duration);
        }
    }

    public static void SetShake(this CinemachineCamera vcam, float strength, CinemachineBasicMultiChannelPerlin perlin = null)
    {
        if (perlin == null)
            vcam.TryGetComponent(out perlin);

        if (perlin == null)
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(CinemachineBasicMultiChannelPerlin));
            return;
        }
        else
        {
            perlin.AmplitudeGain = strength;
            perlin.FrequencyGain = strength;
        }
    }

    public static bool IsAlive(this Tween tween)
    {
        return tween != null && tween.IsActive() && tween.IsPlaying();
    }
}