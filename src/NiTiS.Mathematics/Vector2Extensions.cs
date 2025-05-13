namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector2{T}"/> extensions type.
/// </summary>
public static class Vector2Extensions
{
	/// <summary>
	/// Gets squared magnitude of <paramref name="vector"/>.
	/// </summary>
	/// <param name="vector">The vector to get squared magnitude.</param>
	/// <returns>Squared magnitude of provided vector.</returns>
	public static float LengthSquared(this Vector2<float> vector)
	{
		return vector.X * vector.X + vector.Y * vector.Y;
	}

	/// <summary>
	/// Gets magnitude of <paramref name="vector"/>.
	/// </summary>
	/// <param name="vector">The vector to get magnitude.</param>
	/// <returns>Magnitude of provided vector.</returns>
	public static float Length(this Vector2<float> vector)
	{
		return float.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
	}
}