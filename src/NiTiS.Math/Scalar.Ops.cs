using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

/// <summary>
/// Provides methods for scalar units.
/// </summary>
public static class Scalar
{
	#region ConvertCornerUnits

	/// <summary>
	/// Convert degrees to radians.
	/// </summary>
	/// <param name="degrees">Degrees.</param>
	/// <returns>Radians.</returns>
	[MethodImpl(AggressiveInlining)]
	public static float ToRadians(float degrees)
		=> degrees * float.Pi / 180f;

	/// <summary>
	/// Convert degrees to radians.
	/// </summary>
	/// <param name="degrees">Degrees.</param>
	/// <returns>Radians.</returns>
	[MethodImpl(AggressiveInlining)]
	public static double ToRadians(double degrees)
		=> degrees * double.Pi / 180d;

	/// <summary>
	/// Convert radians to degrees
	/// </summary>
	/// <param name="radians">Radians.</param>
	/// <returns>Degrees.</returns>
	[MethodImpl(AggressiveInlining)]
	public static float ToDegrees(float radians)
		=> radians * (180f / float.Pi);

	/// <summary>
	/// Convert radians to degrees
	/// </summary>
	/// <param name="radians">Radians.</param>
	/// <returns>Degrees.</returns>
	[MethodImpl(AggressiveInlining)]
	public static double ToDegrees(double radians)
		=> radians * (180d / double.Pi);

	#endregion
	
	#region Prime
	/// <summary>
	/// Cheks if <paramref name="number"/> is prime.<br/>
	/// Prime is a positive natural number greater than 1 that is not a product of two smaller natural numbers.
	/// </summary>
	/// <returns><see langword="true"/> when <paramref name="number"/> is prime.</returns>
	public static bool IsPrime(long number)
	{
		if (number < 2) return false;
		if (number == 2 || number == 3) return true;
		if (number % 2 == 0 || number % 3 == 0) return false;

		long limit = (long)MathF.Sqrt(number);

		for (long i = 5; i <= limit; i += 6)
		{
			if (number % i == 0 || number % (i + 2) == 0)
			{
				return false;
			}
		}
		return true;
	}
	/// <summary>
	/// Cheks if <paramref name="number"/> is prime.<br/>
	/// Prime is a positive natural number greater than 1 that is not a product of two smaller natural numbers.
	/// </summary>
	/// <returns><see langword="true"/> when <paramref name="number"/> is prime.</returns>
	public static bool IsPrime(int number)
	{
		if (number < 2) return false;
		if (number == 2 || number == 3) return true;
		if (number % 2 == 0 || number % 3 == 0) return false;

		int limit = (int)MathF.Sqrt(number);

		for (long i = 5; i <= limit; i += 6)
		{
			if (number % i == 0 || number % (i + 2) == 0)
			{
				return false;
			}
		}
		return true;
	}
	#endregion Prime
	
	#region GCD
	/// <summary>
	/// Resolves greatest divisor for <paramref name="a"/> and <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first argument.</param>
	/// <param name="b">The second argument.</param>
	/// <returns>Greatest common divisor.</returns>
	public static N GreatestCommonDivisor<N>(N a, N b)
		where N :
			INumberBase<N>,
			IComparisonOperators<N, N, bool>,
			IModulusOperators<N, N, N>
		=> b == N.Zero ? (a < N.Zero ? -a : a)
		: GreatestCommonDivisor(b, a % b);
	#endregion

	#region LCM
	/// <summary>
	/// Resolves leastest multiple for <paramref name="a"/> and <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first argument.</param>
	/// <param name="b">The second argument.</param>
	/// <returns>Least common multiple.</returns>
	public static N LeastCommonMultiple<N>(N a, N b)
		where N :
			INumberBase<N>,
			IComparisonOperators<N, N, bool>,
			IModulusOperators<N, N, N>
	{
		N lcm = (a / GreatestCommonDivisor(a, b)) * b;
		return lcm > N.Zero ? lcm : -lcm;
	}
	#endregion

	#region Lerp
	public static N Lerp<N>(N origin, N end, N progress)
		where N : INumberBase<N>
		=> origin + (end - origin) * progress;
	#endregion
}
