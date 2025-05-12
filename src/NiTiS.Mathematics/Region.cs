using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

/// <summary>
/// Represent 1-dimensional region.
/// </summary>
[DataContract]
public struct Region<T> : IEquatable<Region<T>>, IFormattable
	where T : unmanaged, INumber<T>
{
	/// <summary>
	/// Region origin.
	/// </summary>
	[DataMember(Order = 0)]
	public T Origin;

	/// <summary>
	/// Size of the region.
	/// </summary>
	[DataMember(Order = 1)]
	public T Size;

	/// <summary>
	/// End of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly T End => Origin + Size;

	/// <summary>
	/// Center of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly T Center => Origin + (Size / (T.One + T.One));

	/// <summary>
	/// Creates new region with specified <paramref name="origin"/> and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region(T origin, T size)
	{
		Origin = origin;
		Size = size;
	}

	/// <summary>
	/// Creates new region from zero point with specified <paramref name="size"/>.
	/// </summary>
	/// <param name="size">Region size.</param>
	public Region(T size)
	{
		Origin = default;
		Size = size;
	}

	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Region<T> reg && Equals(reg);
	}

	/// <inheritdoc/>
	public readonly bool Equals(Region<T> other)
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
	public readonly bool Contains(T point)
	{
		return point >= Origin && point < Origin + Size;
	}

	/// <summary>
	/// Checks if the region intersects with another region.
	/// </summary>
	/// <param name="other">The region to check.</param>
	/// <returns><c>true</c> if the regions intersect; otherwise, <c>false</c>.</returns>
	public readonly bool Intersects(Region<T> other)
	{
		return Origin < other.End && End > other.Origin;
	}

	/// <summary>
	/// Calculates the intersection of the <paramref name="first"/> region with <paramref name="second"/> region.
	/// </summary>
	/// <param name="first">The region to intersect with <paramref name="second"/>.</param>
	/// <param name="second">The region to intersect with <paramref name="first"/>.</param>
	/// <returns>A new region representing the intersection, or a region with zero size if the regions do not intersect.</returns>
	public static Region<T> Intersection(Region<T> first, Region<T> second)
	{
		T x1 = T.Max(first.Origin, second.Origin);
		T x2 = T.Min(first.End, second.End);

		if (x2 <= x1)
		{
			return default;
		}

		return new Region<T>(x1, x2 - x1);
	}

	/// <summary>
	/// Calculates the union of the <paramref name="first"/> region with <paramref name="second"/> region.
	/// </summary>
	/// <param name="first">The region to union with <paramref name="second"/>.</param>
	/// <param name="second">The region to union with <paramref name="first"/>.</param>
	/// <returns>A new region representing the union.</returns>
	public static Region<T> Union(in Region<T> first, in Region<T> second)
	{
		T x1 = T.Min(first.Origin, second.Origin);
		T x2 = T.Max(first.End, second.End);

		return new Region<T>(x1, x2 - x1);
	}

	/// <summary>
	/// Perform equality comparison.
	/// </summary>
	/// <param name="left">Left parameter.</param>
	/// <param name="right">Right parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	public static bool operator ==(Region<T> left, Region<T> right)
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
	public static bool operator !=(Region<T> left, Region<T> right)
	{
		return left.Origin != right.Origin
			|| left.Size != right.Size;
	}
}