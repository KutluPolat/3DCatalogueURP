using KPFramework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableKeyValuePair<TEnum, TType>
{
    public TEnum Key;
    public TType Value;
}
[System.Serializable]
public class SerializableKeyValuePair<TEnum, TType1, TType2>
{
    public TEnum Key;
    public TType1 Value1;
    public TType2 Value2;
}

[System.Serializable]
public class SerializableDictionary<TEnum, TType>
{
    [SerializeField] private List<SerializableKeyValuePair<TEnum, TType>> KeyValuePairs = new();
    private Dictionary<TEnum, TType> dict;

    public bool ContainsKey(TEnum key)
    {
        return dict.ContainsKey(key);
    }

    public void ForEach(System.Action<SerializableKeyValuePair<TEnum, TType>> action)
    {
        foreach (var pair in  KeyValuePairs)
        {
            action?.Invoke(pair);
        }
    }

    public TType GetValue(TEnum key)
    {
        return dict[key];
    }

    public bool TryGetValue(TEnum key, out TType value)
    {
        if (dict == null)
        {
            DebugUtility.LogError(ErrorType.NotInitialized, "KeyValueDict");
            value = default;
            return false;
        }

        if (dict.ContainsKey(key))
        {
            value = dict[key];
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public void Modify(TEnum key, TType newValue)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = newValue;
            
            foreach (var  pair in KeyValuePairs)
            {
                if (pair.Key.Equals(key))
                {
                    pair.Value = newValue;
                }
            }
        }
        else
        {
            var newPair = new SerializableKeyValuePair<TEnum, TType>
            {
                Key = key,
                Value = newValue
            };

            Add(newPair);
        }
    }

    public void Add(SerializableKeyValuePair<TEnum, TType> pair)
    {
        if (dict.TryAdd(pair.Key, pair.Value))
        {
            KeyValuePairs.Add(pair);
        }
    }

    public void Remove(TEnum key)
    {
        if (dict.ContainsKey(key))
        {
            var pair = new SerializableKeyValuePair<TEnum, TType>
            {
                Key = key,
                Value = dict[key]
            };

            dict.Remove(key);
            KeyValuePairs.Remove(pair);
        }
    }

    public void InitializeDict()
    {
        if (dict == null)
        {
            dict = new();
            foreach (var pairs in KeyValuePairs)
            {
                dict.Add(pairs.Key, pairs.Value);
            }
        }
        else if (dict.Count != KeyValuePairs.Count)
        {
            foreach (var pairs in KeyValuePairs)
            {
                dict.TryAdd(pairs.Key, pairs.Value);
            }
        }
    }
}