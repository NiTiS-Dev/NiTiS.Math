using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector2{T}"/> extensions type.
/// </summary>
public static class Vector2Extensions
{
	extension<T>(Vector2<T> vector)
		where T : unmanaged, INumber<T>, IRootFunctions<T>
	{
		public T LengthSquared => vector.X * vector.X + vector.Y * vector.Y;

		public T Length => T.Sqrt(vector.LengthSquared);
	}

	extension<T>(Vector2<T>)
		where T : unmanaged, IBinaryInteger<T>
	{
		/// <summary>
		/// Perform bitwise or operation.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2<T> operator |(Vector2<T> left, Vector2<T> right)
		{
			return new(
				left.X | right.X,
				left.Y | right.Y);
		}

		/// <summary>
		/// Perform bitwise and operation.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2<T> operator &(Vector2<T> left, Vector2<T> right)
		{
			return new(
				left.X & right.X,
				left.Y & right.Y);
		}

		/// <summary>
		/// Perform bitwise xor operation.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2<T> operator ^(Vector2<T> left, Vector2<T> right)
		{
			return new(
				left.X ^ right.X,
				left.Y ^ right.Y);
		}
	}
}