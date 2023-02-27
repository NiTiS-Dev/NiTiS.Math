using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NiTiS.Math.Vectors;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Matrices;

/// <summary>Represents a 4x4 matrix.</summary>
/// <typeparam name="T">Matrix data type.</typeparam>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Matrix4x4<T> :
    IAdditionOperators<Matrix4x4<T>, Matrix4x4<T>, Matrix4x4<T>>,
    ISubtractionOperators<Matrix4x4<T>, Matrix4x4<T>, Matrix4x4<T>>,
    IMultiplyOperators<Matrix4x4<T>, Matrix4x4<T>, Matrix4x4<T>>,

    IAdditionOperators<Matrix4x4<T>, T, Matrix4x4<T>>,
    ISubtractionOperators<Matrix4x4<T>, T, Matrix4x4<T>>,
    IMultiplyOperators<Matrix4x4<T>, T, Matrix4x4<T>>,

    IEqualityOperators<Matrix4x4<T>, Matrix4x4<T>, bool>,

    IEquatable<Matrix4x4<T>>
    where T : unmanaged, INumberBase<T>
{
    #region Matrix
    public const int
        RowsCount = 4,
        ColumnsCount = 4,
        ElementCount = ColumnsCount * RowsCount;

    public T M11;
    public T M12;
    public T M13;
    public T M14;

    public T M21;
    public T M22;
    public T M23;
    public T M24;

    public T M31;
    public T M32;
    public T M33;
    public T M34;

    public T M41;
    public T M42;
    public T M43;
    public T M44;
    #endregion

    public Vector4d<T> Row1
        => Unsafe.As<T, Vector4d<T>>(ref M11);
    public Vector4d<T> Row2
        => Unsafe.As<T, Vector4d<T>>(ref M21);
    public Vector4d<T> Row3
        => Unsafe.As<T, Vector4d<T>>(ref M31);
    public Vector4d<T> Row4
        => Unsafe.As<T, Vector4d<T>>(ref M41);

    public Vector4d<T> Column1
        => new(M11, M21, M31, M41);
    public Vector4d<T> Column2
        => new(M12, M22, M32, M42);
    public Vector4d<T> Column3
        => new(M13, M23, M33, M43);
    public Vector4d<T> Column4
        => new(M14, M24, M34, M44);

    public T this[int index]
    {
        get
        {
            if (index >= ElementCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            return Unsafe.Add(ref M11, index);
        }
    }
    public T this[int x_, int _x]
    {
        get
        {
            if (_x >= RowsCount)
                throw new ArgumentOutOfRangeException(nameof(_x));

            if (x_ >= ColumnsCount)
                throw new ArgumentOutOfRangeException(nameof(x_));

            return Unsafe.Add(ref M11, x_ * 4 + _x);
        }
    }


    public Vector3d<T> Translation
    {
        get => new(M41, M42, M43);
        set => (M41, M42, M43) = (value.X, value.Y, value.Z);
    }

    public static Matrix4x4<T> Identity
        => new(
        T.One, T.Zero, T.Zero, T.Zero,
        T.Zero, T.One, T.Zero, T.Zero,
        T.Zero, T.Zero, T.One, T.Zero,
        T.Zero, T.Zero, T.Zero, T.One
        );

    /// <summary>Calculates the determinant of the current 4x4 matrix</summary>
    /// <returns>The determinant</returns>
    public readonly T GetDeterminant()
    {
        // | a b c d |     | f g h |     | e g h |     | e f h |     | e f g |
        // | e f g h | = a | j k l | - b | i k l | + c | i j l | - d | i j k |
        // | i j k l |     | n o p |     | m o p |     | m n p |     | m n o |
        // | m n o p |
        //
        //   | f g h |
        // a | j k l | = a ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
        //   | n o p |
        //
        //   | e g h |
        // b | i k l | = b ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
        //   | m o p |
        //
        //   | e f h |
        // c | i j l | = c ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
        //   | m n p |
        //
        //   | e f g |
        // d | i j k | = d ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
        //   | m n o |
        //
        // Cost of operation
        // 17 adds and 28 muls.
        //
        // add: 6 + 8 + 3 = 17
        // mul: 12 + 16 = 28

        T a = M11, b = M12, c = M13, d = M14;
        T e = M21, f = M22, g = M23, h = M24;
        T i = M31, j = M32, k = M33, l = M34;
        T m = M41, n = M42, o = M43, p = M44;

        T kp_lo = k * p - l * o;
        T jp_ln = j * p - l * n;
        T jo_kn = j * o - k * n;
        T ip_lm = i * p - l * m;
        T io_km = i * o - k * m;
        T in_jm = i * n - j * m;

        return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
               b * (e * kp_lo - g * ip_lm + h * io_km) +
               c * (e * jp_ln - f * ip_lm + h * in_jm) -
               d * (e * jo_kn - f * io_km + g * in_jm);
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public Matrix4x4(
        T m11, T m12, T m13, T m14,
        T m21, T m22, T m23, T m24,
        T m31, T m32, T m33, T m34,
        T m41, T m42, T m43, T m44
        )
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;
        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;
        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;
    }

    /// <summary>
    /// Add one matrix to another one.
    /// </summary>
    /// <param name="left">The left argument.</param>
    /// <param name="right">The right argument.</param>
    /// <returns>Resulting matrix - result of addition <paramref name="right"/> to <paramref name="left"/> matrix.</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Matrix4x4<T> operator +(Matrix4x4<T> left, Matrix4x4<T> right)
    {
        left.M11 += right.M11;
        left.M12 += right.M12;
        left.M13 += right.M13;
        left.M14 += right.M14;
        left.M21 += right.M21;
        left.M22 += right.M22;
        left.M23 += right.M23;
        left.M24 += right.M24;
        left.M31 += right.M31;
        left.M32 += right.M32;
        left.M33 += right.M33;
        left.M34 += right.M34;
        left.M41 += right.M41;
        left.M42 += right.M42;
        left.M43 += right.M43;
        left.M44 += right.M44;
        return left;
    }

    /// <summary>
    /// Subtract one matrix from another one.
    /// </summary>
    /// <param name="left">The left argument.</param>
    /// <param name="right">The right argument.</param>
    /// <returns>Resulting matrix - result of subtraction <paramref name="right"/> from <paramref name="left"/> matrix.</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Matrix4x4<T> operator -(Matrix4x4<T> left, Matrix4x4<T> right)
    {
        left.M11 -= right.M11;
        left.M12 -= right.M12;
        left.M13 -= right.M13;
        left.M14 -= right.M14;
        left.M21 -= right.M21;
        left.M22 -= right.M22;
        left.M23 -= right.M23;
        left.M24 -= right.M24;
        left.M31 -= right.M31;
        left.M32 -= right.M32;
        left.M33 -= right.M33;
        left.M34 -= right.M34;
        left.M41 -= right.M41;
        left.M42 -= right.M42;
        left.M43 -= right.M43;
        left.M44 -= right.M44;
        return left;
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator ==(Matrix4x4<T> left, Matrix4x4<T> right)
        => left.M11 == right.M11 && left.M22 == right.M22 && left.M33 == right.M33 && left.M44 == right.M44 && // Check diagonal element first for early out.
            left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 &&
            left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 &&
            left.M34 == right.M34 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator !=(Matrix4x4<T> left, Matrix4x4<T> right)
        => left.M11 != right.M11 || left.M22 != right.M22 || left.M33 != right.M33 || left.M44 != right.M44 || // Check diagonal element first for early out.
            left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 ||
            left.M23 != right.M23 || left.M24 != right.M24 || left.M31 != right.M31 || left.M32 != right.M32 ||
            left.M34 != right.M34 || left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43;

    /// <summary>
    /// Multiply two matrices.
    /// </summary>
    /// <param name="left">The left argument.</param>
    /// <param name="right">The right argument.</param>
    /// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> and <paramref name="right"/> matrices.</returns>

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Matrix4x4<T> operator *(Matrix4x4<T> left, Matrix4x4<T> right)
    {
        Unsafe.SkipInit(out Matrix4x4<T> m);

        // First row
        m.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
        m.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
        m.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
        m.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

        // Second row
        m.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
        m.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
        m.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
        m.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

        // Third row
        m.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
        m.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
        m.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
        m.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

        // Fourth row
        m.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
        m.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
        m.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
        m.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

        return m;
    }

    /// <summary>
    /// Add scalar to matrix.
    /// </summary>
    /// <param name="left">The left argument.</param>
    /// <param name="right">The right argument.</param>
    /// <returns>Resulting matrix - result of addition <paramref name="right"/> scalar to <paramref name="left"/> matrix.</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Matrix4x4<T> operator +(Matrix4x4<T> left, T right)
    {
        left.M11 += right;
        left.M12 += right;
        left.M13 += right;
        left.M14 += right;
        left.M21 += right;
        left.M22 += right;
        left.M23 += right;
        left.M24 += right;
        left.M31 += right;
        left.M32 += right;
        left.M33 += right;
        left.M34 += right;
        left.M41 += right;
        left.M42 += right;
        left.M43 += right;
        left.M44 += right;
        return left;
    }

    /// <summary>
    /// Subtract scalar from matrix.
    /// </summary>
    /// <param name="left">The left argument.</param>
    /// <param name="right">The right argument.</param>
    /// <returns>Resulting matrix - result of subtraction <paramref name="right"/> scalar from <paramref name="left"/> matrix.</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Matrix4x4<T> operator -(Matrix4x4<T> left, T right)
    {
        left.M11 -= right;
        left.M12 -= right;
        left.M13 -= right;
        left.M14 -= right;
        left.M21 -= right;
        left.M22 -= right;
        left.M23 -= right;
        left.M24 -= right;
        left.M31 -= right;
        left.M32 -= right;
        left.M33 -= right;
        left.M34 -= right;
        left.M41 -= right;
        left.M42 -= right;
        left.M43 -= right;
        left.M44 -= right;
        return left;
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator ==(Matrix4x4<T> left, T right)
        => left.M11 == right && left.M22 == right && left == right && left.M44 == right && // Check diagonal element first for early out.
            left.M12 == right && left.M13 == right && left == right && left.M21 == right &&
            left.M23 == right && left.M24 == right && left == right && left.M32 == right &&
            left.M34 == right && left.M41 == right && left == right && left.M43 == right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator !=(Matrix4x4<T> left, T right)
        => left.M11 != right || left.M22 != right || left.M33 != right || left.M44 != right || // Check diagonal element first for early out.
            left.M12 != right || left.M13 != right || left.M14 != right || left.M21 != right ||
            left.M23 != right || left.M24 != right || left.M31 != right || left.M32 != right ||
            left.M34 != right || left.M41 != right || left.M42 != right || left.M43 != right;

    /// <summary>
    /// Multiply matrix by scalar.
    /// </summary>
    /// <param name="left">The left argument.</param>
    /// <param name="right">The right argument.</param>
    /// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> matrix and <paramref name="right"/> scalar.</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Matrix4x4<T> operator *(Matrix4x4<T> left, T right)
    {
        Unsafe.SkipInit(out Matrix4x4<T> m);

        // First row
        m.M11 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;
        m.M12 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;
        m.M13 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;
        m.M14 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;

        // Second row			
        m.M21 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;
        m.M22 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;
        m.M23 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;
        m.M24 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;

        // Third row			
        m.M31 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;
        m.M32 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;
        m.M33 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;
        m.M34 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;

        // Fourth row			
        m.M41 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;
        m.M42 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;
        m.M43 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;
        m.M44 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;

        return m;
    }

    /// <summary>Indicates whether the current matrix is the identity matrix.</summary>
    /// <value><see langword="true" /> if the current matrix is the identity matrix; otherwise: <see langword="false" />.</value>
    public readonly bool IsIdentity
            => M11 == T.One && M22 == T.One && M33 == T.One && M44 == T.One
            && M12 == T.Zero && M13 == T.Zero && M14 == T.Zero
            && M21 == T.Zero && M23 == T.Zero && M24 == T.Zero
            && M31 == T.Zero && M32 == T.Zero && M34 == T.Zero
            && M41 == T.Zero && M42 == T.Zero && M43 == T.Zero;

    /// <summary>
    /// Check equality to another matrix.
    /// </summary>
    /// <param name="obj">Object for equality.</param>
    /// <returns><see langword="true"/> when object is matrix that equal to this one, otherwise <see langword="false"/>.</returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is Matrix4x4<T> mat && mat == this;

    /// <summary>
    /// Check equality to another matrix.
    /// </summary>
    /// <param name="mat">The second matrix.</param>
    /// <returns><see langword="true"/> when other matrix is equal to this one, otherwise <see langword="false"/>.</returns>
    public readonly bool Equals(Matrix4x4<T> mat)
        => mat == this;
    public override readonly int GetHashCode()
    {
        HashCode hash = default;

        hash.Add(M11);
        hash.Add(M12);
        hash.Add(M13);
        hash.Add(M14);

        hash.Add(M21);
        hash.Add(M22);
        hash.Add(M23);
        hash.Add(M24);

        hash.Add(M31);
        hash.Add(M32);
        hash.Add(M33);
        hash.Add(M34);

        hash.Add(M41);
        hash.Add(M42);
        hash.Add(M43);
        hash.Add(M44);

        return hash.ToHashCode();
    }
    public override readonly string ToString()
        => string.Format(CultureInfo.CurrentCulture, "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}",
                             M11, M12, M13, M14,
                             M21, M22, M23, M24,
                             M31, M32, M33, M34,
                             M41, M42, M43, M44);
}