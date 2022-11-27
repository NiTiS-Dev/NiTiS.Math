using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace NiTiS.Math;

public readonly struct Square<T> :
	IEquatable<Square<T>>,
	IEqualityOperators<Square<T>, Square<T>, bool>
	where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>
{
	public readonly Vector2D<T> Min;
	public readonly Vector2D<T> Max;
	public readonly Vector2D<T> Size => Max - Min;
	public readonly Vector2D<T> Center => (Min + Max) / Scalar<T>.Two;

	public Square(Vector2D<T> min, Vector2D<T> max)
		=> (Min, Max) = (min, max);
	public Square(Vector2D<T> min, T maxX, T maxY)
		=> (Min, Max) = (min, new(maxX, maxY));
	public Square(T minX, T minY, T maxX, T maxY)
		=> (Min, Max) = (new(minX, minY), new(maxX, maxY));
	public Square(T minX, T minY, Vector2D<T> max)
		=> (Min, Max) = (new(minX, minY), max);

	public readonly bool Equals(Square<T> other)
		=> this == other;
	public override readonly bool Equals([NotNullWhen(true)]object? obj)
		=> obj is Square<T> sqr ? Equals(sqr) : false;
	public override int GetHashCode()
		=> HashCode.Combine(Min, Max);
	public readonly Square<T> GetScaled(Vector2D<T> scale, Vector2D<T> anchor)
		=> new(
			(scale * (Min - anchor)) + anchor,
			(scale * (Max - anchor)) + anchor
			);
	public readonly bool Contains(Vector2D<T> point)
		=> point.X >= Min.X && point.Y >= Min.Y
		&& point.X <= Max.X && point.Y <= Min.Y;
	public readonly bool Contains(Square<T> point)
		=> point.Min.X >= Min.X && point.Min.Y >= Min.Y
		&& point.Max.X <= Max.X && point.Max.Y <= Min.Y;
	public readonly Square<T> GetTranslated(Vector2D<T> distance)
		=> new(Min + distance, Max + distance);

	public static bool operator ==(Square<T> left, Square<T> right)
		=> left.Min == right.Min
		&& left.Max == right.Max;
	public static bool operator !=(Square<T> left, Square<T> right)
		=> left.Min != right.Min
		|| left.Max != right.Max;
}