using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

/// <summary>
/// <see cref="Vector4d{N}"/> operations provider.
/// </summary>
public static class Vector4d
{
	/// <summary>
	/// Convert <see cref="Vector4"/> to <see cref="Vector4d{T}"/>.
	/// </summary>
	/// <param name="vector">Origin non-generic vector.</param>
	/// <returns>The generic vector.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4d<float> ConvertToGeneric(this Vector4 vector)
		=> Unsafe.As<Vector4, Vector4d<float>>(ref vector);

	/// <summary>
	/// Convert <see cref="Vector4d{T}"/> to <see cref="Vector4"/>.
	/// </summary>
	/// <param name="vector">Origin generic vector.</param>
	/// <returns>The non-generic vector.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4 ConvertToSystem(this Vector4d<float> vector)
		=> Unsafe.As<Vector4d<float>, Vector4>(ref vector);

	// Cast to System.Runtime.Intrinsics
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4d<TO> ConvertFromSystem<FROM, TO>(this Vector128<FROM> vector)
		where TO : unmanaged, INumberBase<TO>
		where FROM : struct
		=> Unsafe.As<Vector128<FROM>, Vector4d<TO>>(ref vector);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector128<TO> ConvertToVec128<FROM, TO>(this Vector4d<FROM> vector)
		where TO : struct
		where FROM : unmanaged, INumberBase<FROM>
		=> Unsafe.As<Vector4d<FROM>, Vector128<TO>>(ref vector);

	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4d<TO> ConvertFromSystem<FROM, TO>(this Vector256<FROM> vector)
		where TO : unmanaged, INumberBase<TO>
		where FROM : struct
		=> Unsafe.As<Vector256<FROM>, Vector4d<TO>>(ref vector);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector256<TO> ConvertToVec256<FROM, TO>(this Vector4d<FROM> vector)
		where TO : struct
		where FROM : unmanaged, INumberBase<FROM>
		=> Unsafe.As<Vector4d<FROM>, Vector256<TO>>(ref vector);


	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Abs<N>(Vector4d<N> vec)
		where N : unmanaged, INumberBase<N>
		=> new(
			N.Abs(vec.X),
			N.Abs(vec.Y),
			N.Abs(vec.Z),
			N.Abs(vec.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Add<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> left + right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Add<N>(Vector4d<N> left, N right)
		where N : unmanaged, INumberBase<N>
		=> left + right;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static F AngleBetween<F>(Vector4d<F> left, Vector4d<F> right)
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
	public static Vector4d<N> Bitwise<N>(Vector4d<N> operand)
		where N : unmanaged, INumberBase<N>, IBitwiseOperators<N, N, N>
		=> new(
			~operand.X,
			~operand.Y,
			~operand.Z,
			~operand.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Clamp<N>(Vector4d<N> value, Vector4d<N> min, Vector4d<N> max)
		where N : unmanaged, INumberBase<N>, INumber<N>
		=> new(
			N.Clamp(value.X, min.X, max.X),
			N.Clamp(value.Y, min.Y, max.Y),
			N.Clamp(value.Z, min.Z, max.Z),
			N.Clamp(value.W, min.W, max.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static F Distance<F>(Vector4d<F> from, Vector4d<F> to)
		where F : unmanaged, INumberBase<F>, IRootFunctions<F>
		=> F.Sqrt(DistanceSquared(from, to));

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static N DistanceSquared<N>(Vector4d<N> from, Vector4d<N> to)
		where N : unmanaged, INumberBase<N>
	{
		Vector4d<N> diff = from - to;
		return Dot(diff, diff);
	}

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Divide<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Divide<N>(Vector4d<N> left, N right)
		where N : unmanaged, INumberBase<N>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Divide<N>(N left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z,
			left * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static N Dot<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static N Dot<N>(Vector4d<N> left, N right)
		where N : unmanaged, INumberBase<N>
		=> left.X * right + left.Y * right + left.Z * right + left.W * right;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static F Length<F>(Vector4d<F> operand)
		where F : unmanaged, INumberBase<F>, IRootFunctions<F>
		=> F.Sqrt(operand.LengthSquared);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> Lerp<N>(Vector3d<N> left, Vector3d<N> right, N amount)
		where N : unmanaged, INumberBase<N>
		=> left * (N.One - amount) + right * amount;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Max<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumber<N>
		=> new(
			N.Max(left.X, right.X),
			N.Max(left.Y, right.Y),
			N.Max(left.Z, right.Z),
			N.Max(left.W, right.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Min<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumber<N>
		=> new(
			N.Min(left.X, right.X),
			N.Min(left.Y, right.Y),
			N.Min(left.Z, right.Z),
			N.Min(left.W, right.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Multiply<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Multiply<N>(Vector4d<N> left, N right)
		where N : unmanaged, INumberBase<N>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Multiply<N>(N left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z,
			left * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Negate<N>(Vector4d<N> operand)
		where N : unmanaged, INumberBase<N>
		=> -operand;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<F> Normalize<F>(Vector4d<F> operand)
		where F : unmanaged, INumberBase<F>, IRootFunctions<F>
		=> operand / Length(operand);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Reflect<N>(Vector4d<N> vector, Vector4d<N> normal)
		where N : unmanaged, INumberBase<N>
		=> vector - Scalar<N>.Two * Dot(vector, normal) * normal;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<F> SquareRoot<F>(Vector4d<F> operand)
		where F : unmanaged, INumberBase<F>, IRootFunctions<F>
		=> new(
			F.Sqrt(operand.X),
			F.Sqrt(operand.Y),
			F.Sqrt(operand.Z),
			F.Sqrt(operand.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<N> Substract<N>(Vector4d<N> left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z,
			left.W - right.W
			);

	/// <summary>
	/// Calculate sum of the <paramref name="operand"/>
	/// </summary>
	/// <param name="operand">The sum operand</param>
	/// <returns>The sum of the vector coordinates</returns>
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static N Sum<N>(Vector4d<N> operand)
		where N : unmanaged, INumberBase<N>
		=> operand.X + operand.Y + operand.Z + operand.W;
}
