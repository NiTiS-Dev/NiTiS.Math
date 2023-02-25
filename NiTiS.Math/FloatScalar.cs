using System.Globalization;
using System.Numerics;

namespace NiTiS.Math;

internal static class FloatScalar<T>
	where T : INumberBase<T>, IComparisonOperators<T, T, bool>
{
	private static T billboardEpsilon, billboardMinAngle, decomposeEpsilon;

	public static T BillboardEpsilon
	{
		get
		{
			if (billboardEpsilon == T.Zero)
				return billboardEpsilon = T.Parse("1e-4", NumberStyles.Float, null);

			return billboardEpsilon;
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