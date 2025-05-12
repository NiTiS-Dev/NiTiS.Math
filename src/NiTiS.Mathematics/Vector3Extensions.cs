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

	public static float LengthSquared(this Vector3<float> vec)
	{
		return vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z;
	}

	public static float Length(this Vector3<float> vec)
	{
		return float.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
	}
}