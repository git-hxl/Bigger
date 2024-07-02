
using System;
using System.Collections.Generic;

namespace FastDev
{
    public sealed partial class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private readonly Dictionary<string, ObjectPoolBase> m_ObjectPools = new Dictionary<string, ObjectPoolBase>();

        /// <summary>
        /// 获取对象池数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_ObjectPools.Count;
            }
        }

        /// <summary>
        /// 获取对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>要获取的对象池。</returns>
        public IObjectPool<T> GetObjectPool<T>(string poolName) where T : ObjectBase
        {
            return (IObjectPool<T>)InternalGetObjectPool(poolName);
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="autoReleaseInterval"></param>
        /// <param name="capacity"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, autoReleaseInterval, capacity, expireTime);
        }

        /// <summary>
        /// 销毁对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">要销毁的对象池名称。</param>
        /// <returns>是否销毁对象池成功。</returns>
        public bool DestroyObjectPool<T>(string name) where T : ObjectBase
        {
            return InternalDestroyObjectPool(name);
        }

        private ObjectPoolBase InternalGetObjectPool(string poolName)
        {
            ObjectPoolBase objectPool = null;
            if (m_ObjectPools.TryGetValue(poolName, out objectPool))
            {
                return objectPool;
            }
            return null;
        }

        private IObjectPool<T> InternalCreateObjectPool<T>(string poolName, float autoReleaseInterval, int capacity, float expireTime) where T : ObjectBase
        {
            if (InternalGetObjectPool(poolName) != null)
            {
                throw new Exception(string.Format("Already exist object pool '{0}'.", poolName));
            }

            ObjectPool<T> objectPool = new ObjectPool<T>(poolName, autoReleaseInterval, capacity, expireTime);
            m_ObjectPools.Add(poolName, objectPool);

            return objectPool;
        }

        private bool InternalDestroyObjectPool(string poolName)
        {
            ObjectPoolBase objectPool = null;
            if (m_ObjectPools.TryGetValue(poolName, out objectPool))
            {
                objectPool.Shutdown();
                return m_ObjectPools.Remove(poolName);
            }

            return false;
        }


        private void Update()
        {
            foreach (var item in m_ObjectPools)
            {
                item.Value.Update();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var item in m_ObjectPools)
            {
                item.Value.Shutdown();
            }

            m_ObjectPools.Clear();
        }
    }
}
