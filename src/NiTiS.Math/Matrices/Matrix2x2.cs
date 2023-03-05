using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Matrices;

/// <summary>Structure representing a matrix2x2.</summary>
/// <typeparam name="N">Matrix data type.</typeparam>
public struct Matrix2x2<N> :
	IMatrix<Matrix2x2<N>, N>
	where N : unmanaged, INumberBase<N>
{
	#region Matrix

	private const int
		rowsCount = 2,
		columnsCount = 2,
		elementCount = columnsCount * rowsCount;

	/// <inheritdoc/>
	public static int ColumnsCount => columnsCount;

	/// <inheritdoc/>
	public static int RowsCount => rowsCount;

	public N M11;
	public N M12;

	public N M21;
	public N M22;

	#endregion

	public Vector2d<N> Row1
		=> Unsafe.As<N, Vector2d<N>>(ref M11);
	public Vector2d<N> Row2
		=> Unsafe.As<N, Vector2d<N>>(ref M21);

	public Vector2d<N> Column1
		=> new(M11, M21);
	public Vector2d<N> Column2
		=> new(M12, M22);

	public N this[int index]
	{
		get
		{
			if (index >= elementCount)
				throw new ArgumentOutOfRangeException(nameof(index));

			return Unsafe.Add(ref M11, index);
		}
	}

	/// <inheritdoc/>
	public N this[int row, int column]
	{
		set
		{
			if (column >= rowsCount)
				throw new ArgumentOutOfRangeException(nameof(column));

			if (row >= columnsCount)
				throw new ArgumentOutOfRangeException(nameof(row));

			Unsafe.Add(ref M11, row * columnsCount + column) = value;
		}
		get
		{
			if (column >= rowsCount)
				throw new ArgumentOutOfRangeException(nameof(column));

			if (row >= columnsCount)
				throw new ArgumentOutOfRangeException(nameof(row));

			return Unsafe.Add(ref M11, row * columnsCount + column);
		}
	}

	public static Matrix2x2<N> Identity
		=> new(
		N.One, N.Zero,
		N.Zero, N.One
		);

	[MethodImpl(AggressiveOptimization)]
	public Matrix2x2(
		N m11, N m12,
		N m21, N m22
		)
	{
		M11 = m11;
		M12 = m12;
		M21 = m21;
		M22 = m22;
	}

	[MethodImpl(AggressiveOptimization)]
	public static bool operator ==(Matrix2x2<N> left, Matrix2x2<N> right)
		=> left.M11 == right.M11 && left.M22 == right.M22 // Check diagonal element first for early out.
		&& left.M12 == right.M12 && left.M21 == right.M21
		;

	[MethodImpl(AggressiveOptimization)]
	public static bool operator !=(Matrix2x2<N> left, Matrix2x2<N> right)
		=> left.M11 != right.M11 || left.M22 != right.M22 // Check diagonal element first for early out.
		|| left.M12 != right.M12 || left.M21 != right.M21
		;

	[MethodImpl(AggressiveOptimization)]
	public static bool operator ==(Matrix2x2<N> left, N right)
		=> left.M11 == right && left.M22 == right // Check diagonal element first for early out.
		&& left.M12 == right && left.M21 == right
		;

	[MethodImpl(AggressiveOptimization)]
	public static bool operator !=(Matrix2x2<N> left, N right)
		=> left.M11 != right || left.M22 != right // Check diagonal element first for early out.
		|| left.M12 != right || left.M21 != right
		;

	/// <summary>
	/// Add one matrix to another one.
	/// </summary>
	/// <param name="left">The left argument.</param>
	/// <param name="right">The right argument.</param>
	/// <returns>Resulting matrix - result of addition <paramref name="right"/> to <paramref name="left"/> matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix2x2<N> operator +(Matrix2x2<N> left, Matrix2x2<N> right)
	{
		left.M11 += right.M11;
		left.M12 += right.M12;
		left.M21 += right.M21;
		left.M22 += right.M22;
		return left;
	}

	/// <summary>
	/// Add scalar to matrix.
	/// </summary>
	/// <param name="left">The left argument.</param>
	/// <param name="right">The right argument.</param>
	/// <returns>Resulting matrix - result of addition <paramref name="right"/> scalar to <paramref name="left"/> matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix2x2<N> operator +(Matrix2x2<N> left, N right)
	{
		left.M11 += right;
		left.M12 += right;
		left.M21 += right;
		left.M22 += right;
		return left;
	}

	/// <summary>
	/// Subtract one matrix from another one.
	/// </summary>
	/// <param name="left">The left argument.</param>
	/// <param name="right">The right argument.</param>
	/// <returns>Resulting matrix - result of subtraction <paramref name="right"/> from <paramref name="left"/> matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix2x2<N> operator -(Matrix2x2<N> left, Matrix2x2<N> right)
	{
		left.M11 -= right.M11;
		left.M12 -= right.M12;
		left.M21 -= right.M21;
		left.M22 -= right.M22;
		return left;
	}

	/// <summary>
	/// Subtract scalar from matrix.
	/// </summary>
	/// <param name="left">The left argument.</param>
	/// <param name="right">The right argument.</param>
	/// <returns>Resulting matrix - result of subtraction <paramref name="right"/> scalar from <paramref name="left"/> matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix2x2<N> operator -(Matrix2x2<N> left, N right)
	{
		left.M11 -= right;
		left.M12 -= right;
		left.M21 -= right;
		left.M22 -= right;
		return left;
	}

	/// <summary>
	/// Multiply two matrices.
	/// </summary>
	/// <param name="left">The left argument.</param>
	/// <param name="right">The right argument.</param>
	/// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> and <paramref name="right"/> matrices.</returns>

	[MethodImpl(AggressiveOptimization)]
	public static Matrix2x2<N> operator *(Matrix2x2<N> left, Matrix2x2<N> right)
	{
		Unsafe.SkipInit(out Matrix2x2<N> m);

		// First row
		m.M11 = Vector2d.Dot(left.Row1, right.Column1);
		m.M12 = Vector2d.Dot(left.Row1, right.Column2);

		// Second row
		
		m.M21 = Vector2d.Dot(left.Row2, right.Column1);
		m.M22 = Vector2d.Dot(left.Row2, right.Column2);

		return m;
	}

	/// <summary>
	/// Multiply matrix by scalar.
	/// </summary>
	/// <param name="left">The left argument.</param>
	/// <param name="right">The right argument.</param>
	/// <returns>Resulting matrix - result of multiplication of <paramref name="left"/> matrix and <paramref name="right"/> scalar.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix2x2<N> operator *(Matrix2x2<N> left, N right)
	{
		Unsafe.SkipInit(out Matrix2x2<N> m);

		// First row
		m.M11 = Vector2d.Dot(left.Row1, right);
		m.M12 = Vector2d.Dot(left.Row1, right);

		// Second row		
		m.M21 = Vector2d.Dot(left.Row2, right);
		m.M22 = Vector2d.Dot(left.Row2, right);

		return m;
	}


	/// <summary>Indicates whether the current matrix is the identity matrix.</summary>
	/// <value><see langword="true" /> if the current matrix is the identity matrix; otherwise: <see langword="false" />.</value>
	public readonly bool IsIdentity
			=> M11 == N.One && M22 == N.One
			&& M12 == N.Zero && M21 == N.Zero;

	/// <summary>
	/// Check equality to another matrix.
	/// </summary>
	/// <param name="obj">Object for equality.</param>
	/// <returns><see langword="true"/> when object is matrix that equal to this one, otherwise <see langword="false"/>.</returns>
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Matrix2x2<N> mat && mat == this;

	/// <summary>
	/// Check equality to another matrix.
	/// </summary>
	/// <param name="mat">The second matrix.</param>
	/// <returns><see langword="true"/> when other matrix is equal to this one, otherwise <see langword="false"/>.</returns>
	public readonly bool Equals(Matrix2x2<N> mat)
		=> mat == this;
	public override readonly int GetHashCode()
	{
		HashCode hash = default;

		hash.Add(M11);
		hash.Add(M12);

		hash.Add(M21);
		hash.Add(M22);

		return hash.ToHashCode();
	}
	public override readonly string ToString()
		=> string.Format(CultureInfo.CurrentCulture, "{{ {{M11:{0} M12:{1}}} {{M21:{2} M22:{3}}} }}",
							 M11, M12,
							 M21, M22
							 );
}
