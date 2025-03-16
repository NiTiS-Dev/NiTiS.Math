using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

/// <summary>
/// Represent 2-dimensional region.
/// </summary>
[DataContract]
public struct Region2i : IEquatable<Region2i>, IFormattable
{
	/// <summary>
	/// Region origin.
	/// </summary>
	[DataMember(Order = 0)]
	public Vector2i Origin;

	/// <summary>
	/// Size of the region.
	/// </summary>
	[DataMember(Order = 1)]
	public Vector2i Size;

	/// <summary>
	/// End of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly Vector2i End => Origin + Size;

	/// <summary>
	/// Creates new region with specified <paramref name="origin"/> and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region2i(Vector2i origin, Vector2i size)
	{
		Origin = origin;
		Size = size;
	}

	/// <summary>
	/// Creates new region from zero point with specified <paramref name="size"/>.
	/// </summary>
	/// <param name="size">Region size.</param>
	public Region2i(Vector2i size)
	{
		Origin = default;
		Size = size;
	}

	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Region2i reg && Equals(reg);
	}

	/// <inheritdoc/>
	public readonly bool Equals(Region2i other)
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

	/// <inheritdoc/>
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
	public readonly bool Contains(Vector2i point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
			&& point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y;
	}

	/// <summary>
	/// Checks if the region intersects with another region.
	/// </summary>
	/// <param name="other">The region to check.</param>
	/// <returns><c>true</c> if the regions intersect; otherwise, <c>false</c>.</returns>
	public readonly bool Intersects(Region2i other)
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
	public static Region2i Intersection(in Region2i first, in Region2i second)
	{
		int x1 = int.Max(first.Origin.X, second.Origin.X);
		int y1 = int.Max(first.Origin.Y, second.Origin.Y);
		int x2 = int.Min(first.End.X, second.End.X);
		int y2 = int.Min(first.End.Y, second.End.Y);

		if (x2 <= x1 || y2 <= y1)
		{
			return default;
		}

		return new Region2i(new Vector2i(x1, y1), new Vector2i(x2 - x1, y2 - y1));
	}

	/// <summary>
	/// Calculates the union of the <paramref name="first"/> region with <paramref name="second"/> region.
	/// </summary>
	/// <param name="first">The region to union with <paramref name="second"/>.</param>
	/// <param name="second">The region to union with <paramref name="first"/>.</param>
	/// <returns>A new region representing the union.</returns>
	public static Region2i Union(in Region2i first, in Region2i second)
	{
		int x1 = int.Min(first.Origin.X, second.Origin.X);
		int y1 = int.Min(first.Origin.Y, second.Origin.Y);
		int x2 = int.Max(first.End.X, second.End.X);
		int y2 = int.Max(first.End.Y, second.End.Y);

		return new Region2i(new Vector2i(x1, y1), new Vector2i(x2 - x1, y2 - y1));
	}

	/// <summary>
	/// Perform equality comparison.
	/// </summary>
	/// <param name="left">Left parameter.</param>
	/// <param name="right">Right parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	public static bool operator ==(Region2i left, Region2i right)
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
	public static bool operator !=(Region2i left, Region2i right)
	{
		return left.Origin != right.Origin
			|| left.Size != right.Size;
	}
}