using System;
using UnityEngine;
using System.Collections.Generic;

namespace UniPrep.Utils {
    public abstract class PoolBase<T> {
        protected Dictionary<T, bool> m_Pool = new Dictionary<T, bool>();

        /// <summary>
        /// Gets a free instance from the pool. Constructs a new instance if none is free
        /// </summary>
        /// <returns></returns>
        public T Get() {
            foreach (var pair in m_Pool) {
                if (pair.Value) {
                    m_Pool[pair.Key] = false;
                    return pair.Key;
                }
            }
            Add();
            return Get();
        }

        /// <summary>
        /// Adds a new instance. To be override based on whether T is a component
        /// of a normal c# class
        /// </summary>
        protected abstract void Add();

        /// <summary>
        /// Frees up the instance so that it can be retrieved by another request later
        /// </summary>
        /// <param name="obj"></param>
        public void Free(T obj) {
            if (m_Pool.ContainsKey(obj))
                m_Pool[obj] = true;
        }

        /// <summary>
        /// Removes the instance from the pool
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj) {
            if (m_Pool.ContainsKey(obj))
                m_Pool.Remove(obj);
        }
    }

    /// <summary>
    /// An instance pool of a Unity component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentPool<T> : PoolBase<T> where T : Component {               
        protected override void Add() {
            var instance = new GameObject(typeof(T).FullName).AddComponent<T>();
            m_Pool.Add(instance as T, true);
        }
    }

    /// <summary>
    /// An instance pool of a normal C# class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericPool<T> : PoolBase<T> {
        protected override void Add() {
            var def = default(T);
            var instance = Activator.CreateInstance(def.GetType());
            m_Pool.Add((T)instance, true);
        }
    }

    public class GameObjectPool : PoolBase<GameObject> {
        GameObject m_Instance;

        public GameObjectPool(GameObject instance) {
            m_Instance = instance;
        }

        protected override void Add() {
            var newInstance = MonoBehaviour.Instantiate<GameObject>(m_Instance);
            m_Pool.Add(newInstance, true);
        }
    }
}
