using System.Numerics;
using System.Runtime.CompilerServices;

namespace NiTiS.Mathematics;

/// <summary>
/// <see cref="Vector4{T}"/> extensions type.
/// </summary>
public static class Vector4Extensions
{
	extension<T>(Vector<T>)
		where T : unmanaged, INumberBase<T>
	{
		/// <summary>
		/// Vector instance with all zeroes.
		/// </summary>
		public static Vector4<T> Zero => default;

		/// <summary>
		/// Vector (1, 0, 0, 0).
		/// </summary>
		public static Vector4<T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero);

		/// <summary>
		/// Vector (0, 1, 0, 0).
		/// </summary>
		public static Vector4<T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero);

		/// <summary>
		/// Vector (0, 0, 1, 0).
		/// </summary>
		public static Vector4<T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero);

		/// <summary>
		/// Vector (0, 0, 0, 1).
		/// </summary>
		public static Vector4<T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One);

	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IAdditionOperators<T, T, T>
	{
		/// <summary>
		/// Perform vector addition.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator +(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X + rhs.X,
				lhs.Y + rhs.Y,
				lhs.Z + rhs.Z,
				lhs.W + rhs.W);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, ISubtractionOperators<T, T, T>
	{
		/// <summary>
		/// Perform vector substraction.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator -(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X - rhs.X,
				lhs.Y - rhs.Y,
				lhs.Z - rhs.Z,
				lhs.W - rhs.W);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IMultiplyOperators<T, T, T>
	{
		/// <summary>
		/// Perform vector multiplication.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator *(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X * rhs.X,
				lhs.Y * rhs.Y,
				lhs.Z * rhs.Z,
				lhs.W * rhs.W);
		}

		/// <summary>
		/// Perform vector by scalar multiplication.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator *(T lhs, Vector4<T> rhs)
		{
			return new(
				lhs * rhs.X,
				lhs * rhs.Y,
				lhs * rhs.Z,
				lhs * rhs.W);
		}

		/// <summary>
		/// Perform vector by scalar multiplication.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator *(Vector4<T> lhs, T rhs)
		{
			return new(
				lhs.X * rhs,
				lhs.Y * rhs,
				lhs.Z * rhs,
				lhs.W * rhs);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IDivisionOperators<T, T, T>
	{
		/// <summary>
		/// Perform vector division.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator /(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X / rhs.X,
				lhs.Y / rhs.Y,
				lhs.Z / rhs.Z,
				lhs.W / rhs.W);
		}

		/// <summary>
		/// Perform vector by scalar division.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator /(T lhs, Vector4<T> rhs)
		{
			return new(
				lhs / rhs.X,
				lhs / rhs.Y,
				lhs / rhs.Z,
				lhs / rhs.W);
		}

		/// <summary>
		/// Perform vector by scalar division.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator /(Vector4<T> lhs, T rhs)
		{
			return new(
				lhs.X / rhs,
				lhs.Y / rhs,
				lhs.Z / rhs,
				lhs.W / rhs);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IModulusOperators<T, T, T>
	{
		/// <summary>
		/// Perform vector modulo operation.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator %(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X % rhs.X,
				lhs.Y % rhs.Y,
				lhs.Z % rhs.Z,
				lhs.W % rhs.W);
		}

		/// <summary>
		/// Perform vector by scalar modulo operation.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator %(T lhs, Vector4<T> rhs)
		{
			return new(
				lhs % rhs.X,
				lhs % rhs.Y,
				lhs % rhs.Z,
				lhs % rhs.W);
		}

		/// <summary>
		/// Perform vector by scalar modulo operation.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator %(Vector4<T> lhs, T rhs)
		{
			return new(
				lhs.X % rhs,
				lhs.Y % rhs,
				lhs.Z % rhs,
				lhs.W % rhs);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IEqualityOperators<T, T, bool>
	{
		/// <summary>
		/// Perform equality comparing.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Equality of input parameters.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
		[OverloadResolutionPriority(1)]
#endif
		public static bool operator ==(Vector4<T> left, Vector4<T> right)
		{
			return left.X == right.X
				&& left.Y == right.Y
				&& left.Z == right.Z
				&& left.W == right.W;
		}

		/// <summary>
		/// Perform inequality comparing.
		/// </summary>
		/// <param name="left">The left operation parameter.</param>
		/// <param name="right">The right operation parameter.</param>
		/// <returns>Inequality of input parameters.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
		[OverloadResolutionPriority(1)]
#endif
		public static bool operator !=(Vector4<T> left, Vector4<T> right)
		{
			return left.X != right.X
				|| left.Y != right.Y
				|| left.Z != right.Z
				|| left.W != right.W;
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IComparisonOperators<T, T, bool>
	{
		/// <summary>Restricts a vector between a minimum and a maximum value.</summary>
		/// <param name="value">The vector to restrict.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The restricted vector.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> Clamp(Vector4<T> value, Vector4<T> min, Vector4<T> max)
		{
			return Min(Max(value, min), max);
		}

		/// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <returns>The maximized vector.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> Max(Vector4<T> value1, Vector4<T> value2)
		{
			return new(
				(value1.X > value2.X) ? value1.X : value2.X,
				(value1.Y > value2.Y) ? value1.Y : value2.Y,
				(value1.Z > value2.Z) ? value1.Z : value2.Z,
				(value1.W > value2.W) ? value1.W : value2.W
			);
		}

		/// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <returns>The minimized vector.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> Min(Vector4<T> value1, Vector4<T> value2)
		{
			return new(
				(value1.X < value2.X) ? value1.X : value2.X,
				(value1.Y < value2.Y) ? value1.Y : value2.Y,
				(value1.Z < value2.Z) ? value1.Z : value2.Z,
				(value1.W < value2.W) ? value1.W : value2.W
			);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IUnaryPlusOperators<T, T>
	{
		/// <summary>
		/// Perform vector unary plus.
		/// </summary>
		/// <param name="value">Vector to unary plus.</param>
		/// <returns>Operation result.</returns>
		public static Vector4<T> operator +(Vector4<T> value)
		{
			return new(
				+value.X,
				+value.Y,
				+value.Z,
				+value.W);
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IUnaryNegationOperators<T, T>
	{
		/// <summary>
		/// Perform vector negation.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <returns>Operation result.</returns>
		public static Vector4<T> operator -(Vector4<T> value)
		{
			return new(
				-value.X,
				-value.Y,
				-value.Z,
				-value.W);
		}
	}

	extension(Vector4<bool> vector)
	{
		public static Vector4<bool> operator &(Vector4<bool> lhs, Vector4<bool> rhs)
		{
			return new(
				lhs.X & rhs.X,
				lhs.Y & rhs.Y,
				lhs.Z & rhs.Z,
				lhs.W & rhs.W);
		}

		public static Vector4<bool> operator &(Vector4<bool> lhs, bool rhs)
		{
			return new(
				lhs.X & rhs,
				lhs.Y & rhs,
				lhs.Z & rhs,
				lhs.W & rhs);
		}

		public static Vector4<bool> operator &(bool lhs, Vector4<bool> rhs)
		{
			return new(
				lhs & rhs.X,
				lhs & rhs.Y,
				lhs & rhs.Z,
				lhs & rhs.W);
		}

		public static Vector4<bool> operator |(Vector4<bool> lhs, Vector4<bool> rhs)
		{
			return new(
				lhs.X | rhs.X,
				lhs.Y | rhs.Y,
				lhs.Z | rhs.Z,
				lhs.W | rhs.W);
		}

		public static Vector4<bool> operator |(Vector4<bool> lhs, bool rhs)
		{
			return new(
				lhs.X | rhs,
				lhs.Y | rhs,
				lhs.Z | rhs,
				lhs.W | rhs);
		}

		public static Vector4<bool> operator |(bool lhs, Vector4<bool> rhs)
		{
			return new(
				lhs | rhs.X,
				lhs | rhs.Y,
				lhs | rhs.Z,
				lhs | rhs.W);
		}

		public static Vector4<bool> operator ^(Vector4<bool> lhs, Vector4<bool> rhs)
		{
			return new(
				lhs.X ^ rhs.X,
				lhs.Y ^ rhs.Y,
				lhs.Z ^ rhs.Z,
				lhs.W ^ rhs.W);
		}

		public static Vector4<bool> operator ^(Vector4<bool> lhs, bool rhs)
		{
			return new(
				lhs.X ^ rhs,
				lhs.Y ^ rhs,
				lhs.Z ^ rhs,
				lhs.W ^ rhs);
		}

		public static Vector4<bool> operator ^(bool lhs, Vector4<bool> rhs)
		{
			return new(
				lhs ^ rhs.X,
				lhs ^ rhs.Y,
				lhs ^ rhs.Z,
				lhs ^ rhs.W);
		}

		public static Vector4<bool> operator !(Vector4<bool> value)
		{
			return new(
				!value.X,
				!value.Y,
				!value.Z,
				!value.W);
		}

		/// <summary>
		/// Returns <see langword="true"/> when all elements are <see langword="true"/>.
		/// </summary>
		public bool All => vector.X && vector.Y && vector.Z && vector.W;

		/// <summary>
		/// Returns <see langword="true"/> when any element is <see langword="true"/>.
		/// </summary>
		public bool Any => vector.X || vector.Y || vector.Z || vector.W;
	}

	extension<T>(Vector4<T> vector)
		where T : unmanaged, IRootFunctions<T>
	{
		/// <summary>
		/// The squared length of <see langword="this"/> vector.
		/// </summary>
		/// <seealso cref="get_Length{T}(Vector4{T})"/>
		public T LengthSquared => vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;

		/// <summary>
		/// The length of <see langword="this"/> vector.
		/// </summary>
		/// <seealso cref="get_LengthSquared{T}(Vector4{T})"/>
		public T Length => T.Sqrt(vector.LengthSquared);
	}

	extension<T>(Vector4<T> vector)
		where T : unmanaged, IRootFunctions<T>, IPowerFunctions<T>
	{
		/// <summary>
		/// Calculates the distance from <see langword="this"/> vector to another vector.
		/// </summary>
		/// <param name="to">The vector to which the distance will be calculated.</param>
		/// <returns>The distance between the two vectors.</returns>
		public T DistanceTo(Vector4<T> to)
		{
			return T.Sqrt(
				T.Pow(to.X - vector.X, T.CreateChecked(2)) +
				T.Pow(to.Y - vector.Y, T.CreateChecked(2)) +
				T.Pow(to.Z - vector.Z, T.CreateChecked(2)) +
				T.Pow(to.W - vector.W, T.CreateChecked(2)));
		}
	}

	extension<T>(Vector4<T>)
		where T : unmanaged, IBinaryInteger<T>
	{
		/// <summary>
		/// Perform bitwise or operation.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator |(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X | rhs.X,
				lhs.Y | rhs.Y,
				lhs.Z | rhs.Z,
				lhs.W | rhs.W);
		}

		/// <summary>
		/// Perform bitwise and operation.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator &(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X & rhs.X,
				lhs.Y & rhs.Y,
				lhs.Z & rhs.Z,
				lhs.W & rhs.W);
		}

		/// <summary>
		/// Perform bitwise xor operation.
		/// </summary>
		/// <param name="lhs">The left operation parameter.</param>
		/// <param name="rhs">The right operation parameter.</param>
		/// <returns>Operation result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4<T> operator ^(Vector4<T> lhs, Vector4<T> rhs)
		{
			return new(
				lhs.X ^ rhs.X,
				lhs.Y ^ rhs.Y,
				lhs.Z ^ rhs.Z,
				lhs.W ^ rhs.W);
		}
	}
}