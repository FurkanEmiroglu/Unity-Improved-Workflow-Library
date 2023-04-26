using UnityEngine;

namespace ImprovedWorkflow.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 SetX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 AddX(this Vector2 vector, float x)
        {
            return new Vector2(vector.x + x, vector.y);
        }

        public static Vector3 SetX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 AddX(this Vector3 vector, float x)
        {
            return new Vector3(vector.x + x, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, vector.y + y, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 AddZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, vector.z + z);
        }

        public static bool IsParallelWith(this Vector3 vector, Vector3 other)
        {
            return Vector3.Dot(vector, other) > 0.9999f;
        }

        public static bool IsPerpendicularWith(this Vector3 vector, Vector3 other)
        {
            return Vector3.Dot(vector, other) < 0.0001f;
        }
    }
}