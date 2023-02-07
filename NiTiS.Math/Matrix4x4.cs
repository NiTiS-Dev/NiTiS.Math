using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Matrix4x4<T>
	where T : unmanaged, INumberBase<T>
{
	#region Matrix
	public const int
		RowsCount = 4,
		ColumnsCount = 4,
		ElementCount = ColumnsCount * RowsCount;

	public T M11;
	public T M12;
	public T M13;
	public T M14;

	public T M21;
	public T M22;
	public T M23;
	public T M24;

	public T M31;
	public T M32;
	public T M33;
	public T M34;

	public T M41;
	public T M42;
	public T M43;
	public T M44;
	#endregion

	public Vector4D<T> Row1
		=> Unsafe.As<T, Vector4D<T>>(ref M11);
	public Vector4D<T> Row2
		=> Unsafe.As<T, Vector4D<T>>(ref M21);
	public Vector4D<T> Row3
		=> Unsafe.As<T, Vector4D<T>>(ref M31);
	public Vector4D<T> Row4
		=> Unsafe.As<T, Vector4D<T>>(ref M41);

	public Vector4D<T> Column1
		=> new(M11, M21, M31, M41);
	public Vector4D<T> Column2
		=> new(M12, M22, M32, M42);
	public Vector4D<T> Column3
		=> new(M13, M23, M33, M43);
	public Vector4D<T> Column4
		=> new(M14, M24, M34, M44);
	
	public T this[int index]
	{
		get
		{
			if (index >= ElementCount)
				throw new ArgumentOutOfRangeException(nameof(index));

			return Unsafe.Add(ref M11, index);
		}
	}
	public T this[int x_, int _x]
	{
		get
		{
			if (_x >= RowsCount)
				throw new ArgumentOutOfRangeException(nameof(_x));

			if (x_ >= ColumnsCount)
				throw new ArgumentOutOfRangeException(nameof(x_));

			return Unsafe.Add(ref M11, x_ * 4 + _x);
		}
	}


	public Vector3D<T> Translation
		=> new(M41, M42, M43);

	public static readonly Matrix4x4<T> Identity = new(
		T.One, T.Zero, T.Zero, T.Zero,
		T.Zero, T.One, T.Zero, T.Zero,
		T.Zero, T.Zero, T.One, T.Zero,
		T.Zero, T.Zero, T.Zero, T.One
		);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public Matrix4x4(
		T m11, T m12, T m13, T m14,
		T m21, T m22, T m23, T m24,
		T m31, T m32, T m33, T m34,
		T m41, T m42, T m43, T m44
		)
	{
		M11 = m11;
		M12 = m12;
		M13 = m13;
		M14 = m14;
		M21 = m21;
		M22 = m22;
		M23 = m23;
		M24 = m24;
		M31 = m31;
		M32 = m32;
		M33 = m33;
		M34 = m34;
		M41 = m41;
		M42 = m42;
		M43 = m43;
		M44 = m44;
	}


	/// <summary>Indicates whether the current matrix is the identity matrix.</summary>
	/// <value><see langword="true" /> if the current matrix is the identity matrix; otherwise: <see langword="false" />.</value>
	public readonly bool IsIdentity
			=> M11 == T.One  && M22 == T.One  && M33 == T.One && M44 == T.One
			&& M12 == T.Zero && M13 == T.Zero && M14 == T.Zero
			&& M21 == T.Zero && M23 == T.Zero && M24 == T.Zero
			&& M31 == T.Zero && M32 == T.Zero && M34 == T.Zero
			&& M41 == T.Zero && M42 == T.Zero && M43 == T.Zero;
	public override readonly int GetHashCode()
	{
		HashCode hash = default;

		hash.Add(M11);
		hash.Add(M12);
		hash.Add(M13);
		hash.Add(M14);

		hash.Add(M21);
		hash.Add(M22);
		hash.Add(M23);
		hash.Add(M24);

		hash.Add(M31);
		hash.Add(M32);
		hash.Add(M33);
		hash.Add(M34);

		hash.Add(M41);
		hash.Add(M42);
		hash.Add(M43);
		hash.Add(M44);

		return hash.ToHashCode();
	}
	public override readonly string ToString()
		=> string.Format(CultureInfo.CurrentCulture, "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}",
							 M11, M12, M13, M14,
							 M21, M22, M23, M24,
							 M31, M32, M33, M34,
							 M41, M42, M43, M44);
}