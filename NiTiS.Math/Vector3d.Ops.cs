using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static class Vector3d
{
    /// <summary>
    /// Convert <see cref="Vector3"/> to <see cref="Vector3d{T}"/>
    /// </summary>
    /// <param name="vector">Origin non-generic vector</param>
    /// <returns>The generic vector</returns>
    [MethodImpl(AggressiveOptimization | AggressiveInlining)]
    public static Vector3d<float> ConvertToGeneric(this Vector3 vector)
        => Unsafe.As<Vector3, Vector3d<float>>(ref vector);

    /// <summary>
    /// Convert <see cref="Vector3d{T}"/> to <see cref="Vector3"/>
    /// </summary>
    /// <param name="vector">Origin generic vector</param>
    /// <returns>The non-generic vector</returns>
    [MethodImpl(AggressiveOptimization | AggressiveInlining)]
    public static Vector3 ConvertToSystem(this Vector3d<float> vector)
        => Unsafe.As<Vector3d<float>, Vector3>(ref vector);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Abs<T>(Vector3d<T> vec)
        where T : unmanaged, INumberBase<T>
        => new(
            T.Abs(vec.X),
            T.Abs(vec.Y),
            T.Abs(vec.Z)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Add<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => left + right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Add<T>(Vector3d<T> left, T right)
        where T : unmanaged, INumberBase<T>
        => left + right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T AngleBetween<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>, ITrigonometricFunctions<T>, IComparisonOperators<T, T, bool>
    {
        T value = Dot(left, right);
        T v1Length = Length(left);
        T v2Length = Length(right);

        value /= v1Length * v2Length;

        if (value <= -T.One) return T.Pi;
        if (value >= T.One) return T.Zero;
        return T.Acos(value);
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Bitwise<T>(Vector3d<T> operand)
        where T : unmanaged, INumberBase<T>, IBitwiseOperators<T, T, T>
        => new(
            ~operand.X,
            ~operand.Y,
            ~operand.Z
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Clamp<T>(Vector3d<T> value, Vector3d<T> min, Vector3d<T> max)
        where T : unmanaged, INumberBase<T>, INumber<T>
        => new(
            T.Clamp(value.X, min.X, max.X),
            T.Clamp(value.Y, min.Y, max.Y),
            T.Clamp(value.Z, min.Z, max.Z)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Cross<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => new(
            left.Y * right.Z - left.Z * right.Y,
            left.Z * right.X - left.X * right.Z,
            left.X * right.Y - left.Y * right.X
        );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Distance<T>(Vector3d<T> from, Vector3d<T> to)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => T.Sqrt(DistanceSquared(from, to));

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T DistanceSquared<T>(Vector3d<T> from, Vector3d<T> to)
        where T : unmanaged, INumberBase<T>
    {
        Vector3d<T> diff = from - to;
        return Dot(diff, diff);
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Divide<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => left / right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Divide<T>(Vector3d<T> left, T right)
        where T : unmanaged, INumberBase<T>
        => left / right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Divide<T>(T left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => new(
            left * right.X,
            left * right.Y,
            left * right.Z
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Dot<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => left.X * right.X + left.Y * right.Y + left.Z * right.Z;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Length<T>(Vector3d<T> operand)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => T.Sqrt(operand.LengthSquared);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Lerp<T>(Vector3d<T> left, Vector3d<T> right, T amount)
        where T : unmanaged, INumberBase<T>
        => left * (T.One - amount) + right * amount;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Max<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumber<T>
        => new(
            T.Max(left.X, right.X),
            T.Max(left.Y, right.Y),
            T.Max(left.Z, right.Z)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Min<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumber<T>
        => new(
            T.Min(left.X, right.X),
            T.Min(left.Y, right.Y),
            T.Min(left.Z, right.Z)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Multiply<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Multiply<T>(Vector3d<T> left, T right)
        where T : unmanaged, INumberBase<T>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Multiply<T>(T left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>
        => new(
            left * right.X,
            left * right.Y,
            left * right.Z
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Negate<T>(Vector3d<T> operand)
        where T : unmanaged, INumberBase<T>
        => -operand;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Normalize<T>(Vector3d<T> operand)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => operand / Length(operand);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Reflect<T>(Vector3d<T> vector, Vector3d<T> normal, T two)
        where T : unmanaged, INumberBase<T>
        => vector - two * Dot(vector, normal) * normal;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> SquareRoot<T>(Vector3d<T> operand)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => new(
            T.Sqrt(operand.X),
            T.Sqrt(operand.Y),
            T.Sqrt(operand.Z)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Substract<T>(Vector3d<T> left, Vector3d<T> right)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => new(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z
            );

    /// <summary>
    /// Calculate sum of the <paramref name="operand"/>
    /// </summary>
    /// <param name="operand">The sum operand</param>
    /// <returns>The sum of the vector coordinates</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Sum<T>(Vector3d<T> operand)
        where T : unmanaged, INumberBase<T>
        => operand.X + operand.Y + operand.Z;
}
