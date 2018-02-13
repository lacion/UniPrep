using System;
using UnityEngine;

namespace UniPrep.Utils {
    public class Monitor : MonoBehaviour {
        // Update
        Action m_OnUpdate;
        public void HandleUpdate(Action callback) {
            m_OnUpdate = callback;
        }
        void Update() {
            if (m_OnUpdate != null) m_OnUpdate();
        }

        // FixedUpdate
        Action m_OnFixedUpdate;
        public void HandleFixedUpdate(Action callback) {
            m_OnFixedUpdate = callback;
        }
        void FixedUpdate() {
            if (m_OnFixedUpdate != null) m_OnFixedUpdate();
        }

        // OnTriggerEnter
        Action<Collider> m_OnTriggerEnter;
        public void HandleTriggerEnter(Action<Collider> callback) {
            m_OnTriggerEnter = callback;
        }
        void OnTriggerEnter(Collider collider) {
            if(m_OnTriggerEnter!= null )m_OnTriggerEnter(collider);
        }

        // OnCOllisionEnter
        Action<Collision> m_OnCollisionEnter;
        public void HandleCollisionEnter(Action<Collision> callback) {
            m_OnCollisionEnter = callback;
        }
        void OnCollisionEnter(Collision collision) {
            if (m_OnCollisionEnter != null) m_OnCollisionEnter(collision);
        }
    }

    public static class MonitorExtensions {
        public static Monitor AddMonitor(this GameObject gameObject) {
            var monitor = gameObject.GetComponent<Monitor>();
            if (monitor == null)
                monitor = gameObject.AddComponent<Monitor>();
            return monitor;
        }
    }
}
