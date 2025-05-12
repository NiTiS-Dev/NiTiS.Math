namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector2{T}"/> extensions type.
/// </summary>
public static class Vector2Extensions
{
	public static float LengthSquared(this Vector2<float> vec)
	{
		return vec.X * vec.X + vec.Y * vec.Y;
	}

	public static float Length(this Vector2<float> vec)
	{
		return float.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
	}
}