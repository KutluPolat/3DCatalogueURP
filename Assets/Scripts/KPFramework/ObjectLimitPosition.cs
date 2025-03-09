using UnityEngine;
 using System.Collections;

public class ObjectLimitPosition : MonoBehaviour
{
	[SerializeField] private bool _isLocal = true;
	private float _minX = -1;
	private float _maxX = 1;
	private float _minY = -1;
	private float _maxY = 1;
	private float _minZ = -1;
	private float _maxZ = 1;

	void LateUpdate()
	{
        if (_isLocal)
        {
			transform.localPosition = new Vector3(
				Mathf.Clamp(transform.localPosition.x, _minX, _maxX),										  
				Mathf.Clamp(transform.localPosition.y, _minY, _maxY),										   
				Mathf.Clamp(transform.localPosition.z, _minZ, _maxZ));
		}
        else
        {
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x, _minX, _maxX),
				Mathf.Clamp(transform.position.y, _minY, _maxY),
				Mathf.Clamp(transform.position.z, _minZ, _maxZ));
		}
	}
}