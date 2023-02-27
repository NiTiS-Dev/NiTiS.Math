using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Vectors;

/// <summary>
/// Third-dimension vector with direction and magnitude.
/// </summary>
[DebuggerDisplay($@"{{{nameof(ToString)}(""G""),nq}}")]
public unsafe struct Vector3d<T> :
    // Vector op Vector
    IAdditionOperators<Vector3d<T>, Vector3d<T>, Vector3d<T>>,
    ISubtractionOperators<Vector3d<T>, Vector3d<T>, Vector3d<T>>,
    IDivisionOperators<Vector3d<T>, Vector3d<T>, Vector3d<T>>,
    IMultiplyOperators<Vector3d<T>, Vector3d<T>, Vector3d<T>>,
    IEqualityOperators<Vector3d<T>, Vector3d<T>, bool>,
    // Vector op T
    IAdditionOperators<Vector3d<T>, T, Vector3d<T>>,
    ISubtractionOperators<Vector3d<T>, T, Vector3d<T>>,
    IDivisionOperators<Vector3d<T>, T, Vector3d<T>>,
    IMultiplyOperators<Vector3d<T>, T, Vector3d<T>>,
    // Unary op
    IUnaryNegationOperators<Vector3d<T>, Vector3d<T>>,
    IUnaryPlusOperators<Vector3d<T>, Vector3d<T>>,
    IFormattable,
    IEquatable<Vector3d<T>>
    where T :
        unmanaged,
        INumberBase<T>
{
    /// <summary>
    /// X value of vector, the first dimension.
    /// </summary>
    public T X;
    /// <summary>
    /// Y value of vector, the second dimension.
    /// </summary>
    public T Y;
    /// <summary>
    /// Z value of vector, the third dimension.
    /// </summary>
    public T Z;

    /// <summary>
    /// Squared magnitude of vector.
    /// </summary>
    public readonly T LengthSquared
    {
        [MethodImpl(AggressiveInlining | AggressiveOptimization)]
        get => Vector3d.Dot(this, this);
    }

    public const int ElementCount = 3;
    private static readonly int VectorSize = sizeof(T) * ElementCount;

    /// <summary>
    /// Creates new third-dimensional vector with values <c>(<paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>)</c>.
    /// </summary>
    /// <param name="x">X value of vector.</param>
    /// <param name="y">Y value of vector.</param>
    /// <param name="z">Z value of vector.</param>
    public Vector3d(T x, T y, T z)
        => (X, Y, Z) = (x, y, z);

    /// <summary>
    /// Creates new third-dimensional vector with values <c>(<paramref name="xyz"/>, <paramref name="xyz"/>, <paramref name="xyz"/>)</c>.
    /// </summary>
    /// <param name="xyz">X, Y and Z value of vector.</param>
    public Vector3d(T xyz)
        => (X, Y, Z) = (xyz, xyz, xyz);

    /// <summary>
    /// Expands two-dimension vector to third-dimension.
    /// </summary>
    /// <param name="base2">Base vector.</param>
    /// <param name="z">Z value.</param>
    public Vector3d(Vector2d<T> base2, T z)
        => (X, Y, Z) = (base2.X, base2.Y, z);


    /// <summary>
    /// Creates vector by buffer.
    /// </summary>
    /// <param name="data">Buffer with vector data.</param>
    /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
    public Vector3d(ReadOnlySpan<T> data)
    {
        if (data.Length < ElementCount)
            throw new ArgumentOutOfRangeException(nameof(data));

        this = Unsafe.ReadUnaligned<Vector3d<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
    }

    /// <summary>
    /// Creates vector by buffer.
    /// </summary>
    /// <param name="data">Buffer with vector data.</param>
    /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
    public Vector3d(ReadOnlySpan<byte> data)
    {
        if (data.Length < VectorSize)
            throw new ArgumentOutOfRangeException(nameof(data));

        this = Unsafe.ReadUnaligned<Vector3d<T>>(ref MemoryMarshal.GetReference(data));
    }

    /// <summary>
    /// Vector with all ones, <c>(1, 1, 1)</c>.
    /// </summary>
    public static Vector3d<T> One => new(T.One, T.One, T.One);

    /// <summary>
    /// Vector with all zeros, <c>(0, 0, 0)</c>.
    /// </summary>
    public static Vector3d<T> Zero => new(T.Zero, T.Zero, T.Zero);

    /// <summary>
    /// Vector which X is one, <c>(1, 0, 0)</c>.
    /// </summary>
    public static Vector3d<T> UnitX => new(T.One, T.Zero, T.Zero);

    /// <summary>
    /// Vector which Y is one, <c>(0, 1, 0)</c>.
    /// </summary>
    public static Vector3d<T> UnitY => new(T.Zero, T.One, T.Zero);

    /// <summary>
    /// Vector which Z is one, <c>(0, 0, 1)</c>.
    /// </summary>
    public static Vector3d<T> UnitZ => new(T.Zero, T.Zero, T.One);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator +(Vector3d<T> left, Vector3d<T> right)
        => new(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator -(Vector3d<T> left, Vector3d<T> right)
        => new(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator /(Vector3d<T> left, Vector3d<T> right)
        => new(
            left.X / right.X,
            left.Y / right.Y,
            left.Z / right.Z
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator *(Vector3d<T> left, Vector3d<T> right)
        => new(
            left.X * right.X,
            left.Y * right.Y,
            left.Z * right.Z
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator ==(Vector3d<T> left, Vector3d<T> right)
        => left.X == right.X
        && left.Y == right.Y
        && left.Z == right.Z;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator !=(Vector3d<T> left, Vector3d<T> right)
        => left.X != right.X
        || left.Y != right.Y
        || left.Z != right.Z;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator +(Vector3d<T> left, T right)
        => new(
            left.X + right,
            left.Y + right,
            left.Z + right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator -(Vector3d<T> left, T right)
        => new(
            left.X - right,
            left.Y - right,
            left.Z - right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator /(Vector3d<T> left, T right)
        => new(
            left.X / right,
            left.Y / right,
            left.Z / right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator /(T left, Vector3d<T> right)
        => new(
            left / right.X,
            left / right.Y,
            left / right.Z
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator *(Vector3d<T> left, T right)
        => new(
            left.X * right,
            left.Y * right,
            left.Z * right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator *(T left, Vector3d<T> right)
        => new(
            left * right.X,
            left * right.Y,
            left * right.Z
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator -(Vector3d<T> operand)
        => new(
            -operand.X,
            -operand.Y,
            -operand.Z
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> operator +(Vector3d<T> operand)
        => new(
            +operand.X,
            +operand.Y,
            +operand.Z
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static implicit operator Vector2d<T>(Vector3d<T> operand)
        => new(
            operand.X,
            operand.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static explicit operator Vector4d<T>(Vector3d<T> operand)
        => new(
            operand.X,
            operand.Y,
            operand.Z,
            T.One
            );

    public readonly void CopyTo(T[] array)
        => CopyTo(array, 0);
    public readonly void CopyTo(T[] array, uint offset)
    {
        if (array.LongLength < ElementCount + offset)
            throw new ArgumentOutOfRangeException(nameof(array));

        array[0 + offset] = X;
        array[1 + offset] = Y;
        array[2 + offset] = Z;
    }
    public readonly void CopyTo(Span<T> array, uint offset)
    {
        if (array.Length < ElementCount + offset)
            throw new ArgumentOutOfRangeException(nameof(array));

        array[0 + (int)offset] = X;
        array[1 + (int)offset] = Y;
        array[2 + (int)offset] = Z;
    }
    /// <inheritdoc/>
    public override readonly int GetHashCode()
        => HashCode.Combine(X, Y, Z);

    /// <inheritdoc/>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is Vector3d<T> vec
        ? vec == this : false;

    public readonly bool Equals(Vector3d<T> other)
        => this == other;

    /// <inheritdoc/>
    public override readonly string ToString() => ToString("G", CultureInfo.CurrentCulture);
    public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);
    public readonly string ToString(string? format, IFormatProvider? formatProvider)
    {
        StringBuilder sb = new();
        string separator = ",";
        sb.Append('<');

        sb.Append(X.ToString(format, formatProvider));
        sb.Append(separator);
        if (!string.IsNullOrWhiteSpace(separator))
            sb.Append(' ');

        sb.Append(Y.ToString(format, formatProvider));
        sb.Append(separator);
        if (!string.IsNullOrWhiteSpace(separator))
            sb.Append(' ');

        sb.Append(Z.ToString(format, formatProvider));

        sb.Append('>');
        return sb.ToString();
    }
}
