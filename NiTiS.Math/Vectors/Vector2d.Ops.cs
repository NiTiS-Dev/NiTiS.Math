using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Vectors;

public static unsafe class Vector2d
{
    /// <summary>
    /// Convert <see cref="Vector2"/> to <see cref="Vector2d{T}"/>
    /// </summary>
    /// <param name="vector">Origin non-generic vector</param>
    /// <returns>The generic vector</returns>
    [MethodImpl(AggressiveOptimization | AggressiveInlining)]
    public static Vector2d<float> ConvertToGeneric(this Vector2 vector)
        => Unsafe.As<Vector2, Vector2d<float>>(ref vector);

    /// <summary>
    /// Convert <see cref="Vector2d{T}"/> to <see cref="Vector2"/>
    /// </summary>
    /// <param name="vector">Origin generic vector</param>
    /// <returns>The non-generic vector</returns>
    [MethodImpl(AggressiveOptimization | AggressiveInlining)]
    public static Vector2 ConvertToSystem(this Vector2d<float> vector)
        => Unsafe.As<Vector2d<float>, Vector2>(ref vector);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Abs<T>(Vector2d<T> vec)
        where T : unmanaged, INumberBase<T>
        => new(
            T.Abs(vec.X),
            T.Abs(vec.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Add<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>
        => left + right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Add<T>(Vector2d<T> left, T right)
        where T : unmanaged, INumberBase<T>
        => left + right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T AngleBetween<T>(Vector2d<T> left, Vector2d<T> right)
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
    public static Vector2d<T> Bitwise<T>(Vector2d<T> operand)
        where T : unmanaged, INumberBase<T>, IBitwiseOperators<T, T, T>
        => new(
            ~operand.X,
            ~operand.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Clamp<T>(Vector2d<T> value, Vector2d<T> min, Vector2d<T> max)
        where T : unmanaged, INumberBase<T>, INumber<T>
        => new(
            T.Clamp(value.X, min.X, max.X),
            T.Clamp(value.Y, min.Y, max.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Distance<T>(Vector2d<T> from, Vector2d<T> to)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => T.Sqrt(DistanceSquared(from, to));

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T DistanceSquared<T>(Vector2d<T> from, Vector2d<T> to)
        where T : unmanaged, INumberBase<T>
    {
        Vector2d<T> diff = from - to;
        return Dot(diff, diff);
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Divide<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>
        => left / right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Divide<T>(Vector2d<T> left, T right)
        where T : unmanaged, INumberBase<T>
        => left / right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Divide<T>(T left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>
        => new(
            left * right.X,
            left * right.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Dot<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>
        => left.X * right.X + left.Y * right.Y;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Length<T>(Vector2d<T> operand)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => T.Sqrt(operand.LengthSquared);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Lerp<T>(Vector2d<T> left, Vector2d<T> right, T amount)
        where T : unmanaged, INumberBase<T>
        => left * (T.One - amount) + right * amount;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Max<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumber<T>
        => new(
            T.Max(left.X, right.X),
            T.Max(left.Y, right.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Min<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumber<T>
        => new(
            T.Min(left.X, right.X),
            T.Min(left.Y, right.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Multiply<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Multiply<T>(Vector2d<T> left, T right)
        where T : unmanaged, INumberBase<T>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Multiply<T>(T left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>
        => new(
            left * right.X,
            left * right.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Negate<T>(Vector2d<T> operand)
        where T : unmanaged, INumberBase<T>
        => -operand;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Normalize<T>(Vector2d<T> operand)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => operand / Length(operand);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Reflect<T>(Vector2d<T> vector, Vector2d<T> normal)
        where T : unmanaged, INumberBase<T>
        => vector - Scalar<T>.Two * Dot(vector, normal) * normal;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> SquareRoot<T>(Vector2d<T> operand)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => new(
            T.Sqrt(operand.X),
            T.Sqrt(operand.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<T> Substract<T>(Vector2d<T> left, Vector2d<T> right)
        where T : unmanaged, INumberBase<T>, IRootFunctions<T>
        => new(
            left.X - right.X,
            left.Y - right.Y
            );

    /// <summary>
    /// Calculate sum of the <paramref name="operand"/>
    /// </summary>
    /// <param name="operand">The sum operand</param>
    /// <returns>The sum of the vector coordinates</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T Sum<T>(Vector2d<T> operand)
        where T : unmanaged, INumberBase<T>
        => operand.X + operand.Y;
}