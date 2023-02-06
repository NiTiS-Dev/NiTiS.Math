using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public readonly unsafe struct Mat4<T>
	where T : unmanaged, INumberBase<T>
{
	#region Matrix
	public readonly T M11;
	public readonly T M12;
	public readonly T M13;
	public readonly T M14;

	public readonly T M21;
	public readonly T M22;
	public readonly T M23;
	public readonly T M24;

	public readonly T M31;
	public readonly T M32;
	public readonly T M33;
	public readonly T M34;

	public readonly T M41;
	public readonly T M42;
	public readonly T M43;
	public readonly T M44;
	#endregion
	public Vector3D<T> Translation
		=> new(M41, M42, M43);
	public static Mat4<T> Identity => new(
		T.One, T.Zero, T.Zero, T.Zero,
		T.Zero, T.One, T.Zero, T.Zero,
		T.Zero, T.Zero, T.One, T.Zero,
		T.Zero, T.Zero, T.Zero, T.One
		);

	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public Mat4(
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
	/// <value><see langword="true" /> if the current matrix is the identity matrix; otherwise, <see langword="false" />.</value>
	public readonly bool IsIdentity
	{
		get
		{
			return
				M11 == T.One  && M22 == T.One  && M33 == T.One  && M44 == T.One &&
				M12 == T.Zero && M13 == T.Zero && M14 == T.Zero &&
				M21 == T.Zero && M23 == T.Zero && M24 == T.Zero &&
				M31 == T.Zero && M32 == T.Zero && M34 == T.Zero &&
				M41 == T.Zero && M42 == T.Zero && M43 == T.Zero;
		}
	}
}