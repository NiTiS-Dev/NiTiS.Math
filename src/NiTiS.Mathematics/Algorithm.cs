using System;
using System.Numerics;

namespace NiTiS.Mathematics;

/// <summary>
/// Set of useful mathematics algorithms.
/// </summary>
public static class Algorithm
{
	/// <summary>
	/// Checks if <paramref name="number"/> is prime.<br/>
	/// Prime is a positive natural number greater than 1 that is not a product of two smaller natural numbers.
	/// </summary>
	/// <returns><see langword="true"/> when <paramref name="number"/> is prime.</returns>
	public static bool IsPrime(this long number)
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
	
	/// <inheritdoc cref="IsPrime(long)"/>
	public static bool IsPrime(this int number)
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
	
	/// <inheritdoc cref="IsPrime(long)"/>
	public static bool IsPrime(this ulong number)
	{
		if (number < 2) return false;
		if (number == 2 || number == 3) return true;
		if (number % 2 == 0 || number % 3 == 0) return false;

		uint limit = (uint)MathF.Sqrt(number);

		for (ulong i = 5; i <= limit; i += 6)
		{
			if (number % i == 0 || number % (i + 2) == 0)
			{
				return false;
			}
		}
		return true;
	}
	
	/// <inheritdoc cref="IsPrime(long)"/>
	public static bool IsPrime(this uint number)
	{
		if (number < 2) return false;
		if (number == 2 || number == 3) return true;
		if (number % 2 == 0 || number % 3 == 0) return false;

		uint limit = (uint)MathF.Sqrt(number);

		for (ulong i = 5; i <= limit; i += 6)
		{
			if (number % i == 0 || number % (i + 2) == 0)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Resolves greatest divisor for <paramref name="a"/> and <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first argument.</param>
	/// <param name="b">The second argument.</param>
	/// <returns>Greatest common divisor.</returns>
	public static N GreatestCommonDivisor<N>(N a, N b)
		where N : INumberBase<N>, IComparisonOperators<N, N, bool>, IModulusOperators<N, N, N>
	{
		while (true)
		{
			if (b == N.Zero) return a < N.Zero ? -a : a;
			N temp = a;
			a = b;
			b = temp % b;
		}
	}

	/// <summary>
	/// Resolves least multiple for <paramref name="a"/> and <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first argument.</param>
	/// <param name="b">The second argument.</param>
	/// <returns>Least common multiple.</returns>
	public static N LeastCommonMultiple<N>(N a, N b)
		where N : INumberBase<N>, IComparisonOperators<N, N, bool>, IModulusOperators<N, N, N>
	{
		N lcm = (a / GreatestCommonDivisor(a, b)) * b;
		return lcm > N.Zero ? lcm : -lcm;
	}
}