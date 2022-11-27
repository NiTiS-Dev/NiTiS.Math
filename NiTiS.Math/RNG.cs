using System;

namespace NiTiS.Math;

public abstract class RNG
{
	/// <returns>Range of (0f; 1f)</returns>
	public abstract double Next();
	public virtual int Next(int min, int max) => Next(max - min) + min;
	public virtual int Next(int max) => (int)(max * Next());
	public virtual int this[int min, int max] => Next(min, max);
	public virtual int this[int max] => Next(max);
	public virtual float NextSingle()
		=> (float)Next();
#if NET5_0_OR_GREATER
	public virtual Half NextHalf()
		=> (Half)Next();
#else
	[Obsolete("Half type is not supported\nUse net5.0 or highter")]
	public virtual float NextHalf()
		=> (float)Next();
#endif
}
