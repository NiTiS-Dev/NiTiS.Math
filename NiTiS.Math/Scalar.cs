using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

internal static class Scalar<T>
	where T : INumberBase<T>
{
	public static readonly T Two;
	public static readonly T Half;

	static Scalar()
	{
		Two = T.One + T.One;
		Half = T.One / Two;
	}
}