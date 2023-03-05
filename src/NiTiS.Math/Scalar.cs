using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

internal static class Scalar<N>
	where N : INumberBase<N>
{
	public static readonly N Two;
	public static readonly N Half;

	static Scalar()
	{
		Two = N.One + N.One;
		Half = N.One / Two;
	}
}