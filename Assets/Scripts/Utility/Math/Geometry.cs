using UnityEngine;

namespace Scripts.Utility.Math
{
    /// <summary>
    ///     A collection of geometrical functions.
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        ///     Calculates the perimeter of a rectangle.
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <returns>Perimeter</returns>
        public static float RectanglePerimeter(float w, float h)
        {
            return 2 * (w + h);
        }

        /// <summary>
        ///     Calculates the area of a rectangle.
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <returns>Area</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static float RectangleArea(float w, float h)
        {
            return w * h;
        }

        /// <summary>
        ///     Calculates the surface area of a box.
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="d">Depth</param>
        /// <returns>Surface area</returns>
        public static float BoxSurfaceArea(float w, float h, float d)
        {
            return 2 * (RectangleArea(w, h) + RectangleArea(w, d) + RectangleArea(h, d));
        }

        /// <summary>
        ///     Calculates the volume of a box.
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="d">Depth</param>
        /// <returns>Volume</returns>
        public static float BoxVolume(float w, float h, float d)
        {
            return w * h * d;
        }

        /// <summary>
        ///     Calculates the perimeter of a triangle.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>Perimeter</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static float TrianglePerimeter(float a, float b, float c)
        {
            return a + b + c;
        }

        /// <summary>
        ///     Calculates the area of a triangle.
        /// </summary>
        /// <param name="b">Base</param>
        /// <param name="h">Height</param>
        /// <returns>Area</returns>
        public static float TriangleArea(float b, float h)
        {
            return RectangleArea(b, h) / 2;
        }

        /// <summary>
        ///     Calculates the area of a triangle using Heron's formula.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>Area</returns>
        public static float TriangleArea(float a, float b, float c)
        {
            var s = TrianglePerimeter(a, b, c) / 2;
            return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        /// <summary>
        ///     Calculates c using the Pythagorean Theorem.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>c</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static float PythagoreanTheorem(float a, float b)
        {
            return Mathf.Sqrt(Algebra.NSquared(a) + Algebra.NSquared(b));
        }

        /// <summary>
        ///     Calculates the surface area of a cone.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <param name="h">Height</param>
        /// <returns>Surface area</returns>
        public static float ConeSurfaceArea(float r, float h)
        {
            return Mathf.PI * r * (r + PythagoreanTheorem(2, h));
        }

        /// <summary>
        ///     Calculates the volume of a cone.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <param name="h">Height</param>
        /// <returns>Volume</returns>
        public static float ConeVolume(float r, float h)
        {
            return 1.0f / 3.0f * Mathf.PI * Algebra.NSquared(r) * h;
        }

        /// <summary>
        ///     Calculates the circumference of a circle.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <returns>Circumference</returns>
        public static float Circumference(float r)
        {
            return 2 * Mathf.PI * r;
        }

        /// <summary>
        ///     Calculates the area of a circle.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <returns>Area</returns>
        public static float CircleArea(float r)
        {
            return Mathf.PI * Algebra.NSquared(r);
        }

        /// <summary>
        ///     Calculates the surface area of a sphere.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <returns>Surface area</returns>
        public static float SphereSurfaceArea(float r)
        {
            return 4 * Mathf.PI * Algebra.NSquared(r);
        }

        /// <summary>
        ///     Calculates the volume of a sphere.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <returns>Volume</returns>
        public static float SphereVolume(float r)
        {
            return 4.0f / 3.0f * Mathf.PI * Algebra.NCubed(r);
        }
    }
}