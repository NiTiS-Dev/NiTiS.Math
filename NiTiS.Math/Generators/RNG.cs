using System;

namespace NiTiS.Math.Generators;

/// <summary>
/// The random number generator base
/// </summary>
public abstract class RNG
{
	/// <summary>
	/// Generates a pseudo random number in range from 0 to 1
	/// </summary>
	/// <returns>Range of (0f; 1f)</returns>
	public abstract double Next();
	/// <summary>
	/// Generates a pseudo random number in range from <paramref name="min"/> to <paramref name="max"/>
	/// </summary>
	/// <param name="min">Minimum posible value</param>
	/// <param name="max">Maximum posible value</param>
	/// <returns>[<paramref name="min"/>; <paramref name="max"/>)</returns>
	public virtual int Next(int min, int max) => Next(max - min) + min;
	/// <summary>
	/// Generates a pseudo random number in range from 0 to <paramref name="max"/>
	/// </summary>
	/// <param name="max">Maximum posible value</param>
	/// <returns>[0; <paramref name="max"/>)</returns>
	public virtual int Next(int max) => (int)(max * Next());
	/// <summary>
	/// Generates a pseudo random number in range from <paramref name="min"/> to <paramref name="max"/>
	/// </summary>
	/// <param name="min">Minimum posible value</param>
	/// <param name="max">Maximum posible value</param>
	/// <returns>[<paramref name="min"/>; <paramref name="max"/>)</returns>
	public virtual int this[int min, int max] => Next(min, max);
	/// <summary>
	/// Generates a pseudo random number in range from 0 to <paramref name="max"/>
	/// </summary>
	/// <param name="max">Maximum posible value</param>
	/// <returns>[0; <paramref name="max"/>)</returns>
	public virtual int this[int max] => Next(max);
	public virtual float NextSingle()
		=> (float)Next();
	public virtual Half NextHalf()
		=> (Half)Next();
}
