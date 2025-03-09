using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        FixedUpdate();
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _cameraTransform.position);
    }
}