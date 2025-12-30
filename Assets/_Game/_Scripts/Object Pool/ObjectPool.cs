namespace DangQuocHieu.GameKit
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        private Dictionary<T, Queue<T>> _poolDictionary = new Dictionary<T, Queue<T>>();
        private Dictionary<T, T> _instanceToPrefab = new Dictionary<T, T>();
        [SerializeField] private List<T> _prefabs = new List<T>();
        [SerializeField] private int _initialSize = 10;

        public void InitPool()
        {
            foreach(var prefab in _prefabs)
            {
                if(!_poolDictionary.ContainsKey(prefab))
                {
                    _poolDictionary.Add(prefab, new Queue<T>());
                    for(int i = 0; i < _initialSize; i++)
                    {
                        _poolDictionary[prefab].Enqueue(CreateNewInstance(prefab));
                    }
                }
            }
        }

        private T Get(T prefab)
        {
            T instance;
            if(!_poolDictionary.ContainsKey(prefab))
            {
                _poolDictionary.Add(prefab, new Queue<T>());
            }
            if (_poolDictionary[prefab].Count == 0)
            {
                instance = CreateNewInstance(prefab);
            }
            else
            {
                instance = _poolDictionary[prefab].Dequeue();
            }
            _instanceToPrefab.Add(instance, prefab);
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public T Get(T prefab, Vector3 spawnPosition, Quaternion rotation, Transform parent)
        {
            T instance = Get(prefab);
            instance.transform.position = spawnPosition;
            instance.transform.rotation = rotation;
            instance.transform.SetParent(parent);
            if(instance is IPooledObject pooledObject)
            {
                pooledObject.OnObjectSpawn();
            }
            return instance;
        }

        public void Return(T instance)
        {
            if(_instanceToPrefab.TryGetValue(instance, out T prefab))
            {
                if(_poolDictionary.TryGetValue(prefab, out var pool))
                {
                    if(instance is IPooledObject pooledObject)
                    {
                        pooledObject.OnObjectSpawn();
                    }
                    instance.gameObject.SetActive(false);
                    instance.transform.SetParent(null);
                    pool.Enqueue(instance);
                    _instanceToPrefab.Remove(instance);
                }
                else
                {
                    Destroy(instance.gameObject);
                }
            }
            else
            {
                Destroy(instance.gameObject);
            }
        }

        public T CreateNewInstance(T prefab)
        {
            T newObj = Instantiate(prefab, transform);
            return newObj;
        }
    }

}