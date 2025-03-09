using KPFramework;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    public int NumActive => _pool.Count - _availableObjects.Count;
    public bool IsInitialized => _isInitialized;

    private readonly T _prefab;
    private readonly Transform _parent;

    private readonly List<T> _pool = new();
    private readonly Stack<T> _availableObjects = new();

    private readonly bool _isInitialized;
    private Vector3 _originalScale;

    public ObjectPool(int initialSize, Transform parent, T prefabOfPool)
    {
        if (_isInitialized == false)
        {
            _isInitialized = true;
            _prefab = prefabOfPool;
            _parent = parent;

            while (_pool.Count < initialSize)
            {
                CreateNewObj();
            }
        }
        else
        {
            throw new System.Exception("Already initialized!");
        }
    }

    public T Pop()
    {
        CheckInitialization();

        if (_availableObjects.Count > 0)
        {
            _availableObjects.Peek().transform.localScale = _originalScale;
            _availableObjects.Peek().gameObject.SetActive(true);
            return _availableObjects.Pop();
        }
        else
        {
            CreateNewObj();
            return Pop();
        }
    }

    public void Push(T targetObject)
    {
        CheckInitialization();

        if (_pool.Contains(targetObject))
        {
            if (!_availableObjects.Contains(targetObject))
            {
                _availableObjects.Push(targetObject);
                targetObject.transform.SetParent(_parent);
                targetObject.gameObject.SetActive(false);
            }
        }
        else
        {
            _pool.Add(targetObject);
            Push(targetObject);
        }
    }

    public void CollectAll()
    {
        foreach (var obj in _pool)
        {
            Push(obj);
        }
    }

    private void CreateNewObj()
    {
        T newGameObject = MonoBehaviour.Instantiate<T>(_prefab, _parent);
        _availableObjects.Push(newGameObject);
        _pool.Add(newGameObject);
        newGameObject.gameObject.SetActive(false);

        if (_pool.Count == 1) // is first created object?
        {
            _originalScale = _pool[0].transform.localScale;
        }
    }

    private void CheckInitialization()
    {
        if (_isInitialized == false)
        {
            DebugUtility.LogError(ErrorType.NotInitialized, "Object Pool");
        }
    }
}