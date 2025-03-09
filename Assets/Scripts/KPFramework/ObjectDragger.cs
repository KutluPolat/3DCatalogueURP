using UnityEngine;
using System.Collections;
 
public class ObjectDragger : MonoBehaviour 
{
 
	private Vector3 _initScreenPoint, _currentScreenPoint;
	private Vector3 _offset;
	 
	void OnMouseDown()
	{
		_initScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
		_offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _initScreenPoint.z));
	}
	
	void OnMouseDrag()
	{
		_currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _initScreenPoint.z);
		transform.position = Camera.main.ScreenToWorldPoint(_currentScreenPoint) + _offset;
	}
 }