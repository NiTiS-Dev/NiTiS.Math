using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static class Vector4D
{
	// Cast to System.Numerics
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4D<float> ConvertToGeneric(this Vector4 vector)
		=> Unsafe.As<Vector4, Vector4D<float>>(ref vector);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4 ConvertToSystem(this Vector4D<float> vector)
		=> Unsafe.As<Vector4D<float>, Vector4>(ref vector);

	// Cast to System.Runtime.Intrinsics
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4D<TO> ConvertFromSystem<FROM, TO>(this Vector128<FROM> vector)
		where TO : unmanaged, INumberBase<TO>
		where FROM : struct
		=> Unsafe.As<Vector128<FROM>, Vector4D<TO>>(ref vector);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector128<TO> ConvertToVec128<FROM, TO>(this Vector4D<FROM> vector)
		where TO : struct
		where FROM : unmanaged, INumberBase<FROM>
		=> Unsafe.As< Vector4D<FROM>, Vector128<TO>>(ref vector);

	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector4D<TO> ConvertFromSystem<FROM, TO>(this Vector256<FROM> vector)
		where TO : unmanaged, INumberBase<TO>
		where FROM : struct
		=> Unsafe.As<Vector256<FROM>, Vector4D<TO>>(ref vector);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Vector256<TO> ConvertToVec256<FROM, TO>(this Vector4D<FROM> vector)
		where TO : struct
		where FROM : unmanaged, INumberBase<FROM>
		=> Unsafe.As<Vector4D<FROM>, Vector256<TO>>(ref vector);


	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Abs<T>(Vector4D<T> vec)
		where T : unmanaged, INumberBase<T>
		=> new(
			T.Abs(vec.X),
			T.Abs(vec.Y),
			T.Abs(vec.Z),
			T.Abs(vec.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Add<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left + right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Add<T>(Vector4D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left + right;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T AngleBetween<T>(Vector4D<T> left, Vector4D<T> right)
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
	public static Vector4D<T> Bitwise<T>(Vector4D<T> operand)
		where T : unmanaged, INumberBase<T>, IBitwiseOperators<T, T, T>
		=> new(
			~operand.X,
			~operand.Y,
			~operand.Z,
			~operand.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Clamp<T>(Vector4D<T> value, Vector4D<T> min, Vector4D<T> max)
		where T : unmanaged, INumberBase<T>, INumber<T>
		=> new(
			T.Clamp(value.X, min.X, max.X),
			T.Clamp(value.Y, min.Y, max.Y),
			T.Clamp(value.Z, min.Z, max.Z),
			T.Clamp(value.W, min.W, max.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Distance<T>(Vector4D<T> from, Vector4D<T> to)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> T.Sqrt(DistanceSquared(from, to));

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T DistanceSquared<T>(Vector4D<T> from, Vector4D<T> to)
		where T : unmanaged, INumberBase<T>
	{
		Vector4D<T> diff = from - to;
		return Dot(diff, diff);
	}

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Divide<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Divide<T>(Vector4D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left / right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Divide<T>(T left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z,
			left * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Dot<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static T Length<T>(Vector4D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> T.Sqrt(operand.LengthSquared);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> Lerp<T>(Vector3D<T> left, Vector3D<T> right, T amount)
		where T : unmanaged, INumberBase<T>
		=> (left * (T.One - amount)) + (right * amount);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Max<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumber<T>
		=> new(
			T.Max(left.X, right.X),
			T.Max(left.Y, right.Y),
			T.Max(left.Z, right.Z),
			T.Max(left.W, right.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Min<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumber<T>
		=> new(
			T.Min(left.X, right.X),
			T.Min(left.Y, right.Y),
			T.Min(left.Z, right.Z),
			T.Min(left.W, right.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Multiply<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Multiply<T>(Vector4D<T> left, T right)
		where T : unmanaged, INumberBase<T>
		=> left * right;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Multiply<T>(T left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z,
			left * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Negate<T>(Vector4D<T> operand)
		where T : unmanaged, INumberBase<T>
		=> -operand;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Normalize<T>(Vector4D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> operand / Length(operand);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Reflect<T>(Vector4D<T> vector, Vector4D<T> normal, T two)
		where T : unmanaged, INumberBase<T>
		=> vector - (two * Dot(vector, normal)) * normal;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> SquareRoot<T>(Vector4D<T> operand)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> new(
			T.Sqrt(operand.X),
			T.Sqrt(operand.Y),
			T.Sqrt(operand.Z),
			T.Sqrt(operand.W)
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> Substract<T>(Vector4D<T> left, Vector4D<T> right)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z,
			left.W - right.W
			);
}
