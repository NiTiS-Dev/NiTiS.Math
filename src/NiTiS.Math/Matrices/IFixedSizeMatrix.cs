namespace NiTiS.Math.Matrices;

public interface IFixedSizeMatrix<Self, Element> : IMatrix<Self, Element>
	where Self : IFixedSizeMatrix<Self, Element>
{
	/// <summary>
	/// Number of rows of this matrix.
	/// </summary>
	static abstract new int ColumnsCount { get; }

	/// <summary>
	/// Number of columns of this matrix.
	/// </summary>
	static abstract new int RowsCount { get; }
}