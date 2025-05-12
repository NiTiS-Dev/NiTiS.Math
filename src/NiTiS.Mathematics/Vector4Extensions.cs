namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector2{T}"/> extensions type.
/// </summary>
public static class Vector4Extensions
{
	public static float LengthSquared(this Vector4<float> vec)
	{
		return vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z + vec.W * vec.W;
	}

	public static float Length(this Vector4<float> vec)
	{
		return float.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z + vec.W * vec.W);
	}
}