using System;
using UnityEngine;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    public class CollisionDetector : MonoBehaviour
    {
        public void OnCollisionEnter(Collision other)
        {
            CollisionEntered?.Invoke(other);
        }

        public void OnCollisionExit(Collision other)
        {
            CollisionExited?.Invoke(other);
        }

        public void OnCollisionStay(Collision other)
        {
            CollisionStayed?.Invoke(other);
        }

        public void OnTriggerEnter(Collider other)
        {
            TriggerEntered?.Invoke(other);
        }

        public void OnTriggerExit(Collider other)
        {
            TriggerExited?.Invoke(other);
        }

        public void OnTriggerStay(Collider other)
        {
            TriggerStayed?.Invoke(other);
        }

        public event Action<Collider> TriggerEntered;

        public event Action<Collider> TriggerStayed;

        public event Action<Collider> TriggerExited;

        public event Action<Collision> CollisionEntered;

        public event Action<Collision> CollisionStayed;

        public event Action<Collision> CollisionExited;
    }
}