using NiTiS.Core.Operators;
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
public unsafe readonly struct Vector3D<T> :
	// Vector op Vector
	IAdditionOperators<Vector3D<T>, Vector3D<T>, Vector3D<T>>,
	ISubtractionOperators<Vector3D<T>, Vector3D<T>, Vector3D<T>>,
	IDivisionOperators<Vector3D<T>, Vector3D<T>, Vector3D<T>>,
	IMultiplyOperators<Vector3D<T>, Vector3D<T>, Vector3D<T>>,
	IEqualityOperators<Vector3D<T>, Vector3D<T>, bool>,
	// Vector op T
	IAdditionOperators<Vector3D<T>, T, Vector3D<T>>,
	ISubtractionOperators<Vector3D<T>, T, Vector3D<T>>,
	IDivisionOperators<Vector3D<T>, T, Vector3D<T>>,
	IMultiplyOperators<Vector3D<T>, T, Vector3D<T>>,
	// Unary op
	IUnaryNegationOperators<Vector3D<T>, Vector3D<T>>,
	IUnaryPlusOperators<Vector3D<T>, Vector3D<T>>,
	IFormattable,
	IEquatable<Vector3D<T>>,
	// Cast op
	IImplicitCastOperators<Vector3D<T>, Vector2D<T>>,
	IExplicitCastOperators<Vector3D<T>, Vector4D<T>>
	where T :
		unmanaged,
		INumberBase<T>
{
	public readonly T X;
	public readonly T Y;
	public readonly T Z;
	public readonly T LengthSquared
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => Vector3D.Dot(this, this);
	}

	public const int ElementCount = 3;
	private static readonly int VectorSize = sizeof(T) * ElementCount;

	public Vector3D(ReadOnlySpan<T> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector3D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
	}
	public Vector3D(ReadOnlySpan<byte> data)
	{
		if (data.Length < VectorSize)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector3D<T>>(ref MemoryMarshal.GetReference(data));
	}
	public Vector3D(ReadOnlySpan<T> data, int offset)
	{
		if (data.Length < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector3D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data.Slice(offset))));
	}
	public Vector3D(ReadOnlySpan<byte> data, int offset)
	{
		if (data.Length < VectorSize + offset)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector3D<T>>(ref MemoryMarshal.GetReference(data.Slice(offset)));
	}
	public Vector3D(Vector2D<T> base2, T z)
		=> (X, Y, Z) = (base2.X, base2.Y, z);
	public Vector3D(T x, T y, T z)
		=> (X, Y, Z) = (x, y, z);
	public Vector3D(T xyz)
		=> (X, Y, Z) = (xyz, xyz, xyz);
	public static Vector3D<T> One => new(T.One, T.One, T.One);
	public static Vector3D<T> Zero => new(T.Zero, T.Zero, T.Zero);
	public static Vector3D<T> UnitX => new(T.One, T.Zero, T.Zero);
	public static Vector3D<T> UnitY => new(T.Zero, T.One, T.Zero);
	public static Vector3D<T> UnitZ => new(T.Zero, T.Zero, T.One);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator +(Vector3D<T> left, Vector3D<T> right)
		=> new(
			left.X + right.X,
			left.Y + right.Y,
			left.Z + right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator -(Vector3D<T> left, Vector3D<T> right)
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator /(Vector3D<T> left, Vector3D<T> right)
		=> new(
			left.X / right.X,
			left.Y / right.Y,
			left.Z / right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator *(Vector3D<T> left, Vector3D<T> right)
		=> new(
			left.X * right.X,
			left.Y * right.Y,
			left.Z * right.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator ==(Vector3D<T> left, Vector3D<T> right)
		=> left.X == right.X
		&& left.Y == right.Y
		&& left.Z == right.Z;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator !=(Vector3D<T> left, Vector3D<T> right)
		=> left.X != right.X
		|| left.Y != right.Y
		|| left.Z != right.Z;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator +(Vector3D<T> left, T right)
		=> new(
			left.X + right,
			left.Y + right,
			left.Z + right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator -(Vector3D<T> left, T right)
		=> new(
			left.X - right,
			left.Y - right,
			left.Z - right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator /(Vector3D<T> left, T right)
		=> new(
			left.X / right,
			left.Y / right,
			left.Z / right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator /(T left, Vector3D<T> right)
		=> new(
			left / right.X,
			left / right.Y,
			left / right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator *(Vector3D<T> left, T right)
		=> new(
			left.X * right,
			left.Y * right,
			left.Z * right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator *(T left, Vector3D<T> right)
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator -(Vector3D<T> operand)
		=> new(
			-operand.X,
			-operand.Y,
			-operand.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3D<T> operator +(Vector3D<T> operand)
		=> new(
			+operand.X,
			+operand.Y,
			+operand.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static implicit operator Vector2D<T>(Vector3D<T> operand)
		=> new(
			operand.X,
			operand.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static explicit operator Vector4D<T>(Vector3D<T> operand)
		=> new(
			operand.X,
			operand.Y,
			operand.Z,
			T.One
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
	}
	public readonly void CopyTo(Span<T> array, uint offset)
	{
		if (array.Length < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(array));

		array[0 + (int)offset] = X;
		array[1 + (int)offset] = Y;
		array[2 + (int)offset] = Z;
	}

	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y, Z);
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Vector3D<T> vec
		? vec == this : false;
	public readonly bool Equals(Vector3D<T> other)
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

		sb.Append('>');
		return sb.ToString();
	}
}
