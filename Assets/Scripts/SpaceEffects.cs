using DG.Tweening;
using KPFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

public class SpaceEffects : ScriptBase
{
    private CinemachineCamera _vcam;
    private Tween tweenVCam;
    private Tween tweenChromatic;
    private Tween tweenLens;

    private ChromaticAberration _chromaticAberration;
    private LensDistortion _lensDistortion;

    // Retrieves the Chromatic Aberration effect from the current URP Volume stack.
    private ChromaticAberration GetChromaticAberration()
    {
        if (_chromaticAberration == null)
        {
            VolumeStack stack = VolumeManager.instance.stack;
            _chromaticAberration = stack.GetComponent<ChromaticAberration>();
        }
        return _chromaticAberration;
    }

    // Retrieves the Lens Distortion effect from the current URP Volume stack.
    private LensDistortion GetLensDistortion()
    {
        if (_lensDistortion == null)
        {
            VolumeStack stack = VolumeManager.instance.stack;
            _lensDistortion = stack.GetComponent<LensDistortion>();
        }
        return _lensDistortion;
    }

    // Retrieves the Cinemachine noise component for camera shake.
    private CinemachineBasicMultiChannelPerlin GetNoise()
    {
        if (_vcam == null)
        {
            _vcam = MainCamera.vcam;
        }
        return _vcam?.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public IEnumerator StartHyperDrive()
    {
        float duration = 1f;
        AudioManager.Instance.PlaySFX(AudioNames.VFX_HyperDriveStart);

        // Activate and animate Chromatic Aberration.
        var cAberration = GetChromaticAberration();
        if (cAberration != null)
        {
            cAberration.active = true;
            cAberration.intensity.value = 0f;
            cAberration.intensity.overrideState = true;

            if (tweenChromatic != null && tweenChromatic.IsActive())
                tweenChromatic.Kill();

            tweenChromatic = DOTween.To(() => cAberration.intensity.value, x => cAberration.intensity.value = x, 1f, duration);
        }

        // Activate and animate Lens Distortion.
        var lensDistortion = GetLensDistortion();
        if (lensDistortion != null)
        {
            lensDistortion.active = true;
            lensDistortion.intensity.value = 0f;
            lensDistortion.intensity.overrideState = true;

            if (tweenLens != null && tweenLens.IsActive())
                tweenLens.Kill();

            tweenLens = DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, -100f, duration);
        }

        // Animate camera shake using the Cinemachine noise component.
        var noise = GetNoise();
        if (noise != null)
        {
            if (tweenVCam != null && tweenVCam.IsActive())
                tweenVCam.Kill();

            tweenVCam = DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, 3f, duration);
        }

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator CompleteHyperDrive(Transform warpGate, Transform spaceShip)
    {
        float duration = 0.25f;
        AudioManager.Instance.PlaySFX(AudioNames.VFX_EngineIgnition);

        yield return new WaitForSeconds(0.1f);

        // Animate the spaceship moving forward and scaling down.
        spaceShip.localScale = new Vector3(0.2f, 0.2f, 10f);
        spaceShip.DOMove(spaceShip.position + Vector3.forward * 250f, 0.15f).OnComplete(() =>
        {
            spaceShip.DOScale(Vector3.zero, 0.1f);
        });

        // Reset Chromatic Aberration.
        var cAberration = GetChromaticAberration();
        if (cAberration != null)
        {
            if (tweenChromatic != null && tweenChromatic.IsActive())
                tweenChromatic.Kill();

            tweenChromatic = DOTween.To(() => cAberration.intensity.value, x => cAberration.intensity.value = x, 0f, duration);
        }

        // Reset Lens Distortion.
        var lensDistortion = GetLensDistortion();
        if (lensDistortion != null)
        {
            if (tweenLens != null && tweenLens.IsActive())
                tweenLens.Kill();

            tweenLens = DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, 0f, duration);
        }

        // Reset camera shake by animating the noise amplitude back to 0.
        var noise = GetNoise();
        if (noise != null)
        {
            if (tweenVCam != null && tweenVCam.IsActive())
                tweenVCam.Kill();

            tweenVCam = DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, 0f, duration);
        }

        yield return new WaitForSeconds(duration);

        // Deactivate post-process effects.
        if (cAberration != null)
        {
            cAberration.active = false;
            cAberration.intensity.overrideState = false;
        }

        if (lensDistortion != null)
        {
            lensDistortion.active = false;
            lensDistortion.intensity.overrideState = false;
        }

        // Apply a final camera shake effect using the noise component.
        if (noise != null)
        {
            // Animate amplitude to 4 over 0.5 seconds, then back to 0 over 0.25 seconds.
            DOTween.Kill(noise);
            DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, 4f, 0.5f)
                .OnComplete(() =>
                {
                    DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, 0f, 0.25f);
                });
        }
    }
}
