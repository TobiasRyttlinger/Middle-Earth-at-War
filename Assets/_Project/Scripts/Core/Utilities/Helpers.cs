using UnityEngine;

namespace BFME2.Core
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns the vector with Y set to 0 (flatten to XZ ground plane).
        /// </summary>
        public static Vector3 Flat(this Vector3 v)
        {
            return new Vector3(v.x, 0f, v.z);
        }

        /// <summary>
        /// Returns the distance between two points ignoring the Y axis.
        /// </summary>
        public static float FlatDistance(this Vector3 a, Vector3 b)
        {
            var dx = a.x - b.x;
            var dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>
        /// Returns the squared distance between two points ignoring the Y axis.
        /// Useful for distance comparisons without the sqrt cost.
        /// </summary>
        public static float FlatDistanceSqr(this Vector3 a, Vector3 b)
        {
            var dx = a.x - b.x;
            var dz = a.z - b.z;
            return dx * dx + dz * dz;
        }

        /// <summary>
        /// Converts a Vector2 (screen/UI) to a Vector3 on the XZ plane.
        /// </summary>
        public static Vector3 ToXZ(this Vector2 v, float y = 0f)
        {
            return new Vector3(v.x, y, v.y);
        }
    }

    public static class MathHelpers
    {
        /// <summary>
        /// Remaps a value from one range to another.
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = Mathf.InverseLerp(fromMin, fromMax, value);
            return Mathf.Lerp(toMin, toMax, t);
        }

        /// <summary>
        /// Returns a point on a circle of given radius at the specified angle (radians).
        /// </summary>
        public static Vector3 PointOnCircle(float radius, float angleRadians)
        {
            return new Vector3(
                Mathf.Cos(angleRadians) * radius,
                0f,
                Mathf.Sin(angleRadians) * radius
            );
        }
    }

    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Checks if a layer mask contains the specified layer index.
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
