using UnityEngine;

namespace ScriptableObjects
{
    public class TunableObject : MonoBehaviour
    {
        [SerializeField] protected GameSettings Settings;

        [SerializeField] protected Rigidbody2D Rigidbody2D;

        protected virtual void Start()
        {
            UpdateTunables();
        }

        public virtual void UpdateTunables() { }
    }
}