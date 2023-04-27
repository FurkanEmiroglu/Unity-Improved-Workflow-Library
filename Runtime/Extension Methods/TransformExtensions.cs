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
        
        #region Position Setters
        
        public static void SetPositionX(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.x = value;
            t.position = p;
        }

        public static void AddPositionX(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.x += value;
            t.position = p;
        }

        public static void SetPositionY(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.y = value;
            t.position = p;
        }
        
        public static void AddPositionY(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.y += value;
            t.position = p;
        }
        
        public static void SetPositionZ(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.z = value;
            t.position = p;
        }
        
        public static void AddPositionZ(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.z += value;
            t.position = p;
        }
        
        #endregion
    }
}