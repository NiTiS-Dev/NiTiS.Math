using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

internal static class Scalar<T>
	where T : INumberBase<T>
{
	public static T Two
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => T.One + T.One;
	}
}