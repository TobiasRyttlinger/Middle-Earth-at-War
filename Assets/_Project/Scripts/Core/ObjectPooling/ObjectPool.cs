using System.Collections.Generic;
using UnityEngine;

namespace BFME2.Core
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _available = new();
        private readonly HashSet<T> _active = new();

        public int ActiveCount => _active.Count;
        public int AvailableCount => _available.Count;

        public ObjectPool(T prefab, Transform parent, int preWarmCount = 0)
        {
            _prefab = prefab;
            _parent = parent;

            if (preWarmCount > 0)
            {
                PreWarm(preWarmCount);
            }
        }

        public void PreWarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var instance = Object.Instantiate(_prefab, _parent);
                instance.gameObject.SetActive(false);
                _available.Enqueue(instance);
            }
        }

        public T Rent()
        {
            T instance;

            if (_available.Count > 0)
            {
                instance = _available.Dequeue();
            }
            else
            {
                instance = Object.Instantiate(_prefab, _parent);
            }

            instance.gameObject.SetActive(true);
            _active.Add(instance);
            return instance;
        }

        public void Return(T instance)
        {
            if (instance == null) return;

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_parent);
            _active.Remove(instance);
            _available.Enqueue(instance);
        }

        public void ReturnAll()
        {
            var activeList = new List<T>(_active);
            foreach (var instance in activeList)
            {
                Return(instance);
            }
        }

        public void Clear()
        {
            foreach (var instance in _active)
            {
                if (instance != null)
                    Object.Destroy(instance.gameObject);
            }
            foreach (var instance in _available)
            {
                if (instance != null)
                    Object.Destroy(instance.gameObject);
            }
            _active.Clear();
            _available.Clear();
        }
    }
}
