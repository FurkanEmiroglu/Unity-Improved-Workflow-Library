using System.Collections.Generic;
using UnityEngine;

namespace ImprovedWorkflow.UtilClasses
{
    /// <summary>
    /// A generic object pooling implementation for MonoBehaviours.
    /// </summary>
    /// <typeparam name="T">Generic Type, must derive from a MonoBehaviour</typeparam>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        /// <summary>
        /// expanding amount if the pool size is exceeded
        /// </summary>
        private const int expand_step = 10;

        /// <summary>
        /// pooled items
        /// </summary>
        private readonly Queue<T> m_items;
        
        /// <summary>
        /// parent of the pooled items, if any
        /// </summary>
        private readonly Transform m_parent;

        /// <summary>
        /// prefab to be pooled, will use it as a template to instantiate new items
        /// </summary>
        private readonly T m_prefab;

        /// <summary>
        /// Constructs an object pool of items of type T(Prefab) with a given count
        /// </summary>
        /// <param name="prefab">Source prefab to clone</param>
        /// <param name="count">Pool size</param>
        public ObjectPool(T prefab, int count)
        {
            m_items = new Queue<T>(count);
            m_prefab = prefab;
            InitializePool(prefab, count);
        }

        /// <summary>
        /// Constructs an object pool of items of type T(Prefab) with a given count
        /// </summary>
        /// <param name="prefab">Source prefab to clone</param>
        /// <param name="count">Pool size</param>
        /// <param name="parent">instantiated clone's parent</param>
        public ObjectPool(T prefab, int count, Transform parent)
        {
            m_items = new Queue<T>(count);
            m_parent = parent;
            InitializePool(prefab, count, parent);
        }

        /// <summary>
        /// Selects an item, returns and removes it from the pool,
        /// if the pool is empty, it will expand the pool by the expand_step amount
        /// </summary>
        /// <returns>An item form the pool</returns>
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

        /// <summary>
        /// Returns an item to the pool, doesn't disables the GameObject.
        /// </summary>
        /// <param name="t">Item to return</param>
        public void Return(T t)
        {
            m_items.Enqueue(t);
        }

        /// <summary>
        /// Initializes the pool with the given prefab and count
        /// </summary>
        /// <param name="prefab">Prefab</param>
        /// <param name="count">Pool size</param>
        private void InitializePool(T prefab, int count)
        {
            for (int i = 0; i < count; i++) InstantiateInstance(prefab);
        }

        /// <summary>
        /// Initializes the pool with the given prefab and count, also pooled items will be parented to the given parent
        /// </summary>
        /// <param name="prefab">Prefab</param>
        /// <param name="count">Pool size</param>
        /// <param name="parent">Parent</param>
        private void InitializePool(T prefab, int count, Transform parent)
        {
            for (int i = 0; i < count; i++) InstantiateInstance(prefab, parent);
        }

        /// <summary>
        /// Instantiates a clone from the given prefab and adds it to the pool
        /// </summary>
        /// <param name="prefab">Prefab</param>
        private void InstantiateInstance(T prefab)
        {
            T instance = Object.Instantiate(prefab);
            m_items.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }

        /// <summary>
        /// Instantiates a clone from the given prefab and adds it to the pool, also sets the parent of the clone
        /// </summary>
        /// <param name="prefab">Prefab</param>
        /// <param name="parent">Parent</param>
        /// 
        private void InstantiateInstance(T prefab, Transform parent)
        {
            T instance = Object.Instantiate(prefab, parent, true);
            m_items.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }
    }
}