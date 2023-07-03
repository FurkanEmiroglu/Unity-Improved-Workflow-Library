using UnityEngine;

// ReSharper disable InconsistentNaming

namespace ImprovedWorkflow.CSharpExtensions
{
    public static class VectorExtensions
    {
        #region Vector2

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 xx(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.x);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 yy(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        /// <summary>
        ///     Sets the X component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="x">New x value</param>
        /// <returns>Result Vector</returns>
        public static Vector2 SetX(this ref Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        /// <summary>
        ///     Adds value to the X component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="x">addition amount</param>
        /// <returns>Result vector</returns>
        public static Vector2 AddX(this ref Vector2 vector, float x)
        {
            return new Vector2(vector.x + x, vector.y);
        }

        /// <summary>
        ///     Sets the Y component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="y">New y value</param>
        /// <returns>Result Vector</returns>
        public static Vector2 SetY(this ref Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        /// <summary>
        ///     Adds value to the Y component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="y">addition amount</param>
        /// <returns>Result vector</returns>
        public static void AddY(this ref Vector2 vector, float y)
        {
            vector.y += y;
        }

        /// <summary>
        ///     Checks if the vector is parallel with another vector
        /// </summary>
        /// <param name="vector">First vector</param>
        /// <param name="other">Other vector to check</param>
        /// <returns>Is First vector parallel with the second one</returns>
        public static bool IsParallelWith(this ref Vector2 vector, Vector2 other)
        {
            return Vector2.Dot(vector, other) > 0.9999f;
        }

        /// <summary>
        ///     Checks if the vector is perpendicular with another vector
        /// </summary>
        /// <param name="vector">First vector</param>
        /// <param name="other">Other vector to check</param>
        /// <returns>Is First vector perpendicular with the second one</returns>
        public static bool IsPerpendicularWith(this ref Vector2 vector, Vector2 other)
        {
            return Vector2.Dot(vector, other) < 0.0001f;
        }

        #endregion

        #region Vector3

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 xx(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.x);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 xy(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 xz(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 yx(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 yy(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.y);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 yz(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 zx(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.x);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 zy(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.y);
        }

        /// <summary>
        ///     Works like in hlsl
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <returns></returns>
        public static Vector2 zz(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.z);
        }

        /// <summary>
        ///     Sets the X component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="x">New x value</param>
        /// <returns>Result Vector</returns>
        public static Vector3 SetX(this ref Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        /// <summary>
        ///     Adds value to the X component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="x">addition amount</param>
        /// <returns>Result vector</returns>
        public static Vector3 AddX(this ref Vector3 vector, float x)
        {
            return new Vector3(vector.x + x, vector.y, vector.z);
        }

        /// <summary>
        ///     Sets the Y component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="y">New x value</param>
        /// <returns>Result Vector</returns>
        public static Vector3 SetY(this ref Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        /// <summary>
        ///     Adds value to the Y component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="y">addition amount</param>
        /// <returns>Result vector</returns>
        public static Vector3 AddY(this ref Vector3 vector, float y)
        {
            return new Vector3(vector.x, vector.y + y, vector.z);
        }

        /// <summary>
        ///     Sets the Z component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="z">New x value</param>
        /// <returns>Result Vector</returns>
        public static Vector3 SetZ(this ref Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        ///     Adds value to the Z component of the vector
        /// </summary>
        /// <param name="vector">Target Vector</param>
        /// <param name="z">addition amount</param>
        /// <returns>Result vector</returns>
        public static Vector3 AddZ(this ref Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, vector.z + z);
        }

        /// <summary>
        ///     Checks if the vector is parallel with another vector
        /// </summary>
        /// <param name="vector">First vector</param>
        /// <param name="other">Other vector to check</param>
        /// <returns>Is First vector parallel with the second one</returns>
        public static bool IsParallelWith(this ref Vector3 vector, Vector3 other)
        {
            return Vector3.Dot(vector, other) > 0.9999f;
        }

        /// <summary>
        ///     Checks if the vector is perpendicular with another vector
        /// </summary>
        /// <param name="vector">First vector</param>
        /// <param name="other">Other vector to check</param>
        /// <returns>Is First vector perpendicular with the second one</returns>
        public static bool IsPerpendicularWith(this ref Vector3 vector, Vector3 other)
        {
            return Vector3.Dot(vector, other) < 0.0001f;
        }

        #endregion
    }
}