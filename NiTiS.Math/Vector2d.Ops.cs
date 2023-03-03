using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

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
    public static Vector2d<N> Add<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => left + right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Add<N>(Vector2d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left + right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static F AngleBetween<F>(Vector2d<F> left, Vector2d<F> right)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>, ITrigonometricFunctions<F>, IComparisonOperators<F, F, bool>
    {
        F value = Dot(left, right);
        F v1Length = Length(left);
        F v2Length = Length(right);

        value /= v1Length * v2Length;

        if (value <= -F.One) return F.Pi;
        if (value >= F.One) return F.Zero;
        return F.Acos(value);
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Bitwise<N>(Vector2d<N> operand)
        where N : unmanaged, INumberBase<N>, IBitwiseOperators<N, N, N>
        => new(
            ~operand.X,
            ~operand.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Clamp<N>(Vector2d<N> value, Vector2d<N> min, Vector2d<N> max)
        where N : unmanaged, INumberBase<N>, INumber<N>
        => new(
            N.Clamp(value.X, min.X, max.X),
            N.Clamp(value.Y, min.Y, max.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N Distance<N>(Vector2d<N> from, Vector2d<N> to)
        where N : unmanaged, INumberBase<N>, IRootFunctions<N>
        => N.Sqrt(DistanceSquared(from, to));

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N DistanceSquared<N>(Vector2d<N> from, Vector2d<N> to)
        where N : unmanaged, INumberBase<N>
    {
        Vector2d<N> diff = from - to;
        return Dot(diff, diff);
    }

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Divide<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => left / right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Divide<N>(Vector2d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left / right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Divide<N>(N left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left * right.X,
            left * right.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N Dot<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => left.X * right.X + left.Y * right.Y;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N Dot<N>(Vector2d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left.X * right + left.Y * right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static F Length<F>(Vector2d<F> operand)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>
        => F.Sqrt(operand.LengthSquared);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Lerp<N>(Vector2d<N> left, Vector2d<N> right, N amount)
        where N : unmanaged, INumberBase<N>
        => left * (N.One - amount) + right * amount;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Max<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumber<N>
        => new(
            N.Max(left.X, right.X),
            N.Max(left.Y, right.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Min<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumber<N>
        => new(
            N.Min(left.X, right.X),
            N.Min(left.Y, right.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Multiply<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Multiply<N>(Vector2d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Multiply<N>(N left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left * right.X,
            left * right.Y
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Negate<N>(Vector2d<N> operand)
        where N : unmanaged, INumberBase<N>
        => -operand;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<F> Normalize<F>(Vector2d<F> operand)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>
        => operand / Length(operand);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Reflect<N>(Vector2d<N> vector, Vector2d<N> normal)
        where N : unmanaged, INumberBase<N>
        => vector - Scalar<N>.Two * Dot(vector, normal) * normal;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<F> SquareRoot<F>(Vector2d<F> operand)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>
        => new(
            F.Sqrt(operand.X),
            F.Sqrt(operand.Y)
            );

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector2d<N> Substract<N>(Vector2d<N> left, Vector2d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left.X - right.X,
            left.Y - right.Y
            );

    /// <summary>
    /// Calculate sum of the <paramref name="operand"/>.
    /// </summary>
    /// <param name="operand">The sum operand.</param>
    /// <returns>The sum of the vector coordinates.</returns>
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N Sum<N>(Vector2d<N> operand)
        where N : unmanaged, INumberBase<N>
        => operand.X + operand.Y;
}