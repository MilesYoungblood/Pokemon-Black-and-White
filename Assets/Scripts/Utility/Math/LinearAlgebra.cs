using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Scripts.Utility.Math
{
    /// <summary>
    /// A collection of linear algebra functions.
    /// </summary>
    public static class LinearAlgebra
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Mod(this Vector2 v, float m)
        {
            return new Vector2(v.x % m, v.y % m);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Mod(this Vector3 v, float m)
        {
            return new Vector3(v.x % m, v.y % m, v.z % m);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Mod(this Vector4 v, float m)
        {
            return new Vector4(v.x % m, v.y % m, v.z % m, v.w % m);
        }

        /// <summary>
        /// Enumerates over the Vector2's components
        /// </summary>
        /// <param name="v"></param>
        /// <returns>the x, then y component</returns>
        public static IEnumerable<float> GetComponents(this Vector2 v)
        {
            yield return v.x;
            yield return v.y;
        }

        /// <summary>
        /// Enumerates over the Vector3's components
        /// </summary>
        /// <param name="v"></param>
        /// <returns>the x, then y, then z component</returns>
        public static IEnumerable<float> GetComponents(this Vector3 v)
        {
            yield return v.x;
            yield return v.y;
            yield return v.z;
        }

        /// <summary>
        /// Rounds the Vector2's components to the nearest integer
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The rounded vector</returns>
        public static Vector2 Round(this Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        }

        /// <summary>
        /// Rounds the Vector3's components to the nearest integer
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The rounded vector</returns>
        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        /// <summary>
        /// Rounds the Vector4's components to the nearest integer
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The rounded vector</returns>
        public static Vector4 Round(this Vector4 v)
        {
            return new Vector4(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z), Mathf.Round(v.w));
        }

        /// <summary>
        /// Floors the Vector2's components
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The floored vector</returns>
        public static Vector2 Floor(this Vector2 v)
        {
            return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
        }

        /// <summary>
        /// Floors the Vector3's components
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>The floored vector</returns>
        public static Vector3 Floor(this Vector3 vector)
        {
            return new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));
        }

        /// <summary>
        /// Clamps the Vector2's components, to the same min and max values for each component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>The clamped vector</returns>
        public static Vector2 Clamp(this Vector2 v, float min, float max)
        {
            return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        /// <summary>
        /// Clamps the Vector2's components to either the X or Y axis
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>The clamped vector</returns>
        public static Vector2 ClampToAxis(this Vector2 vector)
        {
            if (Mathf.Abs(vector.x) > Mathf.Epsilon)
            {
                vector.y = 0;
            }
            else if (Mathf.Abs(vector.y) > Mathf.Epsilon)
            {
                vector.x = 0;
            }

            return vector;
        }

        public static float ManhattanDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float ManhattanDistance(Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        public static float ManhattanDistance(Vector4 a, Vector4 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z) + Mathf.Abs(a.w - b.w);
        }

        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }
}
