using System.Collections.Generic;
using UnityEngine;

namespace ImprovedWorkflow.ObjectPooler
{
    public abstract class PoolBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _objectPrefab;
        [SerializeField] private int _initialCount;

        private readonly Queue<T> m_poolQueue = new();

        protected void InitPool()
        {
            for (int i = 0; i < _initialCount; i++)
            {
                T obj = Instantiate(_objectPrefab, transform);
                m_poolQueue.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        protected void InitPool(T prefab, int count)
        {
            _objectPrefab = prefab;
            _initialCount = count;
            InitPool();
        }

        public T Get()
        {
            T obj = m_poolQueue.Dequeue();
            obj.gameObject.SetActive(true);
            m_poolQueue.Enqueue(obj);

            return obj;
        }

        public T Get(Vector3 position)
        {
            T obj = m_poolQueue.Dequeue();
            obj.transform.position = position;
            obj.gameObject.SetActive(true);
            m_poolQueue.Enqueue(obj);

            return obj;
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    }
}