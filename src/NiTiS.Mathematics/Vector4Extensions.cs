using System.ComponentModel;
using System.Numerics;

namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector4{T}"/> extensions type.
/// </summary>
public static class Vector4Extensions
{
	/// <summary>
	/// Gets squared magnitude of <paramref name="vector"/>.
	/// </summary>
	/// <param name="vector">The vector to get squared magnitude.</param>
	/// <returns>Squared magnitude of provided vector.</returns>
#if NET10_0_OR_GREATER
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public static float LengthSquared(this Vector4<float> vector)
	{
		return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;
	}

	/// <summary>
	/// Gets magnitude of <paramref name="vector"/>.
	/// </summary>
	/// <param name="vector">The vector to get magnitude.</param>
	/// <returns>Magnitude of provided vector.</returns>
#if NET10_0_OR_GREATER
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public static float Length(this Vector4<float> vector)
	{
		return float.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W);
	}

	extension<T>(Vector4<T> vector)
		where T : unmanaged, INumber<T>, IRootFunctions<T>
	{
		public T LengthSquared => vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;

		public T Length => T.Sqrt(vector.LengthSquared);
	}
}