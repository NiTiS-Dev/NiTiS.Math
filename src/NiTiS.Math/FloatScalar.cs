#pragma warning disable CS1591
using System.Globalization;
using System.Numerics;

namespace NiTiS.Math;

/// <summary>
/// Hook to some generic float constants.
/// </summary>
internal static class FloatScalar<F>
	where F : INumberBase<F>, IComparisonOperators<F, F, bool>
{
	private static F billboardEpsilon, billboardMinAngle, decomposeEpsilon, slerpEpsilon;

	public static F BillboardEpsilon
	{
		get
		{
			if (billboardEpsilon == F.Zero)
				return billboardEpsilon = F.Parse("1e-4", NumberStyles.Float, null);

			return billboardEpsilon;
		}
	}
	public static F SlerpEpsilon
	{
		get
		{
			if (slerpEpsilon == F.Zero)
				return slerpEpsilon = F.Parse("1e-6", NumberStyles.Float, null);

			return slerpEpsilon;
		}
	}
	public static F BillboardMinAngle
	{
		get
		{
			if (billboardMinAngle == F.Zero)
				return billboardMinAngle = F.Parse("0.99825467074800567042307630923151", NumberStyles.Float, null);

			return billboardMinAngle;
		}
	}
	public static F DecomposeEpsilon
	{
		get
		{
			if (decomposeEpsilon == F.Zero)
				return decomposeEpsilon = F.Parse("0.0001", NumberStyles.Float, null);

			return decomposeEpsilon;
		}
	}
}
#pragma warning restore