#pragma warning disable CS1591
using System.Globalization;
using System.Numerics;

namespace NiTiS.Math;

/// <summary>
/// Hook to some generic float constants.
/// </summary>
internal static class FloatScalar<T>
	where T : INumberBase<T>, IComparisonOperators<T, T, bool>
{
	private static T billboardEpsilon, billboardMinAngle, decomposeEpsilon, slerpEpsilon;

	public static T BillboardEpsilon
	{
		get
		{
			if (billboardEpsilon == T.Zero)
				return billboardEpsilon = T.Parse("1e-4", NumberStyles.Float, null);

			return billboardEpsilon;
		}
	}
	public static T SlerpEpsilon
	{
		get
		{
			if (slerpEpsilon == T.Zero)
				return slerpEpsilon = T.Parse("1e-6", NumberStyles.Float, null);

			return slerpEpsilon;
		}
	}
	public static T BillboardMinAngle
	{
		get
		{
			if (billboardMinAngle == T.Zero)
				return billboardMinAngle = T.Parse("0.99825467074800567042307630923151", NumberStyles.Float, null);

			return billboardMinAngle;
		}
	}
	public static T DecomposeEpsilon
	{
		get
		{
			if (decomposeEpsilon == T.Zero)
				return decomposeEpsilon = T.Parse("0.0001", NumberStyles.Float, null);

			return decomposeEpsilon;
		}
	}
}
#pragma warning restore