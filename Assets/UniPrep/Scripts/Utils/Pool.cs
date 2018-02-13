using System;
using UnityEngine;
using System.Collections.Generic;

namespace UniPrep.Utils {
    public abstract class PoolBase<T> {
        protected List<T> m_Available = new List<T>();
        protected List<T> m_Busy = new List<T>();
        
        /// <summary>
        /// Gets a free instance from the pool. Constructs a new instance if none is free
        /// </summary>
        /// <returns></returns>
        public T Get() {
            if (m_Available.Count > 0) {
                var instance = m_Available[0];
                m_Available.RemoveAt(0);
                m_Busy.Add(instance);
                return instance;
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
            var index = m_Busy.IndexOf(obj);
            if (index == -1)
                return;
            var instance = m_Busy[index];
            m_Available.Add(instance);
            m_Busy.RemoveAt(index);
        }

        /// <summary>
        /// Removes the instance from the pool
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj) {
            var index = m_Available.IndexOf(obj);
            if(index != 0) 
                m_Available.RemoveAt(index);
        }
    }

    /// <summary>
    /// An instance pool of a Unity component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentPool<T> : PoolBase<T> where T : Component {
        GameObjectPool m_Pool = new GameObjectPool(new GameObject("ComponentPoolContainer"));
    
        protected override void Add() {
            var container = m_Pool.Get();
            container.name = typeof(T).FullName;
            var newInstance = container.AddComponent<T>();
            m_Available.Add(newInstance);
        }
    }

    /// <summary>
    /// An instance pool of a normal C# class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericPool<T> : PoolBase<T> {
        protected override void Add() {
            var def = default(T);
            var newInstance = Activator.CreateInstance(def.GetType());
            m_Available.Add((T)newInstance);
        }
    }

    /// <summary>
    /// A Gameobject instance pool. Eg. Pool of bullet shells
    /// </summary>
    public class GameObjectPool : PoolBase<GameObject> {
        GameObject m_Instance;

        /// <summary>
        /// Constructs a gameobject pool with the given instance
        /// </summary>
        /// <param name="instance"></param>
        public GameObjectPool(GameObject instance) {
            m_Instance = instance;
        }

        /// <summary>
        /// Instantiates a copy of <see cref="m_Instance"/> and adds it to the pool
        /// </summary>
        protected override void Add() {
            var newInstance = MonoBehaviour.Instantiate<GameObject>(m_Instance);
            m_Available.Add(newInstance);
        }
    }
}
