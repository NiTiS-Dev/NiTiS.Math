import sys
import os
from io import StringIO

class StringBuilder:
	string = None

	def __init__(self):
		self.string = StringIO()

	def Add(self, str):
		self.string.write(str)

	def __str__(self):
		return self.string.getvalue()

if __name__ == '__main__':
	count = len(sys.argv) - 1 # Ignore filename

	if (count < 2):
		raise RuntimeError

	sizeX = sys.argv[1]
	sizeY = sys.argv[2]

	print(f"Generate Matirx{sizeX}x{sizeY}")
	currentDir = os.getcwd()

	while True:
		listOfFile = os.listdir(currentDir)

		if ".git" in listOfFile:
			print(f"Repository directory: {currentDir}")
			break
		else:
			currentDir = os.path.dirname(currentDir)

	structName = f"Matrix{sizeX}x{sizeY}"
	elements = []

	print (f"Columns {sizeY}")
	print (f"Rows {sizeX}")
	for x in range(int(sizeX)):
		for y in range(int(sizeY)):
			elements.append(f"M{x + 1}{y + 1}")

	currentDir = os.path.join(currentDir, "src", "NiTiS.Math", "Matrices")

	content = StringBuilder()

	content.Add(f"""/// ====================
/// Generated matrix_generate.py
/// ====================
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
namespace NiTiS.Math.Matrices;
/// <summary>Structure representing a {structName.lower()}.</summary>
/// <typeparam name="N">Matrix data type.</typeparam>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct {structName}<N> :
	IFixedSizeMatrix<{structName}<N>, N>,
	IAdditionOperators<{structName}<N>, {structName}<N>, {structName}<N>>,
	ISubtractionOperators<{structName}<N>, {structName}<N>, {structName}<N>>,
	IMultiplyOperators<{structName}<N>, {structName}<N>, {structName}<N>>,
	IEqualityOperators<{structName}<N>, {structName}<N>, bool>,
	IAdditionOperators<{structName}<N>, N, {structName}<N>>,
	ISubtractionOperators<{structName}<N>, N, {structName}<N>>,
	IMultiplyOperators<{structName}<N>, N, {structName}<N>>,
	IEquatable<{structName}<N>>
	where N : unmanaged, INumberBase<N>
""")
	content.Add("{")

	content.Add(f"""
#region Matrix
	private const int
		rowsCount = {sizeY},
		columnsCount = {sizeX},
		elementCount = columnsCount * rowsCount;
	/// <inheritdoc/>
	public static int ColumnsCount => columnsCount;
	/// <inheritdoc/>
	public static int RowsCount => rowsCount;
	/// <inheritdoc/>
	readonly int IMatrix<{structName}<N>, N>.ColumnsCount => columnsCount;
	/// <inheritdoc/>
	readonly int IMatrix<{structName}<N>, N>.RowsCount => rowsCount;
""")
	for element in elements:
		content.Add(f"	public N {element};\n")

	content.Add("	#endregion\n")
	for row in range(int(sizeX)):
		content.Add(f"""
	public Vector{sizeY}d<N> Row{row + 1}
		=> Unsafe.As<N, Vector{sizeY}d<N>>(ref M{row + 1}1);
""")
	for column in range(int(sizeY)):
		content.Add(f"""
	public readonly Vector{sizeX}d<N> Column{column + 1}
		=> new(""")
		for row in range(int(sizeX)):
			content.Add(f"M{row + 1}{column + 1}")
			if 1 + row != int(sizeX):
				content.Add(", ")
		content.Add(");\n")

	content.Add("""
	public N this[int index]
	{
		get
		{
			if (index >= elementCount)
				throw new ArgumentOutOfRangeException(nameof(index));
			return Unsafe.Add(ref M11, index);
		}
		set
		{
			if (index >= elementCount)
				throw new ArgumentOutOfRangeException(nameof(index));
			Unsafe.Add(ref M11, index) = value;
		}
	}
""")

	with open(os.path.join(currentDir, f"{structName}.cs"), 'w') as f:
		f.write(content.__str__())