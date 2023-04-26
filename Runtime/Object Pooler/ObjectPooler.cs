using System.Collections.Generic;
using UnityEngine;

namespace ImprovedWorkflow.ObjectPooler
{
    public class ObjectPooler<T> where T : MonoBehaviour
    {
        private const int expand_step = 10;

        private readonly Queue<T> m_items;
        private readonly Transform m_parent;

        private readonly T m_prefab;

        public ObjectPooler(T prefab, int count)
        {
            m_items = new Queue<T>(count);
            m_prefab = prefab;
            InitializePool(prefab, count);
        }

        public ObjectPooler(T prefab, int count, Transform parent)
        {
            m_items = new Queue<T>(count);
            m_parent = parent;
            InitializePool(prefab, count, parent);
        }

        public T Get()
        {
            if (m_items.Count == 0)
            {
                int c = 0;
                while (c < expand_step)
                {
                    InstantiateInstance(m_prefab, m_parent);
                    c++;
                }
            }

            return m_items.Dequeue();
        }

        public void Return(T t)
        {
            m_items.Enqueue(t);
        }

        private void InitializePool(T prefab, int count)
        {
            for (int i = 0; i < count; i++) InstantiateInstance(prefab);
        }

        private void InitializePool(T prefab, int count, Transform parent)
        {
            for (int i = 0; i < count; i++) InstantiateInstance(prefab, parent);
        }

        private void InstantiateInstance(T prefab)
        {
            T instance = Object.Instantiate(prefab);
            m_items.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }

        private void InstantiateInstance(T prefab, Transform parent)
        {
            T instance = Object.Instantiate(prefab, parent, true);
            m_items.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }
    }
}