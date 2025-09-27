using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector3{T}"/> extensions type.
/// </summary>
public static class Vector3Extensions
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
	public static float LengthSquared(this Vector3<float> vector)
	{
		return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
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
	public static float Length(this Vector3<float> vector)
	{
		return float.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
	}

	extension<T>(Vector3<T> vector)
		where T : unmanaged, INumber<T>, IRootFunctions<T>
	{
		public T LengthSquared => vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;

		public T Length => T.Sqrt(vector.LengthSquared);
	}

	extension<T>(Vector3<T>)
		where T : unmanaged, IBinaryInteger<T>
	{
		/// <summary>
		/// Perform bitwise or operation.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3<T> operator |(Vector3<T> left, Vector3<T> right)
		{
			return new(
				left.X | right.X,
				left.Y | right.Y,
				left.Z | right.Z);
		}

		/// <summary>
		/// Perform bitwise and operation.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3<T> operator &(Vector3<T> left, Vector3<T> right)
		{
			return new(
				left.X & right.X,
				left.Y & right.Y,
				left.Z & right.Z);
		}

		/// <summary>
		/// Perform bitwise xor operation.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3<T> operator ^(Vector3<T> left, Vector3<T> right)
		{
			return new(
				left.X ^ right.X,
				left.Y ^ right.Y,
				left.Z ^ right.Z);
		}
	}
}