using UnityEngine;

namespace ImprovedWorkflow.CSharpExtensions
{
    public static class RigidbodyExtensions
    {
        /// <summary>
        ///     Changes the direction of the rigidbody without changing the speed.
        /// </summary>
        /// <param name="rigidbody">Rigidobdy to manipulate</param>
        /// <param name="direction">New direction, must be normalized</param>
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