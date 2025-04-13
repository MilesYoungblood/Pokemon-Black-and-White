using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Scripts.Utility.Math
{
    /// <summary>
    /// An m×n matrix.
    /// </summary>
    [Serializable]
    public readonly struct Matrix : IEquatable<Matrix>, IEnumerable<float>
    {
        #region Fields

        /// <summary>
        /// The internal matrix data.
        /// </summary>
        private readonly float[] _entries;

        /// <summary>
        /// The number of rows.
        /// </summary>
        public int M { get; }

        /// <summary>
        /// The number of columns.
        /// </summary>
        public int N { get; }

        /// <summary>
        /// The minimum dimension.
        /// </summary>
        public int MinDimension { get; }

        #endregion

        #region Properties

        /// <summary>
        /// Whether a matrix is "infinite".
        /// </summary>
        /// <remarks>Since arrays cannot be infinite, we can represent int.MaxValue as infinity.</remarks>
        private bool IsInfinite => M == int.MaxValue || N == int.MaxValue;

        /// <summary>
        /// Whether a matrix classifies as a row vector.
        /// </summary>
        private bool IsRowVector => N == 1;

        /// <summary>
        /// Whether a matrix classifies as a column vector.
        /// </summary>
        private bool IsColumnVector => M == 1;

        /// <summary>
        /// Whether a matrix is square.
        /// </summary>
        private bool IsSquare => M == N;

        /// <summary>
        /// Whether a square matrix is invertible or singular.
        /// </summary>
        /// <value>det(A) != 0</value>
        private bool IsInvertible => IsSquare && !Numerics.IsZero(Determinant());

        /// <summary>
        /// Whether a matrix is symmetric.
        /// </summary>
        /// <value>A = A^T</value>
        private bool IsSymmetric => Equals(Transpose());

        /// <summary>
        /// Whether a matrix is skew-symmetric.
        /// </summary>
        /// <value>A = -A^T</value>
        private bool IsSkewSymmetric => Equals(-Transpose());

        /// <summary>
        /// Whether a matrix is positive-definite.
        /// </summary>
        private bool IsPositiveDefinite => IsSymmetric && FindEigenvalues().All(eigenvalue => !float.IsNegative(eigenvalue));

        /// <summary>
        /// Whether a matrix is negative-definite.
        /// </summary>
        private bool IsNegativeDefinite => IsSymmetric && FindEigenvalues().All(float.IsNegative);

        /// <summary>
        /// Whether a matrix is indefinite.
        /// </summary>
        private bool IsIndefinite => IsInvertible && Numerics.ContainsMixedSigns(FindEigenvalues());

        /// <summary>
        /// Whether a matrix is orthogonal.
        /// </summary>
        /// <value>A^-1 = A^T</value>
        private bool IsOrthogonal => Inverse() == Transpose();

        #endregion

        #region Indexers

        /// <summary>
        /// Matrix indexer.
        /// </summary>
        /// <param name="i">The row.</param>
        /// <param name="j">The column.</param>
        private float this[int i, int j]
        {
            get => _entries[i * N + j];
            set => _entries[i * N + j] = value;
        }

        #endregion

        #region Methods

        #region Constructors

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="b">The matrix to copy.</param>
        public Matrix(Matrix b) : this(b.M, b.N)
        {
            Array.Copy(b._entries, _entries, _entries.Length);
        }

        /// <summary>
        /// Constructs a matrix from a multi-dimensional array.
        /// </summary>
        /// <param name="entries"></param>
        public Matrix(float[,] entries) : this(entries.GetLength(0), entries.GetLength(1))
        {
            Array.Copy(entries, _entries, _entries.Length);
        }

        /// <summary>
        /// Constructs an <paramref name="m" />×<paramref name="n" /> <see cref="Matrix" />.
        /// </summary>
        /// <param name="m">Number of Rows.</param>
        /// <param name="n">Number of columns.</param>
        public Matrix(int m, int n)
        {
            M = m;
            N = n;
            MinDimension = Mathf.Min(M, N);
            _entries = new float[M * N];
        }

        /// <summary>
        /// Constructs a square matrix.
        /// </summary>
        /// <param name="n">Size of the matrix.</param>
        public Matrix(int n) : this(n, n)
        {
        }

        #endregion

        #region Factories

        /// <summary>
        /// Constructs a duplicate of the matrix.
        /// </summary>
        /// <returns>A deep copy of the matrix.</returns>
        [Pure]
        private Matrix DeepCopy()
        {
            return new Matrix(this);
        }

        /// <summary>
        /// Generates a randomized <paramref name="m" />×<paramref name="n" /> matrix.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>A randomly generated <paramref name="m" />×<paramref name="n" /> matrix.</returns>
        /// <remarks>Time complexity: O(m * n)</remarks>
        [Pure]
        public static Matrix RandomMatrix(int m, int n)
        {
            var random = new Random((uint)DateTime.Now.Ticks);
            var matrix = new Matrix(m, n);
            for (var i = 0; i < m; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    matrix[i, j] = random.NextFloat();
                }
            }

            return matrix;
        }

        /// <summary>
        /// Generates a randomized <paramref name="n" />×<paramref name="n" /> matrix.
        /// </summary>
        /// <param name="n">Matrix size.</param>
        /// <returns>A randomly generated <paramref name="n" />×<paramref name="n" /> matrix.</returns>
        /// <remarks>Time complexity: O(n^2)</remarks>
        [Pure]
        public static Matrix RandomMatrix(int n)
        {
            return RandomMatrix(n, n);
        }

        #endregion

        #region Basic Operations

        /// <summary>
        /// Gets the transpose of the matrix.
        /// </summary>
        /// <returns>A^T</returns>
        [Pure]
        private Matrix Transpose()
        {
            var transpose = new Matrix(N, M);
            for (var i = 0; i < M; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    transpose[j, i] = this[i, j];
                }
            }

            return transpose;
        }

        /// <summary>
        /// Constructs a submatrix of the matrix.
        /// </summary>
        /// <param name="rows">The rows to exclude.</param>
        /// <param name="columns">The columns to exclude.</param>
        /// <returns>A submatrix of the matrix.</returns>
        /// <remarks>Time complexity: O(n^2)</remarks>
        [Pure]
        private Matrix Submatrix(HashSet<int> rows, HashSet<int> columns)
        {
            var submatrix = new Matrix(M - rows.Count, N - columns.Count);
            for (int i = 0, mI = 0; i < M; ++i)
            {
                // exclude the row that is equal to i
                if (rows.Contains(i))
                {
                    continue;
                }

                for (int j = 0, mJ = 0; j < N; ++j)
                {
                    // exclude the column that is equal to j
                    if (columns.Contains(j))
                    {
                        continue;
                    }

                    submatrix[mI, mJ] = this[i, j];
                    ++mJ;
                }

                ++mI;
            }

            return submatrix;
        }

        /// <summary>
        /// Constructs a principal submatrix of the matrix.
        /// </summary>
        /// <param name="indices">The set of indices to keep.</param>
        /// <returns>A principal submatrix of the matrix.</returns>
        /// <remarks>Time complexity: O(n^2)</remarks>
        [Pure]
        public Matrix PrincipalSubmatrix(HashSet<int> indices)
        {
            return Submatrix(
                new HashSet<int>(Enumerable.Range(0, M).Except(indices)),
                new HashSet<int>(Enumerable.Range(0, N).Except(indices))
            );
        }

        #endregion

        #region Diagonals

        /// <summary>
        /// Gets the main diagonal of the matrix.
        /// </summary>
        /// <returns>{ A[1,1], A[2,2], ..., A[min(m,n),min(m,n)] }</returns>
        private float[] MainDiagonal()
        {
            var mainDiagonal = new float[MinDimension];
            for (var i = 0; i < mainDiagonal.Length; ++i)
            {
                mainDiagonal[i] = this[i, i];
            }

            return mainDiagonal;
        }

        /// <summary>
        /// Gets the antidiagonal of the square matrix.
        /// </summary>
        /// <returns>{ A[1,n-1], A[2,n-2], ..., A[n,0] }</returns>
        private float[] Antidiagonal()
        {
            if (!IsSquare)
            {
                return Array.Empty<float>();
            }

            var antidiagonal = new float[N];
            for (var i = 0; i < antidiagonal.Length; ++i)
            {
                antidiagonal[i] = this[i, N - 1 - i];
            }

            return antidiagonal;
        }

        #endregion

        #region Square Matrices

        #region Forms

        /// <summary>
        /// Constructs the identity matrix of size <paramref name="n" />.
        /// </summary>
        /// <param name="n">Size of the identity matrix.</param>
        /// <returns>I</returns>
        /// <remarks>Time complexity: O(n)</remarks>
        [Pure]
        private static Matrix Identity(int n)
        {
            var identity = new Matrix(n);
            for (var i = 0; i < n; ++i)
            {
                identity[i, i] = 1;
            }

            return identity;
        }

        /// <summary>
        /// Gets the inverse of the matrix.
        /// </summary>
        /// <returns>If invertible, A^-1, and if singular, an empty matrix.</returns>
        [Pure]
        private Matrix Inverse()
        {
            try
            {
                return 1 / Determinant() * Adjugate();
            }
            catch (InvalidOperationException)
            {
                return new Matrix();
            }
        }

        /// <summary>
        /// Gets the adjugate of the square matrix.
        /// </summary>
        /// <value>C^T</value>
        /// <returns>adj(A)</returns>
        /// <remarks>Time complexity: O(n^2).</remarks>
        [Pure]
        private Matrix Adjugate()
        {
            if (!IsSquare)
            {
                return new Matrix();
            }

            // We could use Cofactor().Transpose(), however both functions have a time complexity of O(n^2),
            // so we end up with 2n^2 operations.
            var adjugate = new Matrix(N);
            for (var i = 0; i < N; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    // Since the adjugate is the cofactor matrix but transposed,
                    // it's required that we index by [j,i] and not [i,j] like usual.
                    adjugate[j, i] = Cofactor(i, j);
                }
            }

            return adjugate;
        }

        /// <summary>
        /// Gets the cofactor matrix of the square matrix.
        /// </summary>
        /// <returns>C</returns>
        /// <remarks>Time complexity: O(n * (n^2 * (n - 1)!))</remarks>
        [Pure]
        private Matrix Cofactor()
        {
            var cofactor = new Matrix(N);
            for (var i = 0; i < N; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    cofactor[i, j] = Cofactor(i, j);
                }
            }

            return cofactor;
        }

        #endregion

        #region Invariants

        /// <summary>
        /// Gets the trace of the square matrix.
        /// </summary>
        /// <value>A[1,1] + A[2,2] + ... + A[n,n]</value>
        /// <returns>tr(A)</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is not square.</exception>
        /// <remarks>Time complexity: O(n)</remarks>
        [Pure]
        public float Trace()
        {
            if (!IsSquare)
            {
                throw new InvalidOperationException("Matrix must be square to calculate trace.");
            }

            var trace = 0.0f;
            for (var i = 0; i < N; ++i)
            {
                trace += this[i, i];
            }

            return trace;
        }

        #region Determinants

        /// <summary>
        /// Calculates the determinant of the matrix.
        /// </summary>
        /// <returns>det(A)</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is not a square matrix.</exception>
        /// <remarks>
        /// Time Complexity:
        /// <para>- O(1) for n = 1, 2, 3</para>
        /// <para>- O(n!) otherwise</para>
        /// </remarks>
        [Pure]
        private float Determinant()
        {
            if (!IsSquare)
            {
                throw new InvalidOperationException("Matrix must be square to calculate determinant.");
            }

            switch (N)
            {
                case 1:
                    // det(A) = A[0,0]
                    return this[0, 0];
                case 2:
                    // det(A) = ad - bc
                    return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
                case 3:
                    // rule of Sarrus
                    // det(A) = aei + bfg + cdh - ceg - bdi - afh
                    var a = this[0, 0];
                    var b = this[0, 1];
                    var c = this[0, 2];
                    var d = this[1, 0];
                    var e = this[1, 1];
                    var f = this[1, 2];
                    var g = this[2, 0];
                    var h = this[2, 1];
                    var i = this[2, 2];
                    return a * e * i + b * f * g + c * d * h - c * e * g - b * d * i - a * f * h;
                default:
                    // Laplace expansion
                    // det(A) = A[1,j] * M[1,j] + A[2,j] * M[2,j] + ... + A[n,j] * M[n,j]
                    float det = 0;
                    for (var j = 0; j < N; ++j)
                    {
                        det += this[0, j] * Cofactor(0, j);
                    }

                    return det;
            }
        }

        /// <summary>
        /// Gets the cofactor of the square matrix.
        /// </summary>
        /// <value>(-1)^(<paramref name="i" /> + <paramref name="j" />) * M[<paramref name="i" />,<paramref name="j" />]</value>
        /// <param name="i">The row.</param>
        /// <param name="j">The column.</param>
        /// <returns>C[<paramref name="i" />,<paramref name="j" />]</returns>
        /// <remarks>Time complexity: O(n^2 * (n - 1)!)</remarks>
        [Pure]
        private float Cofactor(int i, int j)
        {
            return Numerics.GetSignByParity(i + j) * FirstMinor(i, j);
        }

        /// <summary>
        /// Calculates the minor of the square matrix.
        /// </summary>
        /// <param name="rows">The rows to exclude.</param>
        /// <param name="columns">The columns to exclude.</param>
        /// <returns>The minor of the matrix.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is not square.</exception>
        /// <remarks>Time complexity: O(n^2 * (n - 1)!)</remarks>
        [Pure]
        private float Minor(HashSet<int> rows, HashSet<int> columns)
        {
            if (!IsSquare)
            {
                throw new InvalidOperationException("Matrix must be square to calculate minor.");
            }

            return Submatrix(rows, columns).Determinant();
        }

        /// <summary>
        /// Gets the first minor of the square matrix.
        /// </summary>
        /// <param name="i">The row.</param>
        /// <param name="j">The column.</param>
        /// <returns>M[<paramref name="i" />,<paramref name="j" />]</returns>
        /// <remarks>Time complexity: O(n^2 * (n - 1)!)</remarks>
        [Pure]
        private float FirstMinor(int i, int j)
        {
            return Minor(new HashSet<int> { i }, new HashSet<int> { j });
        }

        #endregion

        #region Eigenvalues and Eigenvectors

        /// <summary>
        /// Finds all eigenvalues of the matrix using the QR algorithm.
        /// </summary>
        /// <returns>The set of eigenvalues.</returns>
        [Pure]
        private float[] FindEigenvalues(int iterations = 1000)
        {
            if (!IsSquare)
            {
                throw new InvalidOperationException("Matrix must be square to calculate eigenvalues.");
            }

            var a = DeepCopy();
            var eigenvalues = new float[N];

            for (var iter = 0; iter < iterations; ++iter)
            {
                // Perform QR decomposition
                var (q, r) = a.QRDecomposition();

                // Update A
                a = r * q;

                // Check for convergence
                var converged = true;
                for (var i = 0; i < N; ++i)
                {
                    if (Mathf.Abs(a[i, i] - eigenvalues[i]) <= Numerics.ConvergenceTolerance)
                    {
                        continue;
                    }

                    converged = false;
                    break;
                }

                // Update eigenvalues
                for (var i = 0; i < N; ++i)
                {
                    eigenvalues[i] = a[i, i];
                }

                if (converged)
                {
                    break;
                }
            }

            return eigenvalues;
        }

        /// <summary>
        /// Computes the eigenvectors for the matrix.
        /// </summary>
        /// <returns>The set of eigenvectors corresponding to the eigenvalues.</returns>
        [Pure]
        public float[,] ComputeEigenvectors()
        {
            var eigenvalues = FindEigenvalues();
            var eigenvectors = new float[N, N];

            for (var k = 0; k < N; ++k)
            {
                var eigenvector = SolveForEigenvector(eigenvalues[k]);
                for (var i = 0; i < N; ++i)
                {
                    eigenvectors[k, i] = eigenvector[i];
                }
            }

            return eigenvectors;
        }

        /// <summary>
        /// Solves the linear system (A - <paramref name="λ" />I) x = 0 for the given eigenvalue <paramref name="λ" />.
        /// </summary>
        /// <param name="λ">The eigenvalue.</param>
        /// <returns>The corresponding eigenvector x.</returns>
        [Pure]
        private Vector SolveForEigenvector(float λ)
        {
            var a = DeepCopy();

            // Subtract λI from A where i = j
            for (var i = 0; i < N; ++i)
            {
                a[i, i] -= λ;
            }

            //(DeepCopy() - λ * Identity(_n)).SolveHomogeneousSystem();

            return a.SolveHomogeneousSystem();
        }

        #endregion

        #endregion

        #region Decomposition

        /// <summary>
        /// Performs the LU decomposition of the matrix.
        /// </summary>
        /// <returns>The lower triangular and upper triangular matrix.</returns>
        public (Matrix, Matrix) LUDecomposition()
        {
            if (!IsSquare)
            {
                return (new Matrix(), new Matrix());
            }

            var l = new Matrix(N);
            var u = DeepCopy();

            for (var i = 0; i < N; ++i)
            {
                l[i, i] = 1;
                for (var j = i; j < N; ++j)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        u[i, j] -= l[i, k] * u[k, j];
                    }
                }

                for (var j = i + 1; j < N; ++j)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        l[j, i] -= l[j, k] * u[k, i];
                    }

                    l[j, i] /= u[i, i];
                }
            }

            return (l, u);
        }

        public Matrix CholeskyDecomposition()
        {
            if (!IsSquare)
            {
                return new Matrix();
            }

            var n = N;
            var cholesky = new Matrix(n, n);

            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j <= i; ++j)
                {
                    float sum = 0;
                    for (var k = 0; k < j; ++k)
                    {
                        sum += cholesky[i, k] * cholesky[j, k];
                    }

                    if (i == j)
                    {
                        cholesky[i, j] = Mathf.Sqrt(this[i, i] - sum);
                        if (cholesky[i, j] <= 0)
                        {
                            // matrix is not positive definitive
                            return new Matrix();
                        }
                    }
                    else
                    {
                        cholesky[i, j] = 1.0f / cholesky[j, j] * (this[i, j] - sum);
                    }
                }
            }

            return cholesky;
        }

        #endregion

        #endregion

        #region Vector Spaces

        /// <summary>
        /// Checks whether a matrix is full rank.
        /// </summary>
        /// <returns>True if the matrix is full rank and false otherwise.</returns>
        [Pure]
        private bool IsFullRank()
        {
            return Rank() == MinDimension;
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <returns>rank(A)</returns>
        [Pure]
        private int Rank()
        {
            return PivotPositions().Count;
        }

        /// <summary>
        /// Gets the nullity of the matrix.
        /// </summary>
        /// <returns>null(A)</returns>
        [Pure]
        private int Nullity()
        {
            return N - Rank();
        }

        /// <summary>
        /// Gets the row space of the matrix.
        /// </summary>
        /// <returns>C(A^T)</returns>
        [Pure]
        public float[,] RowSpace()
        {
            return VectorBasis(true);
        }

        /// <summary>
        /// Gets the column space of the matrix.
        /// </summary>
        /// <returns>C(A)</returns>
        [Pure]
        private float[,] ColumnSpace()
        {
            return VectorBasis(false);
        }

        /// <summary>
        /// Helper function to reduce code duplication for getting row and column spaces.
        /// </summary>
        /// <param name="forRowSpace">True for row space, false for column space.</param>
        /// <returns>Either the row or column space</returns>
        [Pure]
        private float[,] VectorBasis(bool forRowSpace)
        {
            var rRefMatrix = ReducedRowEchelonForm();
            var pivotIndices = forRowSpace ? rRefMatrix.PivotRows() : rRefMatrix.PivotColumns();

            var dimension = forRowSpace ? N : M;
            var space = new float[pivotIndices.Count, dimension];

            var vectorIndex = 0;
            foreach (var pivot in pivotIndices)
            {
                if (forRowSpace)
                {
                    for (var i = 0; i < dimension; ++i)
                    {
                        space[vectorIndex, i] = this[pivot, i];
                    }
                }
                else
                {
                    for (var i = 0; i < dimension; ++i)
                    {
                        space[vectorIndex, i] = this[i, pivot];
                    }
                }

                ++vectorIndex;
            }

            return space;
        }

        /// <summary>
        /// Gets the null space of the matrix.
        /// </summary>
        /// <returns>null(A)</returns>
        [Pure]
        public float[,] NullSpace()
        {
            var rRefMatrix = ReducedRowEchelonForm();
            var pivotColumns = rRefMatrix.PivotColumns(out var freeColumns);

            var nullSpace = new float[freeColumns.Count, N];

            var nullIndex = 0;
            foreach (var freeCol in freeColumns)
            {
                nullSpace[nullIndex, freeCol] = 1;

                for (var i = 0; i < rRefMatrix.M; ++i)
                {
                    foreach (var pivotCol in pivotColumns.Where(pivotCol => !Numerics.IsZero(rRefMatrix[i, pivotCol])))
                    {
                        nullSpace[nullIndex, pivotCol] = -rRefMatrix[i, freeCol];
                        break;
                    }
                }

                ++nullIndex;
            }

            return nullSpace;
        }

        #endregion

        #region Pivots and Free Variables

        /// <summary>
        /// Gets the pivot positions in the reduced row echelon form of the matrix.
        /// </summary>
        /// <returns>A list of tuples representing the pivot positions (row, column).</returns>
        [Pure]
        private List<(int, int)> PivotPositions()
        {
            var rRefMatrix = ReducedRowEchelonForm();
            var pivotPositions = new List<(int, int)>();

            for (var i = 0; i < rRefMatrix.M; ++i)
            {
                for (var j = 0; j < rRefMatrix.N; ++j)
                {
                    if (!Numerics.IsZero(rRefMatrix[i, j] - 1.0f))
                    {
                        continue;
                    }

                    pivotPositions.Add((i, j));
                    break;
                }
            }

            return pivotPositions;
        }

        /// <summary>
        /// Gets the pivot columns in the matrix.
        /// </summary>
        /// <returns>A hashset of pivot column indices.</returns>
        [Pure]
        private HashSet<int> PivotColumns()
        {
            return PivotIndices(false);
        }

        /// <summary>
        /// Gets the pivot columns and free columns in the matrix.
        /// </summary>
        /// <param name="freeColumns">An output parameter that will contain the free column indices.</param>
        /// <returns>A hashset of pivot column indices.</returns>
        [Pure]
        private HashSet<int> PivotColumns(out HashSet<int> freeColumns)
        {
            var pivotColumns = PivotColumns();
            freeColumns = FreeIndices(false);

            return pivotColumns;
        }

        /// <summary>
        /// Gets the free columns in the matrix.
        /// </summary>
        /// <returns>A hashset of free column indices.</returns>
        [Pure]
        private HashSet<int> FreeColumns()
        {
            PivotColumns(out var freeColumns);
            return freeColumns;
        }

        /// <summary>
        /// Gets the pivot rows in the reduced row echelon form of the matrix.
        /// </summary>
        /// <returns>A hashset of pivot row indices.</returns>
        [Pure]
        private HashSet<int> PivotRows()
        {
            return PivotIndices(true);
        }

        /// <summary>
        /// Gets the pivot rows and free rows in the matrix.
        /// </summary>
        /// <param name="freeRows">An output parameter that will contain the free row indices.</param>
        /// <returns>A hashset of pivot row indices.</returns>
        [Pure]
        private HashSet<int> PivotRows(out HashSet<int> freeRows)
        {
            var pivotRows = PivotRows();
            freeRows = FreeIndices(true);

            return pivotRows;
        }

        /// <summary>
        /// Gets the free rows in the matrix.
        /// </summary>
        /// <returns>A hashset of free row indices.</returns>
        [Pure]
        public HashSet<int> FreeRows()
        {
            PivotRows(out var freeRows);
            return freeRows;
        }

        /// <summary>
        /// Gets either the pivot rows or pivot columns.
        /// </summary>
        /// <param name="rows">True for rows, false for columns.</param>
        /// <returns>The respective pivot vector.</returns>
        /// <exception cref="Exception">Thrown if a vector could not be added.</exception>
        [Pure]
        private HashSet<int> PivotIndices(bool rows)
        {
            var pivotIndices = new HashSet<int>();

            for (var i = 0; i < M; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    if (Numerics.IsZero(this[i, j]))
                    {
                        continue;
                    }

                    if (!pivotIndices.Add(rows ? i : j))
                    {
                        throw new Exception("Error adding vector to hashset.");
                    }

                    break;
                }
            }

            return pivotIndices.Distinct().ToHashSet();
        }

        private HashSet<int> FreeIndices(bool rows)
        {
            var pivotIndices = rows ? PivotRows(out var freeIndices) : PivotColumns(out freeIndices);
            var size = rows ? M : N;
            for (var index = 0; index < size; ++index)
            {
                if (!pivotIndices.Contains(index))
                {
                    freeIndices.Add(index);
                }
            }

            return freeIndices;
        }

        #endregion

        #region Decomposition

        /// <summary>
        /// Performs the QR decomposition of the matrix using the Gram-Schmidt process.
        /// </summary>
        /// <returns>The orthogonal matrix Q, and the upper triangular matrix R.</returns>
        private (Matrix, Matrix) QRDecomposition()
        {
            var q = new Matrix(M, N);
            var r = new Matrix(N, N);

            for (var j = 0; j < N; ++j)
            {
                // Set the j-th column of Q to the j-th column of this matrix
                for (var i = 0; i < M; ++i)
                {
                    q[i, j] = this[i, j];
                }

                // Make orthogonal
                for (var k = 0; k < j; ++k)
                {
                    float dotProduct = 0;
                    for (var i = 0; i < M; ++i)
                    {
                        dotProduct += q[i, j] * q[i, k];
                    }

                    r[k, j] = dotProduct;
                    for (var i = 0; i < M; ++i)
                    {
                        q[i, j] -= dotProduct * q[i, k];
                    }
                }

                // Normalize
                float norm = 0;
                for (var i = 0; i < M; ++i)
                {
                    norm += q[i, j] * q[i, j];
                }

                norm = Mathf.Sqrt(norm);
                r[j, j] = norm;
                for (var i = 0; i < M; ++i)
                {
                    q[i, j] /= norm;
                }
            }

            return (q, r);
        }

        #endregion

        #region Solving Linear Systems

        /// <summary>
        /// Solves the linear system Ax = <paramref name="b" /> for x.
        /// </summary>
        /// <param name="b">The vector.</param>
        /// <returns>The solution vector x.</returns>
        [Pure]
        public Vector SolveLinearSystem(Vector b)
        {
            if (!CanAugment(this, b))
            {
                return new Vector();
            }

            var inverse = Inverse();
            if (inverse == new Matrix() && inverse.N <= 100)
            {
                return inverse * b;
            }

            var upperTriangular = (this | b).RowEchelonForm();

            return IsConsistent(upperTriangular) ? upperTriangular.BackSubstitution() : new Vector();

            bool IsConsistent(Matrix a)
            {
                for (var i = 0; i < a.M; ++i)
                {
                    var allZero = true;
                    for (var j = 0; j < a.N - 1; ++j)
                    {
                        if (Numerics.IsZero(a[i, j]))
                        {
                            continue;
                        }

                        allZero = false;
                        break;
                    }

                    if (allZero && !Numerics.IsZero(a[i, a.N - 1]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Solves the homogeneous system Ax = 0.
        /// </summary>
        /// <returns>The solution vector x.</returns>
        private Vector SolveHomogeneousSystem()
        {
            return ReducedRowEchelonForm().BackSubstitution();
        }

        public Vector CramersRule(Vector b)
        {
            if (!IsSquare)
            {
                return new Vector();
            }

            var n = M;
            var detA = Determinant();

            var x = new Vector(n);
            for (var j = 0; j < n; ++j)
            {
                var ai = DeepCopy();
                for (var i = 0; i < ai.M; ++i)
                {
                    ai[i, j] = b[i];
                }

                x[j] = ai.Determinant() / detA;
            }

            return x;
        }

        /// <summary>
        /// Solves the system of linear equations Ax = b using back substitution.
        /// </summary>
        /// <returns>The solution vector x.</returns>
        private Vector BackSubstitution()
        {
            var x = new Vector(N);
            for (var i = M - 1; i >= 0; --i)
            {
                x[i] = this[i, N - 1];
                for (var j = i + 1; j < N - 1; ++j)
                {
                    x[i] -= this[i, j] * x[j];
                }

                x[i] /= this[i, i];
            }

            return x;
        }

        #endregion

        #region Elimination

        /// <summary>
        /// Converts the matrix into row echelon form (REF) using Gaussian elimination.
        /// </summary>
        /// <returns>The matrix in row echelon form (REF).</returns>
        private Matrix RowEchelonForm()
        {
            var matrix = DeepCopy();
            var lead = 0;

            for (var i = 0; i < matrix.M; ++i)
            {
                if (matrix.N <= lead)
                {
                    break;
                }

                if (!FindPivot(matrix, i, ref lead, out var pivotRow))
                {
                    continue;
                }

                if (i != pivotRow)
                {
                    InterchangeRows(matrix, pivotRow, i);
                }

                if (!Numerics.IsZero(matrix[i, lead]))
                {
                    NormalizePivotRow(matrix, i, lead);
                }

                EliminateColumn(matrix, i, lead);

                lead++;
            }

            return matrix;

            bool FindPivot(Matrix a, int r, ref int leading, out int pivotRow)
            {
                pivotRow = r;
                while (Numerics.IsZero(a[pivotRow, leading]))
                {
                    pivotRow++;
                    if (pivotRow != a.M)
                    {
                        continue;
                    }

                    pivotRow = r;
                    leading++;
                    if (leading != a.N)
                    {
                        continue;
                    }

                    leading--;
                    return false;
                }

                return true;
            }

            void InterchangeRows(Matrix a, int row1, int row2)
            {
                for (var i = 0; i < matrix.N; ++i)
                {
                    (a[row1, i], a[row2, i]) = (a[row2, i], a[row1, i]);
                }
            }

            void NormalizePivotRow(Matrix a, int r, int leading)
            {
                var div = a[r, leading];
                for (var j = 0; j < a.N; ++j)
                {
                    a[r, j] /= div;
                }
            }

            void EliminateColumn(Matrix a, int r, int leading)
            {
                for (var i = 0; i < a.M; ++i)
                {
                    if (i == r)
                    {
                        continue;
                    }

                    var sub = a[i, leading];
                    for (var j = 0; j < a.N; ++j)
                    {
                        a[i, j] -= sub * a[r, j];
                    }
                }
            }
        }

        /// <summary>
        /// Gets a copy of the matrix in its reduced row echelon form by performing Gauss-Jordan elimination.
        /// </summary>
        /// <returns>The reduced row echelon form of the matrix.</returns>
        [Pure]
        private Matrix ReducedRowEchelonForm()
        {
            var matrix = RowEchelonForm();
            for (var r = matrix.M - 1; r >= 0; ++r)
            {
                var lead = -1;
                for (var j = 0; j < matrix.N; ++j)
                {
                    if (Numerics.IsZero(matrix[r, j]))
                    {
                        continue;
                    }

                    lead = j;
                    break;
                }

                if (lead == -1)
                {
                    continue;
                }

                for (var i = 0; i < r; ++i)
                {
                    var sub = matrix[i, lead];
                    for (var j = 0; j < matrix.N; ++j)
                    {
                        matrix[i, j] -= sub * matrix[r, j];
                    }
                }
            }

            return matrix;
        }

        #endregion

        [Pure]
        private static bool HasSameDimensions(Matrix a, Matrix b)
        {
            return a.M == b.M && a.N == b.N;
        }

        [Pure]
        private static bool CanAugment(Matrix a, Vector b)
        {
            return a.M == b.Dimension;
        }

        #region Operators

        /// <summary>
        /// Matrix addition operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Matrix <paramref name="b" />.</param>
        /// <returns><paramref name="a" /> + <paramref name="b" /></returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrices don't have matching dimensions.</exception>
        /// <remarks>Time complexity: O(m * n)</remarks>
        [Pure]
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (!HasSameDimensions(a, b))
            {
                throw new InvalidOperationException("Matrices must have the same dimensions to add.");
            }

            var result = new Matrix(a.M, a.N);
            for (var i = 0; i < a.M; ++i)
            {
                for (var j = 0; j < a.N; ++j)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Matrix negation operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <returns>-a</returns>
        /// <remarks>Time complexity: O(m * n)</remarks>
        public static Matrix operator -(Matrix a)
        {
            return -1 * a;
        }

        /// <summary>
        /// Matrix subtraction operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Matrix <paramref name="b" />.</param>
        /// <returns><paramref name="a" /> + -<paramref name="b" /></returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrices don't have matching dimensions.</exception>
        /// <remarks>Time complexity: O(m * n)</remarks>
        [Pure]
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (!HasSameDimensions(a, b))
            {
                throw new InvalidOperationException("Matrices must have the same dimensions to add.");
            }

            var result = new Matrix(a.M, a.N);
            for (var i = 0; i < a.M; ++i)
            {
                for (var j = 0; j < a.N; ++j)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Matrix multiplication operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Matrix <paramref name="b" />.</param>
        /// <returns>
        ///     <paramref name="a" /><paramref name="b" />
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrices don't have compatible dimensions.</exception>
        /// <remarks>Time complexity: O(m * n * p)</remarks>
        [Pure]
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.N != b.M)
            {
                throw new InvalidOperationException("Matrices must have compatible dimensions to multiply.");
            }

            var m = a.M;
            var n = a.N;
            var p = b.N;

            var result = new Matrix(m, p);
            for (var i = 0; i < m; ++i)
            {
                for (var j = 0; j < p; ++j)
                {
                    for (var k = 0; k < n; ++k)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Matrix-vector multiplication operator.
        /// </summary>
        /// <param name="a">The matrix.</param>
        /// <param name="b">The vector.</param>
        /// <returns>
        ///     <paramref name="a" /><paramref name="b" />
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the matrix's column count is not equal to the vector's dimension</exception>
        /// <remarks>Time complexity: O(m * n)</remarks>
        [Pure]
        public static Vector operator *(Matrix a, Vector b)
        {
            if (a.N != b.Dimension)
            {
                throw new ArgumentException("Matrix column count must equal vector length for multiplication.");
            }

            var result = new Vector(a.M);
            {
                for (var i = 0; i < a.M; ++i)
                {
                    for (var j = 0; j < a.N; ++j)
                    {
                        result[i] += a[i, j] * b[j];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Scalar multiplication operator.
        /// </summary>
        /// <param name="c">The scalar.</param>
        /// <param name="a">The matrix.</param>
        /// <returns>cA</returns>
        /// <remarks>Time complexity: O(m * n)</remarks>
        [Pure]
        public static Matrix operator *(float c, Matrix a)
        {
            var result = new Matrix(a.M, a.N);
            for (var i = 0; i < a.M; ++i)
            {
                for (var j = 0; j < a.N; ++j)
                {
                    result[i, j] = c * a[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Matrix augmentation operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Matrix <paramref name="b" />.</param>
        /// <returns><paramref name="a" /> | <paramref name="b" /></returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrices have different row sizes.</exception>
        /// <remarks>Time complexity: O(m * (na + nb))</remarks>
        public static Matrix operator |(Matrix a, Matrix b)
        {
            if (a.M != b.M)
            {
                throw new InvalidOperationException("Matrices must have the same number of rows to augment.");
            }

            var result = new Matrix(a.M, a.N + b.N);

            for (var i = 0; i < a.M; ++i)
            {
                for (var j = 0; j < a.N; ++j)
                {
                    result[i, j] = a[i, j];
                }

                for (var j = 0; j < b.N; ++j)
                {
                    result[i, j + a.N] = b[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Matrix-vector augmentation operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Vector <paramref name="b" />.</param>
        /// <returns><paramref name="a" /> | <paramref name="b" /></returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if <paramref name="a" />'s row count is different from
        ///     <paramref name="b" />'s dimension.
        /// </exception>
        public static Matrix operator |(Matrix a, Vector b)
        {
            if (!CanAugment(a, b))
            {
                throw new InvalidOperationException("The matrix's row count must match the vector's dimension to augment.");
            }

            var augmentedMatrix = new Matrix(a.M, a.N + 1);
            for (var i = 0; i < a.M; ++i)
            {
                for (var j = 0; j < a.N; ++j)
                {
                    augmentedMatrix[i, j] = a[i, j];
                }

                augmentedMatrix[i, a.N] = b[i];
            }

            return augmentedMatrix;
        }

        /// <summary>
        /// Matrix equality operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Matrix <paramref name="b" />.</param>
        /// <returns>True if the matrices have the same contents and false if not.</returns>
        public static bool operator ==(Matrix a, Matrix b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Matrix inequality operator.
        /// </summary>
        /// <param name="a">Matrix <paramref name="a" />.</param>
        /// <param name="b">Matrix <paramref name="b" />.</param>
        /// <returns>True if the matrices have different contents and false if not.</returns>
        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        #endregion

        #region Interface Implementations

        public bool Equals(Matrix other)
        {
            if (!HasSameDimensions(this, other))
            {
                return false;
            }

            for (var i = 0; i < M; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    if (!Mathf.Approximately(this[i, j], other[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix other && Equals(other);
        }

        IEnumerator<float> IEnumerable<float>.GetEnumerator()
        {
            for (var i = 0; i < M; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    yield return this[i, j];
                }
            }
        }

        //[MustDisposeResource]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<float>)this).GetEnumerator();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_entries, M, N);
        }

        #endregion

        #endregion
    }
}
