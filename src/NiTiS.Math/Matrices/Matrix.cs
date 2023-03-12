using NiTiS.Core.Annotations;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NiTiS.Math.Matrices;

/// <summary>Structure representing a dynamic size matrix.</summary>
/// <typeparam name="N">Matrix data type.</typeparam>
[NotImplementYet]
[Obsolete(nameof(NotImplementYetAttribute))]
public sealed class Matrix<N> :
	IMatrix<Matrix<N>, N>,

	IAdditionOperators<Matrix<N>, Matrix<N>, Matrix<N>>,
	ISubtractionOperators<Matrix<N>, Matrix<N>, Matrix<N>>,
	IMultiplyOperators<Matrix<N>, Matrix<N>, Matrix<N>>,
	IEqualityOperators<Matrix<N>, Matrix<N>, bool>,

	IAdditionOperators<Matrix<N>, N, Matrix<N>>,
	ISubtractionOperators<Matrix<N>, N, Matrix<N>>,
	IMultiplyOperators<Matrix<N>, N, Matrix<N>>,

	IEquatable<Matrix<N>>
	where N : unmanaged, INumberBase<N>
{
    private N[] items;

    private int rows, columns;

	public Matrix(int rows, int columns)
	{
		this.rows = rows;
		this.columns = columns;

		items = new N[rows * columns];
	}

	public Matrix(N[] buffer, int rows, int columns)
	{
		if (buffer.Length < rows * columns)
			throw new ArgumentException("Buffer is less that matrix size");

		this.items = buffer;
		this.rows = rows;
		this.columns = columns;
	}

	public unsafe Matrix(N* src, int rows, int columns)
	{
		items = new N[rows * columns];
		this.columns = columns;
		this.rows = rows;

		fixed (void* dst = &items[0])
		{
			Buffer.MemoryCopy(src, dst, rows * columns, rows * columns);
		}
	}

	public int ColumnsCount
	=> columns;

	public int RowsCount
		=> rows;

	public N this[int row, int column] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

	public static Matrix<N> operator +(Matrix<N> left, Matrix<N> right)
	{
		if (left.columns != right.columns || left.rows != right.rows)
			throw new InvalidOperationException();

		return null;
	}

	public static Matrix<N> operator -(Matrix<N> left, Matrix<N> right)
	{
		if (left.columns != right.columns || left.rows != right.rows)
			throw new InvalidOperationException();
		
		return null;
	}

	public static Matrix<N> operator *(Matrix<N> left, Matrix<N> right)
	{
		throw new NotImplementedException();
	}

	public static Matrix<N> operator +(Matrix<N> left, N right)
	{
		throw new NotImplementedException();
	}

	public static Matrix<N> operator -(Matrix<N> left, N right)
	{
		throw new NotImplementedException();
	}

	public static Matrix<N> operator *(Matrix<N> left, N right)
	{
		throw new NotImplementedException();
	}

	public static bool operator ==(Matrix<N>? left, Matrix<N>? right)
	{
		throw new NotImplementedException();
	}

	public static bool operator !=(Matrix<N>? left, Matrix<N>? right)
	{
		throw new NotImplementedException();
	}

	public bool Equals(Matrix<N>? other)
	{
		if (other is null)
			return false;

		int cc = other.RowsCount;
		int rc = other.RowsCount;

		if (rc != columns || cc != columns)
			return false;

		for (int r = 0; r < rc; r++)
		{
			for (int c = 0; c < cc; c++)
			{
				int index = c + (r * columns);
				if (items[index] != other.items[index])
					return false;
            }
		}

		return true;
	}

	/// <summary>
	/// Creates identity matrix with <paramref name="size"/>
	/// </summary>
	/// <param name="size">Count of columns and rows.</param>
	/// <returns>X by X sized matrix.</returns>
	public static Matrix<N> CreateIdentity(int size)
	{
		int square = size * size;
		N[] buffer = new N[square];
		for (int r = 0; r < size; r++)
		{
			for (int c = 0; c < size; c++)
			{
				buffer[c + (r * size)] = c == r ? N.One : N.Zero;
			}
        }

		return new(buffer, size, size);
	}

	/// <inheritdoc/>
	public override bool Equals(object? obj)
		=> obj is Matrix<N> mat && Equals(mat);

	/// <inheritdoc/>
	public override int GetHashCode()
		=> HashCode.Combine(items);
}