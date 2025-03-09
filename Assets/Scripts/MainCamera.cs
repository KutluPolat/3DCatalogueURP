using TreeEditor;
using Unity.Cinemachine;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static Camera main
    {
        get
        {
            if (_b == null)
                _b = Camera.main;

            return _b;
        }
    }
    public static CinemachineCamera vcam
    {
        get
        {
            if (_a == null)
            {
                if (brain == null)
                {
                    brain = main.GetComponent<CinemachineBrain>();
                }

                if (brain != null)
                {
                    _a = (CinemachineCamera)brain.ActiveVirtualCamera;
                }
            }

            return _a;
        }
    }

    private static Camera _b;
    private static CinemachineBrain brain;
    private static CinemachineCamera _a;
    private CinemachineOrbitalFollow orbitalTransposer;

    private void Update()
    {
        if (InputManager.DeltaPosMouse1 != Vector3.zero)
        {
            RotateOrbitalTransposer(InputManager.DeltaPosMouse1.x * Time.deltaTime * 25f);
        }
    }

    private void RotateOrbitalTransposer(float amount)
    {
        if (vcam == null)
            return;

        if (orbitalTransposer == null)
            vcam.TryGetComponent(out orbitalTransposer);

        if (orbitalTransposer == null)
            return;

        orbitalTransposer.HorizontalAxis.Value += amount;
    }
}
