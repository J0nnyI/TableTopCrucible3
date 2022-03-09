using System;
using System.Collections.Generic;

namespace TableTopCrucible.Core.Helper;

public static class DictionaryHelper
{
    public static void AddOrUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dict,
        TKey key, 
        TValue newValue)
    {
        if (dict.TryGetValue(key, out var value))
            dict[key] = newValue;
        else
            dict.Add(key, newValue);
    }

    public static void AddOrUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dict,
        TKey key, 
        TValue initialValue,
        Func<TValue, TValue> updater)
    {
        if (dict.TryGetValue(key, out var value))
            dict[key] = updater(value);
        else
            dict.Add(key, initialValue);
    }
}