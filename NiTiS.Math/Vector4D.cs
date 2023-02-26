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
public unsafe struct Vector4d<T> :
	// Vector op Vector
	IAdditionOperators<Vector4d<T>, Vector4d<T>, Vector4d<T>>,
	ISubtractionOperators<Vector4d<T>, Vector4d<T>, Vector4d<T>>,
	IDivisionOperators<Vector4d<T>, Vector4d<T>, Vector4d<T>>,
	IMultiplyOperators<Vector4d<T>, Vector4d<T>, Vector4d<T>>,
	IEqualityOperators<Vector4d<T>, Vector4d<T>, bool>,
	// Vector op T
	IAdditionOperators<Vector4d<T>, T, Vector4d<T>>,
	ISubtractionOperators<Vector4d<T>, T, Vector4d<T>>,
	IDivisionOperators<Vector4d<T>, T, Vector4d<T>>,
	IMultiplyOperators<Vector4d<T>, T, Vector4d<T>>,
	// Unary op
	IUnaryNegationOperators<Vector4d<T>, Vector4d<T>>,
	IUnaryPlusOperators<Vector4d<T>, Vector4d<T>>,
	IFormattable,
	IEquatable<Vector4d<T>>
	where T :
		unmanaged,
		INumberBase<T>
{
	/// <summary>
	/// X value of vector, the first dimension.
	/// </summary>
	public T X;
	/// <summary>
	/// Y value of vector, the second dimension.
	/// </summary>
	public T Y;
	/// <summary>
	/// Z value of vector, the third dimension.
	/// </summary>
	public T Z;
	/// <summary>
	/// W value of vector, the fourth dimension.
	/// </summary>
	public T W;

	/// <summary>
	/// Squared magnitude of vector.
	/// </summary>
	public readonly T LengthSquared
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => Vector4d.Dot(this, this);
	}

	public const int ElementCount = 4;
	private static readonly int VectorSize = sizeof(T) * ElementCount;

	/// <summary>
	/// Creates new fourth-dimensional vector with values <c>(<paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>, <paramref name="z"/>)</c>.
	/// </summary>
	/// <param name="x">X value of vector.</param>
	/// <param name="y">Y value of vector.</param>
	/// <param name="z">Z value of vector.</param>
	/// <param name="w">W value of vector.</param>
	public Vector4d(T x, T y, T z, T w)
		=> (X, Y, Z, W) = (x, y, z, w);

	/// <summary>
	/// Creates new fourth-dimensional vector with values <c>(<paramref name="xyzw"/>, <paramref name="xyzw"/>, <paramref name="xyzw"/>, <paramref name="xyzw"/>)</c>.
	/// </summary>
	/// <param name="xyzw">X, Y, Z and W value of vector.</param>
	public Vector4d(T xyzw)
		=> (X, Y, Z, W) = (xyzw, xyzw, xyzw, xyzw);

	/// <summary>
	/// Expands two-dimension vector to fourth-dimension.
	/// </summary>
	/// <param name="base2">Base vector.</param>
	/// <param name="z">Z value.</param>
	/// <param name="w">W value.</param>
	public Vector4d(Vector2d<T> base2, T z, T w)
		=> (X, Y, Z, W) = (base2.X, base2.Y, z, w);

	/// <summary>
	/// Expands third-dimension vector to fourth-dimension.
	/// </summary>
	/// <param name="base3">Base vector.</param>
	/// <param name="w">W value.</param>
	public Vector4d(Vector3d<T> base3, T w)
		=> (X, Y, Z, W) = (base3.X, base3.Y, base3.Z, w);

	/// <summary>
	/// Creates vector by buffer.
	/// </summary>
	/// <param name="data">Buffer with vector data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>

	public Vector4d(ReadOnlySpan<T> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector4d<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates vector by buffer.
	/// </summary>
	/// <param name="data">Buffer with vector data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Vector4d(ReadOnlySpan<byte> data)
	{
		if (data.Length < VectorSize)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector4d<T>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Vector with all ones, <c>(1, 1, 1, 1)</c>.
	/// </summary>
	public static Vector4d<T> One => new(T.One, T.One, T.One, T.One);

	/// <summary>
	/// Vector with all zeros, <c>(0, 0, 0, 0)</c>.
	/// </summary>
	public static Vector4d<T> Zero => new(T.Zero, T.Zero, T.Zero, T.Zero);

	/// <summary>
	/// Vector which X is one, <c>(1, 0, 0, 0)</c>.
	/// </summary>
	public static Vector4d<T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero);

	/// <summary>
	/// Vector which Y is one, <c>(0, 1, 0, 0)</c>.
	/// </summary>
	public static Vector4d<T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero);

	/// <summary>
	/// Vector which Z is one, <c>(0, 0, 1, 0)</c>.
	/// </summary>
	public static Vector4d<T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero);

	/// <summary>
	/// Vector which W is one, <c>(0, 0, 0, 1)</c>.
	/// </summary>
	public static Vector4d<T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator +(Vector4d<T> left, Vector4d<T> right)
		=> new(
			left.X + right.X,
			left.Y + right.Y,
			left.Z + right.Z,
			left.W + right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator -(Vector4d<T> left, Vector4d<T> right)
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z,
			left.W - right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator /(Vector4d<T> left, Vector4d<T> right)
		=> new(
			left.X / right.X,
			left.Y / right.Y,
			left.Z / right.Z,
			left.W / right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator *(Vector4d<T> left, Vector4d<T> right)
		=> new(
			left.X * right.X,
			left.Y * right.Y,
			left.Z * right.Z,
			left.W * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator ==(Vector4d<T> left, Vector4d<T> right)
		=> left.X == right.X
		&& left.Y == right.Y
		&& left.Z == right.Z
		&& left.W == right.W;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator !=(Vector4d<T> left, Vector4d<T> right)
		=> left.X != right.X
		|| left.Y != right.Y
		|| left.Z != right.Z
		|| left.W != right.W;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator +(Vector4d<T> left, T right)
		=> new(
			left.X + right,
			left.Y + right,
			left.Z + right,
			left.W + right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator -(Vector4d<T> left, T right)
		=> new(
			left.X - right,
			left.Y - right,
			left.Z - right,
			left.W - right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator /(Vector4d<T> left, T right)
		=> new(
			left.X / right,
			left.Y / right,
			left.Z / right,
			left.W / right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator /(T left, Vector4d<T> right)
		=> new(
			left / right.X,
			left / right.Y,
			left / right.Z,
			left / right.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator *(Vector4d<T> left, T right)
		=> new(
			left.X * right,
			left.Y * right,
			left.Z * right,
			left.W * right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator *(T left, Vector4d<T> right)
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z,
			left * right.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator -(Vector4d<T> operand)
		=> new(
			-operand.X,
			-operand.Y,
			-operand.Z,
			-operand.W
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector4d<T> operator +(Vector4d<T> operand)
		=> new(
			+operand.X,
			+operand.Y,
			+operand.Z,
			+operand.W
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static implicit operator Vector2d<T>(Vector4d<T> operand)
		=> new(
			operand.X,
			operand.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static implicit operator Vector3d<T>(Vector4d<T> operand)
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

	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y, Z, W);

	/// <inheritdoc/>
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Vector4d<T> vec
		? vec == this : false;
	public readonly bool Equals(Vector4d<T> other)
		=> this == other;

	/// <inheritdoc/>
	public override readonly string ToString() => ToString("G", CultureInfo.CurrentCulture);
	public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
	{
		StringBuilder sb = new();
		string separator = ",";
		sb.Append('<');

		sb.Append(X.ToString(format, formatProvider));
		sb.Append(separator);
		if (!String.IsNullOrWhiteSpace(separator))
			sb.Append(' ');

		sb.Append(Y.ToString(format, formatProvider));
		sb.Append(separator);
		if (!String.IsNullOrWhiteSpace(separator))
			sb.Append(' ');

		sb.Append(Z.ToString(format, formatProvider));
		sb.Append(separator);
		if (!String.IsNullOrWhiteSpace(separator))
			sb.Append(' ');

		sb.Append(W.ToString(format, formatProvider));

		sb.Append('>');
		return sb.ToString();
	}
}