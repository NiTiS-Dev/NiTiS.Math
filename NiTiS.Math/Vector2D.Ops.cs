using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static class Vector2D
{
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Abs<T>(Vector2D<T> vec)
		where T : unmanaged, INumberBase<T>
		=> new(
			T.Abs(vec.X),
			T.Abs(vec.Y)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Add<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left + right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Add<T>(Vector2D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left + right;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Bitwise<T>(Vector2D<T> operand)
		where T : unmanaged, INumberBase<T>, IBitwiseOperators<T, T, T>
		=> new(
			~operand.X,
			~operand.Y
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Clamp<T>(Vector2D<T> value, Vector2D<T> min, Vector2D<T> max)
		where T : unmanaged, INumberBase<T>, INumber<T>
		=> new(
			T.Clamp(value.X, min.X, max.X),
			T.Clamp(value.Y, min.Y, max.Y)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Distance<T>(Vector2D<T> from, Vector2D<T> to)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> T.Sqrt(DistanceSquared(from, to));

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T DistanceSquared<T>(Vector2D<T> from, Vector2D<T> to)
		where T : unmanaged, INumberBase<T>
	{
		Vector2D<T> diff = from - to;
		return Dot(diff, diff);
	}

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Divide<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Divide<T>(Vector2D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Divide<T>(T left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			left * right.X,
			left * right.Y
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Dot<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left.X * right.X + left.Y * right.Y;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Length<T>(Vector2D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> T.Sqrt(operand.LengthSquared);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Lerp<T>(Vector2D<T> left, Vector2D<T> right, T amount)
		where T : unmanaged, INumberBase<T>
		=> (left * (T.One - amount)) + (right * amount);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Max<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumber<T>
		=> new(
			T.Max(left.X, right.X),
			T.Max(left.Y, right.Y)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Min<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumber<T>
		=> new(
			T.Min(left.X, right.X),
			T.Min(left.Y, right.Y)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Multiply<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Multiply<T>(Vector2D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Multiply<T>(T left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			left * right.X,
			left * right.Y
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Negate<T>(Vector2D<T> operand)
		where T : unmanaged, INumberBase<T>
		=> -operand;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Normalize<T>(Vector2D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> operand / Length(operand);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Reflect<T>(Vector2D<T> vector, Vector2D<T> normal)
		where T : unmanaged, INumberBase<T>
		=> vector - (Scalar<T>.Two * Dot(vector, normal)) * normal;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> SquareRoot<T>(Vector2D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> new(
			T.Sqrt(operand.X),
			T.Sqrt(operand.Y)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2D<T> Substract<T>(Vector2D<T> left, Vector2D<T> right)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> new(
			left.X - right.X,
			left.Y - right.Y
			);
}