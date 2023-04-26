using UnityEngine;

namespace ImprovedWorkflow.Extensions
{
    public static class TransformExtensions
    {
        public static void DestroyAllChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }

        public static void DestroyAllChildrenImmediate(this Transform t)
        {
            foreach (Transform child in t) Object.DestroyImmediate(child.gameObject);
        }
    }
}