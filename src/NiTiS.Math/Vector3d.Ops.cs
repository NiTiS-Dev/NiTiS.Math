using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static class Vector3d
{
	#region Convert
	/// <summary>
	/// Convert <see cref="Vector3"/> to <see cref="Vector3d{T}"/>.
	/// </summary>
	/// <param name="vector">Origin non-generic vector.</param>
	/// <returns>The generic vector.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
    public static Vector3d<float> ConvertToGeneric(this Vector3 vector)
        => Unsafe.As<Vector3, Vector3d<float>>(ref vector);

    /// <summary>
    /// Convert <see cref="Vector3d{T}"/> to <see cref="Vector3"/>.
    /// </summary>
    /// <param name="vector">Origin generic vector.</param>
    /// <returns>The non-generic vector.</returns>
    [MethodImpl(AggressiveOptimization | AggressiveInlining)]
    public static Vector3 ConvertToSystem(this Vector3d<float> vector)
        => Unsafe.As<Vector3d<float>, Vector3>(ref vector);
	#endregion

	#region Abs
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<T> Abs<T>(Vector3d<T> vec)
        where T : unmanaged, INumberBase<T>
        => new(
            T.Abs(vec.X),
            T.Abs(vec.Y),
            T.Abs(vec.Z)
            );
	#endregion

	#region Add
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Add<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => left + right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Add<N>(Vector3d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left + right;
	#endregion

	#region AngleBetween
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static F AngleBetween<F>(Vector3d<F> left, Vector3d<F> right)
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
	#endregion

	#region Bitwise
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Bitwise<N>(Vector3d<N> operand)
        where N : unmanaged, INumberBase<N>, IBitwiseOperators<N, N, N>
        => new(
            ~operand.X,
            ~operand.Y,
            ~operand.Z
            );
	#endregion

	#region Clamp
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Clamp<N>(Vector3d<N> value, Vector3d<N> min, Vector3d<N> max)
        where N : unmanaged, INumber<N>
        => new(
            N.Clamp(value.X, min.X, max.X),
            N.Clamp(value.Y, min.Y, max.Y),
            N.Clamp(value.Z, min.Z, max.Z)
            );
	#endregion

	#region Cross
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Cross<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left.Y * right.Z - left.Z * right.Y,
            left.Z * right.X - left.X * right.Z,
            left.X * right.Y - left.Y * right.X
        );
	#endregion

	#region Distance
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static F Distance<F>(Vector3d<F> from, Vector3d<F> to)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>
        => F.Sqrt(DistanceSquared(from, to));

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N DistanceSquared<N>(Vector3d<N> from, Vector3d<N> to)
        where N : unmanaged, INumberBase<N>
    {
        Vector3d<N> diff = from - to;
        return Dot(diff, diff);
    }
	#endregion

	#region Divide
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Divide<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => left / right;

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Divide<N>(Vector3d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left / right;
    
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Divide<N>(N left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left * right.X,
            left * right.Y,
            left * right.Z
            );
	#endregion

	#region Dot
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N Dot<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => left.X * right.X + left.Y * right.Y + left.Z * right.Z;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static N Dot<N>(Vector3d<N> left, N right)
		where N : unmanaged, INumberBase<N>
		=> left.X * right + left.Y * right + left.Z * right;
	#endregion

	#region Length
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static F Length<F>(Vector3d<F> operand)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>
        => F.Sqrt(operand.LengthSquared);
	#endregion

	#region Lerp
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Lerp<N>(Vector3d<N> left, Vector3d<N> right, N amount)
        where N : unmanaged, INumberBase<N>
        => left * (N.One - amount) + right * amount;
	#endregion

	#region Max
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Max<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumber<N>
        => new(
            N.Max(left.X, right.X),
            N.Max(left.Y, right.Y),
            N.Max(left.Z, right.Z)
            );
	#endregion

	#region Min
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Min<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumber<N>
        => new(
            N.Min(left.X, right.X),
            N.Min(left.Y, right.Y),
            N.Min(left.Z, right.Z)
            );
	#endregion

	#region Multiply
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Multiply<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Multiply<N>(Vector3d<N> left, N right)
        where N : unmanaged, INumberBase<N>
        => left * right;
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Multiply<N>(N left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left * right.X,
            left * right.Y,
            left * right.Z
            );
	#endregion

	#region Negate
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Negate<N>(Vector3d<N> operand)
        where N : unmanaged, INumberBase<N>
        => -operand;
	#endregion

	#region Normalize
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Normalize<N>(Vector3d<N> operand)
        where N : unmanaged, INumberBase<N>, IRootFunctions<N>
        => operand / Length(operand);
	#endregion

	#region Reflect
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Reflect<N>(Vector3d<N> vector, Vector3d<N> normal)
        where N : unmanaged, INumberBase<N>
        => vector - Scalar<N>.Two * Dot(vector, normal) * normal;
	#endregion

	#region Root
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<F> SquareRoot<F>(Vector3d<F> operand)
        where F : unmanaged, INumberBase<F>, IRootFunctions<F>
        => new(
            F.Sqrt(operand.X),
            F.Sqrt(operand.Y),
            F.Sqrt(operand.Z)
            );
	#endregion

	#region Substract
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Vector3d<N> Substract<N>(Vector3d<N> left, Vector3d<N> right)
        where N : unmanaged, INumberBase<N>
        => new(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z
            );

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> Substract<N>(Vector3d<N> left, N right)
		where N : unmanaged, INumberBase<N>
		=> new(
			left.X - right,
			left.Y - right,
			left.Z - right
			);
	#endregion

	#region Sum
	/// <summary>
	/// Calculate sum of the <paramref name="operand"/>.
	/// </summary>
	/// <param name="operand">The sum operand.</param>
	/// <returns>The sum of the vector coordinates.</returns>
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static N Sum<N>(Vector3d<N> operand)
        where N : unmanaged, INumberBase<N>
        => operand.X + operand.Y + operand.Z;
	#endregion
}
