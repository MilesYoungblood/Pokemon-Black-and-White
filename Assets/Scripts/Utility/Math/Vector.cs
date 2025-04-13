using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Utility.Math
{
    /// <summary>
    ///     A vector of size n.
    /// </summary>
    [Serializable]
    public readonly struct Vector : IEquatable<Vector>, IEnumerable<float>, IEnumerable<(int, float)>
    {
        /// <summary>
        /// The internal vector components.
        /// </summary>
        private readonly float[] _components;

        /// <summary>
        /// The dimension of the vector.
        /// </summary>
        public int Dimension { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Vector(params float[] components) : this(components.Length)
        {
            Array.Copy(components, _components, Dimension);
        }

        /// <summary>
        /// A vector of size <paramref name="n" />.
        /// </summary>
        /// <param name="n">The size.</param>
        public Vector(int n)
        {
            _components = new float[n];
            Dimension = n;
        }

        /// <summary>
        /// Vector indexer.
        /// </summary>
        /// <param name="i">The index.</param>
        public float this[int i]
        {
            get => _components[i];
            set => _components[i] = value;
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="v">Vector <paramref name="v" />.</param>
        /// <param name="u">Vector <paramref name="u" />.</param>
        /// <returns>The dot product.</returns>
        /// <exception cref="ArgumentException">Thrown if the vectors are incompatible.</exception>
        public static float Dot(Vector v, Vector u)
        {
            if (v.Dimension != u.Dimension)
            {
                throw new ArgumentException("Vectors must be of the same dimension.");
            }

            float result = 0;
            for (var i = 0; i < v.Dimension; i++)
            {
                result += v[i] * u[i];
            }

            return result;
        }

        /// <summary>
        /// Calculates the cross-product of two vectors.
        /// </summary>
        /// <param name="v">Vector <paramref name="v" />.</param>
        /// <param name="u">Vector <paramref name="u" />.</param>
        /// <returns>The cross-product.</returns>
        /// <exception cref="ArgumentException">Thrown if the vectors' dimension isn't 3.</exception>
        public static Vector Cross(Vector v, Vector u)
        {
            if (v.Dimension != 3 || u.Dimension != 3)
            {
                throw new ArgumentException("Cross product is defined only for 3D vectors.");
            }

            return new Vector(v[1] * u[2] - v[2] * u[1], v[2] * u[0] - v[0] * u[2], v[0] * u[1] - v[1] * u[0]);
        }

        /// <summary>
        /// Calculates the "magnitude" of the vector.
        /// </summary>
        /// <returns>The "magnitude" of the vector.</returns>
        public float Magnitude()
        {
            return Mathf.Sqrt(Dot(this, this));
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <returns>The normalized vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vector is a zero vector.</exception>
        public Vector Normalize()
        {
            var magnitude = Magnitude();
            if (magnitude == 0)
            {
                throw new InvalidOperationException("Cannot normalize a zero vector.");
            }

            return this / magnitude;
        }

        /// <summary>
        /// Calculates the vector between two vectors.
        /// </summary>
        /// <param name="a">Vector <paramref name="a" />.</param>
        /// <param name="b">Vector <paramref name="b" />.</param>
        /// <returns>The angle between <paramref name="a" /> and <paramref name="b" />.</returns>
        public static float Angle(Vector a, Vector b)
        {
            return Mathf.Acos(Dot(a, b) / (a.Magnitude() * b.Magnitude()));
        }

        // Projection of vector A onto vector B
        public static Vector Project(Vector a, Vector b)
        {
            var bNorm = b.Normalize();
            return bNorm * Dot(a, bNorm);
        }

        // Distance between vectors
        public static float Distance(Vector a, Vector b)
        {
            return (a - b).Magnitude();
        }

        // Linear interpolation
        public static Vector Lerp(Vector a, Vector b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vector operator -(Vector a)
        {
            return a * -1;
        }

        // Addition
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("Vectors must be of the same dimension.");
            }

            var result = new Vector(a.Dimension);
            for (var i = 0; i < result.Dimension; ++i)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        // Subtraction
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("Vectors must be of the same dimension.");
            }

            return a + -b;
        }

        // Scalar multiplication
        public static Vector operator *(Vector a, float scalar)
        {
            var result = new Vector(a.Dimension);
            for (var i = 0; i < a.Dimension; ++i)
            {
                result[i] = a[i] * scalar;
            }

            return result;
        }

        // Scalar division
        public static Vector operator /(Vector a, float scalar)
        {
            if (Numerics.IsZero(scalar))
            {
                throw new DivideByZeroException("Scalar cannot be zero.");
            }

            return a * (1 / scalar);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public bool Equals(Vector other)
        {
            if (Dimension != other.Dimension)
            {
                return false;
            }

            for (var i = 0; i < Dimension; i++)
            {
                if (!Mathf.Approximately(_components[i], other._components[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerator<float> GetEnumerator()
        {
            for (var i = 0; i < Dimension; ++i)
            {
                yield return _components[i];
            }
        }

        IEnumerator<(int, float)> IEnumerable<(int, float)>.GetEnumerator()
        {
            for (var i = 0; i < Dimension; ++i)
            {
                yield return (i, _components[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is Vector other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_components, Dimension);
        }
    }
}
