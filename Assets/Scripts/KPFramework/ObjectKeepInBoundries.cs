using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectKeepInBoundries : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private float _leftOffsetPercent, _rightOffsetPercent, _upOffsetPercent, _bottomOffsetPercent;
    private float _leftOffset, _rightOffset, _upOffset, _bottomOffset;
    private Camera _mainCamera;
    private Vector2 _screenBounds;
    private Vector3 _latestPos, _cacheScreenPos;
    //private float _objectWidth;
    //private float _objectHeight;

    void Start()
    {
        _mainCamera = Camera.main;
        _latestPos = transform.position;
        _screenBounds = new Vector2(_mainCamera.pixelWidth, _mainCamera.pixelHeight);

        _leftOffset = _screenBounds.x * _leftOffsetPercent * 0.01f;
        _rightOffset = _screenBounds.x * _rightOffsetPercent * 0.01f;
        _upOffset = _screenBounds.y * _upOffsetPercent * 0.01f;
        _bottomOffset = _screenBounds.y * _bottomOffsetPercent * 0.01f;

        //_screenBounds = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));
        //_objectWidth = transform.GetComponent<Renderer>().bounds.extents.x; //extents = size of width / 2
        //_objectHeight = transform.GetComponent<Renderer>().bounds.extents.y; //extents = size of height / 2
    }

    void LateUpdate()
    {
        if (IsOutOfScreenBounds(transform.position))
        {
            transform.position = _latestPos;
        }

        _latestPos = transform.position;

        //Vector3 viewPos = transform.position;
        //viewPos.x = Mathf.Clamp(viewPos.x, _screenBounds.x /*+ _objectWidth*/, _screenBounds.x * -1 /*- _objectWidth*/);
        //viewPos.y = Mathf.Clamp(viewPos.y, _screenBounds.y /*+ _objectHeight*/, _screenBounds.y * -1/* - _objectHeight*/);
        //transform.position = viewPos;
    }

    private bool IsOutOfScreenBounds(Vector3 worldPos)
    {
        _cacheScreenPos = Camera.main.WorldToScreenPoint(worldPos);

        return _cacheScreenPos.x < _leftOffset || _cacheScreenPos.x > (_screenBounds.x - _rightOffset) || _cacheScreenPos.y < _bottomOffset || _cacheScreenPos.y > (_screenBounds.y - _upOffset);
    }
}