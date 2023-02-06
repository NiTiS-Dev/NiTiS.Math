using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

[DebuggerDisplay($@"{{{nameof(ToString)}(""G""),nq}}")]
public readonly unsafe struct Vector4D<T> :
	// Vector op Vector
	IAdditionOperators<Vector4D<T>, Vector4D<T>, Vector4D<T>>,
	ISubtractionOperators<Vector4D<T>, Vector4D<T>, Vector4D<T>>,
	IDivisionOperators<Vector4D<T>, Vector4D<T>, Vector4D<T>>,
	IMultiplyOperators<Vector4D<T>, Vector4D<T>, Vector4D<T>>,
	IEqualityOperators<Vector4D<T>, Vector4D<T>, bool>,
	// Vector op T
	IAdditionOperators<Vector4D<T>, T, Vector4D<T>>,
	ISubtractionOperators<Vector4D<T>, T, Vector4D<T>>,
	IDivisionOperators<Vector4D<T>, T, Vector4D<T>>,
	IMultiplyOperators<Vector4D<T>, T, Vector4D<T>>,
	// Unary op
	IUnaryNegationOperators<Vector4D<T>, Vector4D<T>>,
	IUnaryPlusOperators<Vector4D<T>, Vector4D<T>>,
	IFormattable,
	IEquatable<Vector4D<T>>
	where T :
		unmanaged,
		INumberBase<T>
{
	public readonly T X;
	public readonly T Y;
	public readonly T Z;
	public readonly T W;
	public readonly T LengthSquared
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => Vector4D.Dot(this, this);
	}

	public const int ElementCount = 4;
	private static readonly int VectorSize = sizeof(T) * ElementCount;
	public Vector4D(ReadOnlySpan<T> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
	}
	public Vector4D(ReadOnlySpan<byte> data)
	{
		if (data.Length < VectorSize)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector4D<T>>(ref MemoryMarshal.GetReference(data));
	}
	public Vector4D(ReadOnlySpan<T> data, int offset)
	{
		if (data.Length < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data.Slice(offset))));
	}
	public Vector4D(ReadOnlySpan<byte> data, int offset)
	{
		if (data.Length < VectorSize + offset)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector4D<T>>(ref MemoryMarshal.GetReference(data.Slice(offset)));
	}
	public Vector4D(Vector3D<T> base3, T w)
		=> (X, Y, Z, W) = (base3.X, base3.Y, base3.Z, w);
	public Vector4D(Vector2D<T> base2, T z, T w)
		=> (X, Y, Z, W) = (base2.X, base2.Y, z, w);
	public Vector4D(T x, T y, T z, T w)
		=> (X, Y, Z, W) = (x, y, z, w);
	public Vector4D(T xyzw)
		=> (X, Y, Z, W) = (xyzw, xyzw, xyzw, xyzw);
	public static Vector4D<T> One => new(T.One, T.One, T.One, T.One);
	public static Vector4D<T> Zero => new(T.Zero, T.Zero, T.Zero, T.Zero);
	public static Vector4D<T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero);
	public static Vector4D<T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero);
	public static Vector4D<T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero);
	public static Vector4D<T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator +(Vector4D<T> left, Vector4D<T> right)
		=> new(
			left.X + right.X,
			left.Y + right.Y,
			left.Z + right.Z,
			left.W + right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator -(Vector4D<T> left, Vector4D<T> right)
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z,
			left.W - right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator /(Vector4D<T> left, Vector4D<T> right)
		=> new(
			left.X / right.X,
			left.Y / right.Y,
			left.Z / right.Z,
			left.W / right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator *(Vector4D<T> left, Vector4D<T> right)
		=> new(
			left.X * right.X,
			left.Y * right.Y,
			left.Z * right.Z,
			left.W * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator ==(Vector4D<T> left, Vector4D<T> right)
		=> left.X == right.X
		&& left.Y == right.Y
		&& left.Z == right.Z
		&& left.W == right.W;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator !=(Vector4D<T> left, Vector4D<T> right)
		=> left.X != right.X
		|| left.Y != right.Y
		|| left.Z != right.Z
		|| left.W != right.W;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator +(Vector4D<T> left, T right)
		=> new(
			left.X + right,
			left.Y + right,
			left.Z + right,
			left.W + right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator -(Vector4D<T> left, T right)
		=> new(
			left.X - right,
			left.Y - right,
			left.Z - right,
			left.W - right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator /(Vector4D<T> left, T right)
		=> new(
			left.X / right,
			left.Y / right,
			left.Z / right,
			left.W / right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator /(T left, Vector4D<T> right)
		=> new(
			left / right.X,
			left / right.Y,
			left / right.Z,
			left / right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator *(Vector4D<T> left, T right)
		=> new(
			left.X * right,
			left.Y * right,
			left.Z * right,
			left.W * right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator *(T left, Vector4D<T> right)
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z,
			left * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator -(Vector4D<T> operand)
		=> new(
			-operand.X,
			-operand.Y,
			-operand.Z,
			-operand.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4D<T> operator +(Vector4D<T> operand)
		=> new(
			+operand.X,
			+operand.Y,
			+operand.Z,
			+operand.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static implicit operator Vector2D<T>(Vector4D<T> operand)
		=> new(
			operand.X,
			operand.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static implicit operator Vector3D<T>(Vector4D<T> operand)
		=> new(
			operand.X,
			operand.Y,
			operand.Z
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
		array[3 + offset] = W;
	}
	public readonly void CopyTo(Span<T> array, uint offset)
	{
		if (array.Length < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(array));

		array[0 + (int)offset] = X;
		array[1 + (int)offset] = Y;
		array[2 + (int)offset] = Z;
		array[3 + (int)offset] = W;
	}

	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y, Z, W);
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Vector4D<T> vec
		? vec == this : false;
	public readonly bool Equals(Vector4D<T> other)
		=> this == other;
	public override readonly string ToString() => ToString("G", CultureInfo.CurrentCulture);
	public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
	{
		StringBuilder sb = new();
		string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
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
		sb.Append(separator);
		if (!string.IsNullOrWhiteSpace(separator))
			sb.Append(' ');

		sb.Append(W.ToString(format, formatProvider));

		sb.Append('>');
		return sb.ToString();
	}
}