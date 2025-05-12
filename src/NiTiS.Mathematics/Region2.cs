using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

/// <summary>
/// Represent 2-dimensional region.
/// </summary>
[DataContract]
public struct Region2<T> : IEquatable<Region2<T>>, IFormattable
	where T : unmanaged, INumber<T>
{
	/// <summary>
	/// Region origin.
	/// </summary>
	[DataMember(Order = 0)]
	public Vector2<T> Origin;

	/// <summary>
	/// Size of the region.
	/// </summary>
	[DataMember(Order = 1)]
	public Vector2<T> Size;

	/// <summary>
	/// End of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly Vector2<T> End => Origin + Size;

	/// <summary>
	/// Center of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly Vector2<T> Center => Origin + (Size / (T.One + T.One));

	/// <summary>
	/// Creates new region with specified <paramref name="origin"/> and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region2(Vector2<T> origin, Vector2<T> size)
	{
		Origin = origin;
		Size = size;
	}

	/// <summary>
	/// Creates new region from zero point with specified <paramref name="size"/>.
	/// </summary>
	/// <param name="size">Region size.</param>
	public Region2(Vector2<T> size)
	{
		Origin = default;
		Size = size;
	}

	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Region2<T> reg && Equals(reg);
	}

	/// <inheritdoc/>
	public readonly bool Equals(Region2<T> other)
	{
		return this == other;
	}

	/// <inheritdoc/>
	public readonly override int GetHashCode()
	{
		return HashCode.Combine(Origin, Size);
	}

	/// <inheritdoc/>
	public readonly override string ToString()
	{
		return ToString("G", null);
	}

	/// <inheritdoc cref="ToString(string, IFormatProvider)"/>
	public readonly string ToString(string? format)
	{
		return ToString(format, null);
	}

	/// <inheritdoc/>
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
	{
		return $"{{Origin: {Origin}, Size: {Size}}}";
	}

	/// <summary>
	/// Checks if the region contains the specified point.
	/// </summary>
	/// <param name="point">The point to check.</param>
	/// <returns><c>true</c> if the point is within the region; otherwise, <c>false</c>.</returns>
	public readonly bool Contains(Vector2<T> point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
			&& point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y;
	}

	/// <summary>
	/// Checks if the region intersects with another region.
	/// </summary>
	/// <param name="other">The region to check.</param>
	/// <returns><c>true</c> if the regions intersect; otherwise, <c>false</c>.</returns>
	public readonly bool Intersects(Region2<T> other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
			&& Origin.Y < other.End.Y && End.Y > other.Origin.Y;
	}

	/// <summary>
	/// Calculates the intersection of the <paramref name="first"/> region with <paramref name="second"/> region.
	/// </summary>
	/// <param name="first">The region to intersect with <paramref name="second"/>.</param>
	/// <param name="second">The region to intersect with <paramref name="first"/>.</param>
	/// <returns>A new region representing the intersection, or a region with zero size if the regions do not intersect.</returns>
	public static Region2<T> Intersection(in Region2<T> first, in Region2<T> second)
	{
		T x1 = T.Max(first.Origin.X, second.Origin.X);
		T y1 = T.Max(first.Origin.Y, second.Origin.Y);
		T x2 = T.Min(first.End.X, second.End.X);
		T y2 = T.Min(first.End.Y, second.End.Y);

		if (x2 <= x1 || y2 <= y1)
		{
			return default;
		}

		return new Region2<T>(new Vector2<T>(x1, y1), new Vector2<T>(x2 - x1, y2 - y1));
	}

	/// <summary>
	/// Calculates the union of the <paramref name="first"/> region with <paramref name="second"/> region.
	/// </summary>
	/// <param name="first">The region to union with <paramref name="second"/>.</param>
	/// <param name="second">The region to union with <paramref name="first"/>.</param>
	/// <returns>A new region representing the union.</returns>
	public static Region2<T> Union(in Region2<T> first, in Region2<T> second)
	{
		T x1 = T.Min(first.Origin.X, second.Origin.X);
		T y1 = T.Min(first.Origin.Y, second.Origin.Y);
		T x2 = T.Max(first.End.X, second.End.X);
		T y2 = T.Max(first.End.Y, second.End.Y);

		return new Region2<T>(new Vector2<T>(x1, y1), new Vector2<T>(x2 - x1, y2 - y1));
	}

	/// <summary>
	/// Perform equality comparison.
	/// </summary>
	/// <param name="left">Left parameter.</param>
	/// <param name="right">Right parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	public static bool operator ==(Region2<T> left, Region2<T> right)
	{
		return left.Origin == right.Origin
			&& left.Size == right.Size;
	}

	/// <summary>
	/// Perform inequality comparison.
	/// </summary>
	/// <param name="left">Left parameter.</param>
	/// <param name="right">Right parameter.</param>
	/// <returns>Inequality of input parameters.</returns>
	public static bool operator !=(Region2<T> left, Region2<T> right)
	{
		return left.Origin != right.Origin
			|| left.Size != right.Size;
	}
}