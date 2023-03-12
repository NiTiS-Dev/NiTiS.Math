using System;

namespace NiTiS.Math.Matrices;

/// <summary>
/// Defines a number type.
/// </summary>
/// <typeparam name="Self">The type that implements the interface.</typeparam>
public interface IMatrix<Self, Element>
	where Self : IMatrix<Self, Element>
{
	/// <summary>
	/// Number of rows of this matrix.
	/// </summary>
	int ColumnsCount { get; }

	/// <summary>
	/// Number of columns of this matrix.
	/// </summary>
	int RowsCount { get; }

	/// <summary>
	/// The element at [<paramref name="row"/>][<paramref name="column"/>].
	/// </summary>
	/// <param name="row">Zero-based row index.</param>
	/// <param name="column">Zero-based column index.</param>
	/// <returns>The matrix element.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	///	<paramref name="row"/> was less than zero or greater than the number of rows.
	///	-or-
	///	<paramref name="column"/> was less than zero or greater than the number of columns.
	/// </exception>
	Element this[int row, int column] { get; set; }
}