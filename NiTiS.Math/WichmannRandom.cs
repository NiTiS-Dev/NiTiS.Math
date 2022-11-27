using System;

namespace NiTiS.Math;

public sealed class WichmannRandom : RNG
{
	private double s1, s2, s3;
	public WichmannRandom(ushort seed)
	{
		if (seed > 30000)
		{
			throw new ArgumentOutOfRangeException(nameof(seed));
		}
		s1 = seed;
		s2 = seed + 1;
		s3 = seed + 2;
	}
	public override double Next()
	{
		s1 = 171 * (s1 % 177) - 2 * (s1 / 177);
		if (s1 < 0) { s1 += 30269; }
		s2 = 172 * (s2 % 176) - 35 * (s2 / 176);
		if (s2 < 0) { s2 += 30307; }
		s3 = 170 * (s3 % 178) - 63 * (s3 / 178);
		if (s3 < 0) { s3 += 30323; }
		double r = (s1 * 1.0) / 30269 + (s2 * 1.0) / 30307 + (s3 * 1.0) / 30323;
		return r - SMath.Truncate(r);
	}
	/// <summary>
	/// Creates a new instance using millisecond as seed value
	/// </summary>
	/// <returns>A new random instance</returns>
	public static WichmannRandom Create()
		=> new((ushort)DateTime.Now.Millisecond);
}