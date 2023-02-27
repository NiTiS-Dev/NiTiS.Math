﻿using System;
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
/// Two-dimension vector with direction and magnitude.
/// </summary>
/// <typeparam name="T">Vector type.</typeparam>
[DebuggerDisplay($@"{{{nameof(ToString)}(""G""),nq}}")]
public unsafe struct Vector2d<T> :
    // Vector op Vector
    IAdditionOperators<Vector2d<T>, Vector2d<T>, Vector2d<T>>,
    ISubtractionOperators<Vector2d<T>, Vector2d<T>, Vector2d<T>>,
    IDivisionOperators<Vector2d<T>, Vector2d<T>, Vector2d<T>>,
    IMultiplyOperators<Vector2d<T>, Vector2d<T>, Vector2d<T>>,
    IEqualityOperators<Vector2d<T>, Vector2d<T>, bool>,
    // Vector op T
    IAdditionOperators<Vector2d<T>, T, Vector2d<T>>,
    ISubtractionOperators<Vector2d<T>, T, Vector2d<T>>,
    IDivisionOperators<Vector2d<T>, T, Vector2d<T>>,
    IMultiplyOperators<Vector2d<T>, T, Vector2d<T>>,
    // Unary op
    IUnaryNegationOperators<Vector2d<T>, Vector2d<T>>,
    IUnaryPlusOperators<Vector2d<T>, Vector2d<T>>,
    IFormattable,
    IEquatable<Vector2d<T>>
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
    /// Squared magnitude of vector.
    /// </summary>
    public readonly T LengthSquared
    {
        [MethodImpl(AggressiveInlining | AggressiveOptimization)]
        get => Vector2d.Dot(this, this);
    }

    public const int ElementCount = 2;
    private static readonly int VectorSize = sizeof(T) * ElementCount;

    /// <summary>
    /// Creates new two-dimensional vector with values <c>(<paramref name="x"/>, <paramref name="y"/>)</c>.
    /// </summary>
    /// <param name="x">X value of vector.</param>
    /// <param name="y">Y value of vector.</param>
    public Vector2d(T x, T y)
    => (X, Y) = (x, y);

    /// <summary>
    /// Creates new two-dimensional vector with values <c>(<paramref name="xy"/>, <paramref name="xy"/>)</c>.
    /// </summary>
    /// <param name="xy">X and Y value of vector.</param>
    public Vector2d(T xy)
        => (X, Y) = (xy, xy);

    /// <summary>
    /// Creates vector by buffer.
    /// </summary>
    /// <param name="data">Buffer with vector data.</param>
    /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
    public Vector2d(ReadOnlySpan<T> data)
    {
        if (data.Length < ElementCount)
            throw new ArgumentOutOfRangeException(nameof(data));

        this = Unsafe.ReadUnaligned<Vector2d<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
    }

    /// <summary>
    /// Creates vector by buffer.
    /// </summary>
    /// <param name="data">Buffer with vector data.</param>
    /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
    public Vector2d(ReadOnlySpan<byte> data)
    {
        if (data.Length < VectorSize)
            throw new ArgumentOutOfRangeException(nameof(data));

        this = Unsafe.ReadUnaligned<Vector2d<T>>(ref MemoryMarshal.GetReference(data));
    }

    /// <summary>
    /// Vector with all ones, <c>(1, 1)</c>.
    /// </summary>
    public static Vector2d<T> One => new(T.One, T.One);

    /// <summary>
    /// Vector with all zeros, <c>(0, 0)</c>.
    /// </summary>
    public static Vector2d<T> Zero => new(T.Zero, T.Zero);

    /// <summary>
    /// Vector which X is one, <c>(1, 0)</c>.
    /// </summary>
    public static Vector2d<T> UnitX => new(T.One, T.Zero);

    /// <summary>
    /// Vector which Y is one, <c>(0, 1)</c>.
    /// </summary>
    public static Vector2d<T> UnitY => new(T.Zero, T.One);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator +(Vector2d<T> left, Vector2d<T> right)
        => new(
            left.X + right.X,
            left.Y + right.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator -(Vector2d<T> left, Vector2d<T> right)
        => new(
            left.X - right.X,
            left.Y - right.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator /(Vector2d<T> left, Vector2d<T> right)
        => new(
            left.X / right.X,
            left.Y / right.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator *(Vector2d<T> left, Vector2d<T> right)
        => new(
            left.X * right.X,
            left.Y * right.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator ==(Vector2d<T> left, Vector2d<T> right)
        => left.X == right.X
        && left.Y == right.Y;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static bool operator !=(Vector2d<T> left, Vector2d<T> right)
        => left.X != right.X
        || left.Y != right.Y;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator +(Vector2d<T> left, T right)
        => new(
            left.X + right,
            left.Y + right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator -(Vector2d<T> left, T right)
        => new(
            left.X - right,
            left.Y - right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator /(Vector2d<T> left, T right)
        => new(
            left.X / right,
            left.Y / right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator /(T left, Vector2d<T> right)
        => new(
            left / right.X,
            left / right.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator *(Vector2d<T> left, T right)
        => new(
            left.X * right,
            left.Y * right
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator *(T left, Vector2d<T> right)
        => new(
            left * right.X,
            left * right.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator -(Vector2d<T> operand)
        => new(
            -operand.X,
            -operand.Y
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> operator +(Vector2d<T> operand)
        => new(
            +operand.X,
            +operand.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static explicit operator Vector3d<T>(Vector2d<T> operand)
        => new(
            operand.X,
            operand.Y,
            T.Zero
            );
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static explicit operator Vector4d<T>(Vector2d<T> operand)
        => new(
            operand.X,
            operand.Y,
            T.Zero,
            T.Zero
            );

    public readonly void CopyTo(T[] array)
        => CopyTo(array, 0);
    public readonly void CopyTo(T[] array, uint offset)
    {
        if (array.LongLength < ElementCount + offset)
            throw new ArgumentOutOfRangeException(nameof(array));

        array[0 + offset] = X;
        array[1 + offset] = Y;
    }
    public readonly void CopyTo(Span<T> array, uint offset)
    {
        if (array.Length < ElementCount + offset)
            throw new ArgumentOutOfRangeException(nameof(array));

        array[0 + (int)offset] = X;
        array[1 + (int)offset] = Y;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
        => HashCode.Combine(X, Y);

    /// <inheritdoc/>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is Vector2d<T> vec
        ? vec == this : false;
    public readonly bool Equals(Vector2d<T> other)
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
        sb.Append('>');
        return sb.ToString();
    }
}
