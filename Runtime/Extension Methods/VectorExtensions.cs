using UnityEngine;
// ReSharper disable InconsistentNaming

namespace ImprovedWorkflow.Extensions
{
    public static class VectorExtensions
    {
        #region Vector2
        
        public static Vector2 xx(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.x);
        }
        
        public static Vector2 yy(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
        
        public static Vector2 SetX(this ref Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 AddX(this ref Vector2 vector, float x)
        {
            return new Vector2(vector.x + x, vector.y);
        }
        
        public static Vector2 SetY(this ref Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static void AddY(this ref Vector2 vector, float y)
        {
            vector.y += y;
        }
        
        public static bool IsParallelWith(this ref Vector2 vector, Vector2 other)
        {
            return Vector2.Dot(vector, other) > 0.9999f;
        }
        
        public static bool IsPerpendicularWith(this ref Vector2 vector, Vector2 other)
        {
            return Vector2.Dot(vector, other) < 0.0001f;
        }

        #endregion

        #region Vector3

        public static Vector2 xx(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.x);
        }
        
        public static Vector2 xy(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
        
        public static Vector2 xz(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
        
        public static Vector2 yx(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.x);
        }
        
        public static Vector2 yy(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.y);
        }
        
        public static Vector2 yz(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }
        
        public static Vector2 zx(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.x);
        }
        
        public static Vector2 zy(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.y);
        }
        
        public static Vector2 zz(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.z);
        }
        
        public static Vector3 SetX(this ref Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 AddX(this ref Vector3 vector, float x)
        {
            return new Vector3(vector.x + x, vector.y, vector.z);
        }

        public static Vector3 SetY(this ref Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 AddY(this ref Vector3 vector, float y)
        {
            return new Vector3(vector.x, vector.y + y, vector.z);
        }

        public static Vector3 SetZ(this ref Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 AddZ(this ref Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, vector.z + z);
        }

        public static bool IsParallelWith(this ref Vector3 vector, Vector3 other)
        {
            return Vector3.Dot(vector, other) > 0.9999f;
        }

        public static bool IsPerpendicularWith(this ref Vector3 vector, Vector3 other)
        {
            return Vector3.Dot(vector, other) < 0.0001f;
        }

        #endregion
    }
}