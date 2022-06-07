﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Universalis.Application.Caching;

public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : IEquatable<TKey> where TValue : class
{
    private readonly object _lock;
    private readonly CacheEntry<TKey, TValue>[] _data;
    private readonly IDictionary<TKey, int> _idMap;
    private readonly Stack<int> _freeEntries;

    public int Count => GetCount();

    public MemoryCache(int size)
    {
        _lock = new object();
        _data = new CacheEntry<TKey, TValue>[size];
        _idMap = new Dictionary<TKey, int>();
        _freeEntries = new Stack<int>(Enumerable.Range(0, size));
        _freeEntries.TrimExcess();
    }

    public virtual Task Set(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var keyCopy = JsonSerializer.Deserialize<TKey>(JsonSerializer.Serialize(key));
        var valCopy = JsonSerializer.Deserialize<TValue>(JsonSerializer.Serialize(value));
        if (keyCopy == null || valCopy == null) throw new ArgumentException("key or value de/serialized to null.");

        Monitor.Enter(_lock);
        try
        {
            // Check if this key already has an entry associated with it
            // that we can reuse
            if (_idMap.TryGetValue(keyCopy, out var idx))
            {
                _data[idx].Referenced = false;
                _data[idx].Value = valCopy;
                return Task.CompletedTask;
            }

            CleanAdd(keyCopy, valCopy);
        }
        finally
        {
            Monitor.Exit(_lock);
        }
        
        return Task.CompletedTask;
    }

    public virtual Task<TValue> Get(TKey key, CancellationToken cancellationToken = default)
    {
        Monitor.Enter(_lock);
        try
        {
            if (!_idMap.TryGetValue(key, out var idx)) return Task.FromResult<TValue>(null);

            var val = _data[idx];
            val.Referenced = true;

            var valCopy = JsonSerializer.Deserialize<TValue>(JsonSerializer.Serialize(val.Value));
            return Task.FromResult(valCopy);
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    public virtual Task<bool> Delete(TKey key, CancellationToken cancellationToken = default)
    {
        Monitor.Enter(_lock);
        try
        {
            if (!_idMap.TryGetValue(key, out var idx))
            {
                return Task.FromResult(false);
            }

            CleanRemove(idx);

            return Task.FromResult(true);
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }
    
    /// <summary>
    /// Evicts an entry from the cache, returning the evicted entry's index to the free entry stack.
    /// </summary>
    /// <returns>true if an entry was evicted; otherwise false.</returns>
    protected virtual bool Evict()
    {
        // This loops at most once due to all of the reference booleans being
        // flipped in the first iteration.
        while (_data.Length != 0)
        {
            for (var i = 0; i < _data.Length; i++)
            {
                if (_data[i] == null) continue;

                if (!_data[i].Referenced)
                {
                    CleanRemove(i);
                    return true;
                }

                _data[i].Referenced = false;
            }
        }

        return false;
    }

    private int GetCount()
    {
        Monitor.Enter(_lock);
        try
        {
            return _idMap.Count;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    private void CleanAdd(TKey key, TValue value)
    {
        // Get a data array index
        if (!_freeEntries.TryPop(out var nextIdx))
        {
            while (!Evict())
            {
            }

            nextIdx = _freeEntries.Pop();
        }

        // Set the cache entry
        _idMap.Add(key, nextIdx);
        _data[nextIdx] = new CacheEntry<TKey, TValue>
        {
            Key = key,
            Value = value,
        };
    }

    private void CleanRemove(int idx)
    {
        var val = _data[idx];

        _data[idx] = null;
        _idMap.Remove(val.Key);
        _freeEntries.Push(idx);
    }

    private class CacheEntry<TCacheKey, TCacheValue>
    {
        public bool Referenced;

        public TCacheKey Key;

        public TCacheValue Value;
    }
}