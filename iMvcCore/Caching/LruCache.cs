using System;
using System.Collections.Generic;

namespace iMvcCore.Caching
{
     /// <summary>
    /// Least recently used cache
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public sealed class LruCache<K, V>
    {
        private readonly int _capacity;
        private readonly Dictionary<K, Item> _dict;
        private readonly LinkedList<K> _linkedList = new LinkedList<K>();
        private readonly object _lock = new object();

        public LruCache() : this(16)
        {
        }

        public LruCache(int capacity)
        {
            _capacity = capacity;
            _dict = new Dictionary<K, Item>(capacity);
        }

        public V GetOrAdd(K key, Func<V> valFunc)
        {
            lock (_lock)
            {
                if (_dict.TryGetValue(key, out Item item))
                {
                    _linkedList.Remove(item.Node);
                }
                else
                {
                    if (_dict.Count >= _capacity)
                    {
                        LinkedListNode<K> lastNode = _linkedList.Last;
                        _linkedList.RemoveLast();
                        _dict.Remove(lastNode.Value);
                    }

                    item = new Item(valFunc(), new LinkedListNode<K>(key));
                    _dict[key] = item;
                }

                _linkedList.AddFirst(item.Node);
                return item.Value;
            }
        }

        public void Remove(K key)
        {
            lock (_lock)
            {
                if (!_dict.TryGetValue(key, out Item item))
                {
                    return;
                }

                _dict.Remove(key);
                _linkedList.Remove(item.Node);
            }
        }

        public IEnumerable<K> GetAllKeys()
        {
            lock (_lock)
            {
                foreach (var item in _dict)
                {
                    yield return item.Key;
                }
            }
        }

        private struct Item
        {
            public Item(V value, LinkedListNode<K> node)
            {
                Value = value;
                Node = node;
            }

            public V Value { get; }
            public LinkedListNode<K> Node { get; }
        }
    }
}