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

/// <summary>
/// Two-dimension vector with direction and magnitude.
/// </summary>
/// <typeparam name="N">Vector type.</typeparam>
[DebuggerDisplay($@"{{{nameof(ToString)}(""G""),nq}}")]
public unsafe struct Vector2d<N> :
	// Vector op Vector
	IAdditionOperators<Vector2d<N>, Vector2d<N>, Vector2d<N>>,
	ISubtractionOperators<Vector2d<N>, Vector2d<N>, Vector2d<N>>,
	IDivisionOperators<Vector2d<N>, Vector2d<N>, Vector2d<N>>,
	IMultiplyOperators<Vector2d<N>, Vector2d<N>, Vector2d<N>>,
	IEqualityOperators<Vector2d<N>, Vector2d<N>, bool>,
	// Vector op T
	IAdditionOperators<Vector2d<N>, N, Vector2d<N>>,
	ISubtractionOperators<Vector2d<N>, N, Vector2d<N>>,
	IDivisionOperators<Vector2d<N>, N, Vector2d<N>>,
	IMultiplyOperators<Vector2d<N>, N, Vector2d<N>>,
	// Unary op
	IUnaryNegationOperators<Vector2d<N>, Vector2d<N>>,
	IUnaryPlusOperators<Vector2d<N>, Vector2d<N>>,
	IFormattable,
	IEquatable<Vector2d<N>>
	where N :
		unmanaged,
		INumberBase<N>
{
	/// <summary>
	/// X value of vector, the first dimension.
	/// </summary>
	public N X;
	/// <summary>
	/// Y value of vector, the second dimension.
	/// </summary>
	public N Y;

	/// <summary>
	/// Squared magnitude of vector.
	/// </summary>
	public readonly N LengthSquared
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => Vector2d.Dot(this, this);
	}

	public const int ElementCount = 2;
	private static readonly int VectorSize = sizeof(N) * ElementCount;

	/// <summary>
	/// Creates new two-dimensional vector with values <c>(<paramref name="x"/>, <paramref name="y"/>)</c>.
	/// </summary>
	/// <param name="x">X value of vector.</param>
	/// <param name="y">Y value of vector.</param>
	public Vector2d(N x, N y)
	=> (X, Y) = (x, y);

	/// <summary>
	/// Creates new two-dimensional vector with values <c>(<paramref name="xy"/>, <paramref name="xy"/>)</c>.
	/// </summary>
	/// <param name="xy">X and Y value of vector.</param>
	public Vector2d(N xy)
		=> (X, Y) = (xy, xy);

	/// <summary>
	/// Creates vector by buffer.
	/// </summary>
	/// <param name="data">Buffer with vector data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Vector2d(ReadOnlySpan<N> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector2d<N>>(ref Unsafe.As<N, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates vector by buffer.
	/// </summary>
	/// <param name="data">Buffer with vector data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Vector2d(ReadOnlySpan<byte> data)
	{
		if (data.Length < VectorSize)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector2d<N>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Vector with all ones, <c>(1, 1)</c>.
	/// </summary>
	public static Vector2d<N> One => new(N.One, N.One);

	/// <summary>
	/// Vector with all zeros, <c>(0, 0)</c>.
	/// </summary>
	public static Vector2d<N> Zero => new(N.Zero, N.Zero);

	/// <summary>
	/// Vector which X is one, <c>(1, 0)</c>.
	/// </summary>
	public static Vector2d<N> UnitX => new(N.One, N.Zero);

	/// <summary>
	/// Vector which Y is one, <c>(0, 1)</c>.
	/// </summary>
	public static Vector2d<N> UnitY => new(N.Zero, N.One);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator +(Vector2d<N> left, Vector2d<N> right)
		=> new(
			left.X + right.X,
			left.Y + right.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator -(Vector2d<N> left, Vector2d<N> right)
		=> new(
			left.X - right.X,
			left.Y - right.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator /(Vector2d<N> left, Vector2d<N> right)
		=> new(
			left.X / right.X,
			left.Y / right.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator *(Vector2d<N> left, Vector2d<N> right)
		=> new(
			left.X * right.X,
			left.Y * right.Y
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator ==(Vector2d<N> left, Vector2d<N> right)
		=> left.X == right.X
		&& left.Y == right.Y;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator !=(Vector2d<N> left, Vector2d<N> right)
		=> left.X != right.X
		|| left.Y != right.Y;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator +(Vector2d<N> left, N right)
		=> new(
			left.X + right,
			left.Y + right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator -(Vector2d<N> left, N right)
		=> new(
			left.X - right,
			left.Y - right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator /(Vector2d<N> left, N right)
		=> new(
			left.X / right,
			left.Y / right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator /(N left, Vector2d<N> right)
		=> new(
			left / right.X,
			left / right.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator *(Vector2d<N> left, N right)
		=> new(
			left.X * right,
			left.Y * right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator *(N left, Vector2d<N> right)
		=> new(
			left * right.X,
			left * right.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator -(Vector2d<N> operand)
		=> new(
			-operand.X,
			-operand.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector2d<N> operator +(Vector2d<N> operand)
		=> new(
			+operand.X,
			+operand.Y
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static explicit operator Vector3d<N>(Vector2d<N> operand)
		=> new(
			operand.X,
			operand.Y,
			N.Zero
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static explicit operator Vector4d<N>(Vector2d<N> operand)
		=> new(
			operand.X,
			operand.Y,
			N.Zero,
			N.Zero
			);

	public readonly void CopyTo(N[] array)
		=> CopyTo(array, 0);
	public readonly void CopyTo(N[] array, uint offset)
	{
		if (array.LongLength < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(array));

		array[0 + offset] = X;
		array[1 + offset] = Y;
	}
	public readonly void CopyTo(Span<N> array, uint offset)
	{
		if (array.Length < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(array));

		array[0 + (int)offset] = X;
		array[1 + (int)offset] = Y;
	}

	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y);

	/// <inheritdoc/>
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Vector2d<N> vec
		? vec == this : false;
	public readonly bool Equals(Vector2d<N> other)
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
		if (!string.IsNullOrWhiteSpace(separator))
			sb.Append(' ');
		sb.Append(Y.ToString(format, formatProvider));
		sb.Append('>');
		return sb.ToString();
	}
}
