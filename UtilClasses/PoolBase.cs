using System.Collections.Generic;
using UnityEngine;

namespace IW.UtilClasses
{
    public abstract class PoolBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Prefab to be pooled, will use it as a template to instantiate new items
        /// </summary>
        [SerializeField] 
        private T _objectPrefab;
        
        /// <summary>
        /// Pool size
        /// </summary>
        [SerializeField] 
        private int _initialCount;

        /// <summary>
        /// pooled items
        /// </summary>
        private readonly Queue<T> m_poolQueue = new();

        /// <summary>
        /// Initializes the pool with the serialized prefab and count
        /// </summary>
        protected void InitPool()
        {
            for (int i = 0; i < _initialCount; i++)
            {
                T obj = Instantiate(_objectPrefab, transform);
                m_poolQueue.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Can be used to re-initialize the pool with different prefab and count values
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="count"></param>
        protected void InitPool(T prefab, int count)
        {
            _objectPrefab = prefab;
            _initialCount = count;
            InitPool();
        }

        /// <summary>
        /// Selects an item, returns and removes it from the pool. CAREFUL: Doesn't expands the pool if it's empty.
        /// </summary>
        /// <returns>An item form the pool</returns>
        public T Get()
        {
            T obj = m_poolQueue.Dequeue();
            obj.gameObject.SetActive(true);
            m_poolQueue.Enqueue(obj);

            return obj;
        }

        /// <summary>
        /// Selects an item, returns and removes it from the pool, places it at the given world position.
        /// CAREFUL: Doesn't expands the pool if it's empty.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public T Get(Vector3 position)
        {
            T obj = m_poolQueue.Dequeue();
            obj.transform.position = position;
            obj.gameObject.SetActive(true);
            m_poolQueue.Enqueue(obj);

            return obj;
        }

        /// <summary>
        /// Returns an item to the pool, doesn't disables the GameObject.
        /// </summary>
        /// <param name="obj">Item to return</param>
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            m_poolQueue.Enqueue(obj);
        }
    }
}