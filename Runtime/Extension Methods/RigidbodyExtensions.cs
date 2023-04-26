using UnityEngine;

namespace ImprovedWorkflow.Extensions
{
    public static class RigidbodyExtensions
    {
        public static void ChangeDirection(this Rigidbody rigidbody, Vector3 direction)
        {
            if (direction.magnitude > 1)
            {
                Debug.LogWarning("Direction must be normalized.");
                direction.Normalize();
            }

            rigidbody.velocity = direction * rigidbody.velocity.magnitude;
        }
    }
}