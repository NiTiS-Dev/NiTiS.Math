using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static class Vector3D
{
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Abs<T>(Vector3D<T> vec)
		where T : unmanaged, INumberBase<T>
		=> new(
			T.Abs(vec.X),
			T.Abs(vec.Y),
			T.Abs(vec.Z)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Add<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left + right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Add<T>(Vector3D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left + right;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Bitwise<T>(Vector3D<T> operand)
		where T : unmanaged, INumberBase<T>, IBitwiseOperators<T, T, T>
		=> new(
			~operand.X,
			~operand.Y,
			~operand.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Clamp<T>(Vector3D<T> value, Vector3D<T> min, Vector3D<T> max)
		where T : unmanaged, INumberBase<T>, INumber<T>
		=> new(
			T.Clamp(value.X, min.X, max.X),
			T.Clamp(value.Y, min.Y, max.Y),
			T.Clamp(value.Z, min.Z, max.Z)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Cross<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			(left.Y * right.Z) - (left.Z * right.Y),
			(left.Z * right.X) - (left.X * right.Z),
			(left.X * right.Y) - (left.Y * right.X)
		);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Distance<T>(Vector3D<T> from, Vector3D<T> to)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> T.Sqrt(DistanceSquared(from, to));

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T DistanceSquared<T>(Vector3D<T> from, Vector3D<T> to)
		where T : unmanaged, INumberBase<T>
	{
		Vector3D<T> diff = from - to;
		return Dot(diff, diff);
	}

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Divide<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Divide<T>(Vector3D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Divide<T>(T left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Dot<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left.X * right.X + left.Y * right.Y + left.Z * right.Z;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Length<T>(Vector3D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> T.Sqrt(operand.LengthSquared);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Lerp<T>(Vector3D<T> left, Vector3D<T> right, T amount)
		where T : unmanaged, INumberBase<T>
		=> (left * (T.One - amount)) + (right * amount);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Max<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumber<T>
		=> new(
			T.Max(left.X, right.X),
			T.Max(left.Y, right.Y),
			T.Max(left.Z, right.Z)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Min<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumber<T>
		=> new(
			T.Min(left.X, right.X),
			T.Min(left.Y, right.Y),
			T.Min(left.Z, right.Z)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Multiply<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Multiply<T>(Vector3D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Multiply<T>(T left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Negate<T>(Vector3D<T> operand)
		where T : unmanaged, INumberBase<T>
		=> -operand;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Normalize<T>(Vector3D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> operand / Length(operand);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Reflect<T>(Vector3D<T> vector, Vector3D<T> normal, T two)
		where T : unmanaged, INumberBase<T>
		=> vector - (two * Dot(vector, normal)) * normal;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> SquareRoot<T>(Vector3D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> new(
			T.Sqrt(operand.X),
			T.Sqrt(operand.Y),
			T.Sqrt(operand.Z)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Substract<T>(Vector3D<T> left, Vector3D<T> right)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z
			);
}
