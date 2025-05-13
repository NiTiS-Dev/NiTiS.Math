namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector3{T}"/> extensions type.
/// </summary>
public static class Vector3Extensions
{
#if FALSE // Wait until C#14 official release
	extension(Vector3<float> vec)
	{
		/// <summary>
		/// Gets vector magnitude.
		/// </summary>
		public float Length => float.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
		
		/// <summary>
		/// Gets squared vector magnitude.
		/// </summary>
		public float LengthSquared => vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z;
	}
#endif

	/// <summary>
	/// Gets squared magnitude of <paramref name="vector"/>.
	/// </summary>
	/// <param name="vector">The vector to get squared magnitude.</param>
	/// <returns>Squared magnitude of provided vector.</returns>
	public static float LengthSquared(this Vector3<float> vector)
	{
		return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
	}

	/// <summary>
	/// Gets magnitude of <paramref name="vector"/>.
	/// </summary>
	/// <param name="vector">The vector to get magnitude.</param>
	/// <returns>Magnitude of provided vector.</returns>
	public static float Length(this Vector3<float> vector)
	{
		return float.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
	}
}