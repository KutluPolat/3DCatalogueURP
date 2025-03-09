using System.Collections.Generic;

public class SuperStack<T>
{
    public int Count => _collection.Count;

    private int _lastIndex => _collection.Count - 1; // _lastIndex = ^1
    private List<T> _collection = new List<T>();
    private T _objectCache;

    public void Push(T @object) => _collection.Add(@object);
    public T Pop()
    {
        _objectCache = _collection[_lastIndex];
        _collection.RemoveAt(_lastIndex);
        return _objectCache;
    }
    public T Dequeue()
    {
        _objectCache = _collection[0];
        _collection.RemoveAt(0);
        return _objectCache;
    }

    public T Peek() => _collection[_lastIndex];
    public List<T> PeekAll() => _collection;

    public T Pull(int index)
    {
        if (index < _collection.Count)
        {
            return _collection[index];
        }
        else
        {
            throw new System.Exception("Index is out of range");
        }
    }

    public int IndexOf(T @object) => _collection.IndexOf(@object);
    public bool Remove(T @object) => _collection.Remove(@object);
    public void RemoveAt(int index) => _collection.RemoveAt(index);
    public void Clear() => _collection.Clear();
}