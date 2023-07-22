using UnityEngine;

namespace IW.ExtensionMethods
{
    public static class TransformExtensions
    {
        /// <summary>
        ///     Destroys every children of the given transform.
        /// </summary>
        /// <param name="t">Target Transform</param>
        public static void DestroyAllChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }

        /// <summary>
        ///     Destroys every children of the given transform IMMEDIATE. Don't use it at runtime.
        /// </summary>
        /// <param name="t">Target transform</param>
        public static void DestroyAllChildrenImmediate(this Transform t)
        {
            foreach (Transform child in t) Object.DestroyImmediate(child.gameObject);
        }

        #region Position Setters

        /// <summary>
        ///     Sets the x value of the position of the given transform.
        /// </summary>
        /// <param name="t">Target transform</param>
        /// <param name="value">new x position</param>
        public static void SetPositionX(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.x = value;
            t.position = p;
        }

        /// <summary>
        ///     Adds the x value of the position x of the given transform.
        /// </summary>
        /// <param name="t">Target transform</param>
        /// <param name="value">position.x addition amount</param>
        public static void AddPositionX(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.x += value;
            t.position = p;
        }


        /// <summary>
        ///     Sets the y value of the position of the given transform.
        /// </summary>
        /// <param name="t">Target transform</param>
        /// <param name="value">new y position</param>
        public static void SetPositionY(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.y = value;
            t.position = p;
        }


        /// <summary>
        ///     Adds the y value of the position y of the given transform.
        /// </summary>
        /// <param name="t">Target transform</param>
        /// <param name="value">position.y addition amount</param>
        public static void AddPositionY(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.y += value;
            t.position = p;
        }


        /// <summary>
        ///     Sets the z value of the position of the given transform.
        /// </summary>
        /// <param name="t">Target transform</param>
        /// <param name="value">new z position</param>
        public static void SetPositionZ(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.z = value;
            t.position = p;
        }


        /// <summary>
        ///     Adds the z value of the position z of the given transform.
        /// </summary>
        /// <param name="t">Target transform</param>
        /// <param name="value">position.z addition amount</param>
        public static void AddPositionZ(this Transform t, float value)
        {
            Vector3 p = t.position;
            p.z += value;
            t.position = p;
        }

        #endregion
    }
}