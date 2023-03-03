using NiTiS.Math.Matrices;
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
/// Third-dimension vector with direction and magnitude.
/// </summary>
[DebuggerDisplay($@"{{{nameof(ToString)}(""G""),nq}}")]
public unsafe struct Vector3d<N> :
	// Vector op Vector
	IAdditionOperators<Vector3d<N>, Vector3d<N>, Vector3d<N>>,
	ISubtractionOperators<Vector3d<N>, Vector3d<N>, Vector3d<N>>,
	IDivisionOperators<Vector3d<N>, Vector3d<N>, Vector3d<N>>,
	IMultiplyOperators<Vector3d<N>, Vector3d<N>, Vector3d<N>>,
	IEqualityOperators<Vector3d<N>, Vector3d<N>, bool>,
	// Vector op T
	IAdditionOperators<Vector3d<N>, N, Vector3d<N>>,
	ISubtractionOperators<Vector3d<N>, N, Vector3d<N>>,
	IDivisionOperators<Vector3d<N>, N, Vector3d<N>>,
	IMultiplyOperators<Vector3d<N>, N, Vector3d<N>>,
	// Unary op
	IUnaryNegationOperators<Vector3d<N>, Vector3d<N>>,
	IUnaryPlusOperators<Vector3d<N>, Vector3d<N>>,
	IFormattable,
	IEquatable<Vector3d<N>>
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
	/// Z value of vector, the third dimension.
	/// </summary>
	public N Z;

	/// <summary>
	/// Squared magnitude of vector.
	/// </summary>
	public readonly N LengthSquared
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => Vector3d.Dot(this, this);
	}

	public const int ElementCount = 3;
	private static readonly int VectorSize = sizeof(N) * ElementCount;

	/// <summary>
	/// Creates new third-dimensional vector with values <c>(<paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>)</c>.
	/// </summary>
	/// <param name="x">X value of vector.</param>
	/// <param name="y">Y value of vector.</param>
	/// <param name="z">Z value of vector.</param>
	public Vector3d(N x, N y, N z)
		=> (X, Y, Z) = (x, y, z);

	/// <summary>
	/// Creates new third-dimensional vector with values <c>(<paramref name="xyz"/>, <paramref name="xyz"/>, <paramref name="xyz"/>)</c>.
	/// </summary>
	/// <param name="xyz">X, Y and Z value of vector.</param>
	public Vector3d(N xyz)
		=> (X, Y, Z) = (xyz, xyz, xyz);

	/// <summary>
	/// Expands two-dimension vector to third-dimension.
	/// </summary>
	/// <param name="base2">Base vector.</param>
	/// <param name="z">Z value.</param>
	public Vector3d(Vector2d<N> base2, N z)
		=> (X, Y, Z) = (base2.X, base2.Y, z);


	/// <summary>
	/// Creates vector by buffer.
	/// </summary>
	/// <param name="data">Buffer with vector data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Vector3d(ReadOnlySpan<N> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector3d<N>>(ref Unsafe.As<N, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates vector by buffer.
	/// </summary>
	/// <param name="data">Buffer with vector data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Vector3d(ReadOnlySpan<byte> data)
	{
		if (data.Length < VectorSize)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vector3d<N>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Vector with all ones, <c>(1, 1, 1)</c>.
	/// </summary>
	public static Vector3d<N> One => new(N.One, N.One, N.One);

	/// <summary>
	/// Vector with all zeros, <c>(0, 0, 0)</c>.
	/// </summary>
	public static Vector3d<N> Zero => new(N.Zero, N.Zero, N.Zero);

	/// <summary>
	/// Vector which X is one, <c>(1, 0, 0)</c>.
	/// </summary>
	public static Vector3d<N> UnitX => new(N.One, N.Zero, N.Zero);

	/// <summary>
	/// Vector which Y is one, <c>(0, 1, 0)</c>.
	/// </summary>
	public static Vector3d<N> UnitY => new(N.Zero, N.One, N.Zero);

	/// <summary>
	/// Vector which Z is one, <c>(0, 0, 1)</c>.
	/// </summary>
	public static Vector3d<N> UnitZ => new(N.Zero, N.Zero, N.One);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator +(Vector3d<N> left, Vector3d<N> right)
		=> new(
			left.X + right.X,
			left.Y + right.Y,
			left.Z + right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator -(Vector3d<N> left, Vector3d<N> right)
		=> new(
			left.X - right.X,
			left.Y - right.Y,
			left.Z - right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator /(Vector3d<N> left, Vector3d<N> right)
		=> new(
			left.X / right.X,
			left.Y / right.Y,
			left.Z / right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator *(Vector3d<N> left, Vector3d<N> right)
		=> new(
			left.X * right.X,
			left.Y * right.Y,
			left.Z * right.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator ==(Vector3d<N> left, Vector3d<N> right)
		=> left.X == right.X
		&& left.Y == right.Y
		&& left.Z == right.Z;
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static bool operator !=(Vector3d<N> left, Vector3d<N> right)
		=> left.X != right.X
		|| left.Y != right.Y
		|| left.Z != right.Z;

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator +(Vector3d<N> left, N right)
		=> new(
			left.X + right,
			left.Y + right,
			left.Z + right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator -(Vector3d<N> left, N right)
		=> new(
			left.X - right,
			left.Y - right,
			left.Z - right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator /(Vector3d<N> left, N right)
		=> new(
			left.X / right,
			left.Y / right,
			left.Z / right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator /(N left, Vector3d<N> right)
		=> new(
			left / right.X,
			left / right.Y,
			left / right.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator *(Vector3d<N> left, N right)
		=> new(
			left.X * right,
			left.Y * right,
			left.Z * right
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator *(N left, Vector3d<N> right)
		=> new(
			left * right.X,
			left * right.Y,
			left * right.Z
			);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator -(Vector3d<N> operand)
		=> new(
			-operand.X,
			-operand.Y,
			-operand.Z
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Vector3d<N> operator +(Vector3d<N> operand)
		=> new(
			+operand.X,
			+operand.Y,
			+operand.Z
			);

	#region Multiplication with Matrices

	//TODO: Matrix4x2,3 not implement yet

	///// <summary>
	///// Multiply two matrices.
	///// </summary>
	///// <param name="left">Vector, first operand that use like 1x4 matrix.</param>
	///// <param name="right">Matrix, second operand.</param>
	///// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> and <paramref name="right"/> matrices.</returns>
	//[MethodImpl(AggressiveOptimization)]
	//public static Vector3d<T> operator *(Vector4d<T> left, Matrix4x2<T> right)
	//{
	//	Unsafe.SkipInit(out Vector4d<T> result);

	//	result.X = Vector3d.Dot(left, right.Column1);
	//	result.Y = Vector3d.Dot(left, right.Column2);
	//	result.Z = Vector3d.Dot(left, right.Column3);

	//	return result;
	//}

	///// <summary>
	///// Multiply two matrices.
	///// </summary>
	///// <param name="left">Vector, first operand that use like 1x4 matrix.</param>
	///// <param name="right">Matrix, second operand.</param>
	///// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> and <paramref name="right"/> matrices.</returns>
	//[MethodImpl(AggressiveOptimization)]
	//public static Vector3d<T> operator *(Vector4d<T> left, Matrix4x3<T> right)
	//{
	//	Unsafe.SkipInit(out Vector4d<T> result);

	//	result.X = Vector3d.Dot(left, right.Column1);
	//	result.Y = Vector3d.Dot(left, right.Column2);
	//	result.Z = Vector3d.Dot(left, right.Column3);

	//	return result;
	//}

	/// <summary>
	/// Multiply two matrices.
	/// </summary>
	/// <param name="left">Vector, first operand that use like 1x4 matrix.</param>
	/// <param name="right">Matrix, second operand.</param>
	/// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> and <paramref name="right"/> matrices.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Vector4d<T> operator *(Vector4d<T> left, Matrix4x4<T> right)
	{
		Unsafe.SkipInit(out Vector4d<T> result);

		result.X = Vector4d.Dot(left, right.Column1);
		result.Y = Vector4d.Dot(left, right.Column2);
		result.Z = Vector4d.Dot(left, right.Column3);
		result.W = Vector4d.Dot(left, right.Column4);

		return result;
	}

	#endregion

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
<<<<<<<< HEAD:NiTiS.Math/Vector4d.cs
	public static implicit operator Vector2d<T>(Vector4d<T> operand)
		=> new(operand.X, operand.Y);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static implicit operator Vector3d<T>(Vector4d<T> operand)
		=> new(operand.X, operand.Y, operand.Z);
========
	public static implicit operator Vector2d<N>(Vector3d<N> operand)
		=> new(
			operand.X,
			operand.Y
			);
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static explicit operator Vector4d<N>(Vector3d<N> operand)
		=> new(
			operand.X,
			operand.Y,
			operand.Z,
			N.One
			);
>>>>>>>> dev:NiTiS.Math/Vector3d.cs

	public readonly void CopyTo(N[] array)
		=> CopyTo(array, 0);
	public readonly void CopyTo(N[] array, uint offset)
	{
		if (array.LongLength < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(array));

		array[0 + offset] = X;
		array[1 + offset] = Y;
		array[2 + offset] = Z;
	}
	public readonly void CopyTo(Span<N> array, uint offset)
	{
		if (array.Length < ElementCount + offset)
			throw new ArgumentOutOfRangeException(nameof(array));

		array[0 + (int)offset] = X;
		array[1 + (int)offset] = Y;
		array[2 + (int)offset] = Z;
	}
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y, Z);

	/// <inheritdoc/>
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
<<<<<<<< HEAD:NiTiS.Math/Vector4d.cs
		=> obj is Vector4d<T> vec
		&& this == vec;
	public readonly bool Equals(Vector4d<T> other)
========
		=> obj is Vector3d<N> vec
		? vec == this : false;

	public readonly bool Equals(Vector3d<N> other)
>>>>>>>> dev:NiTiS.Math/Vector3d.cs
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
		sb.Append(separator);
		if (!string.IsNullOrWhiteSpace(separator))
			sb.Append(' ');

		sb.Append(Z.ToString(format, formatProvider));

		sb.Append('>');
		return sb.ToString();
	}
}
