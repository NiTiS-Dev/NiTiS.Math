using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

/// <summary>
/// Represent 4-dimensional region.
/// </summary>
[DataContract]
public struct Region4<T> : IEquatable<Region4<T>>, IFormattable
	where T : unmanaged, INumber<T>
{
	/// <summary>
	/// Region origin.
	/// </summary>
	[DataMember(Order = 0)]
	public Vector4<T> Origin;

	/// <summary>
	/// Size of the region.
	/// </summary>
	[DataMember(Order = 1)]
	public Vector4<T> Size;

	/// <summary>
	/// End of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly Vector4<T> End => Origin + Size;

	/// <summary>
	/// Center of the region.
	/// </summary>
	[IgnoreDataMember]
	public readonly Vector4<T> Center => Origin + (Size / (T.One + T.One));

	/// <summary>
	/// Creates new region with specified <paramref name="origin"/> and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region4(Vector4<T> origin, Vector4<T> size)
	{
		Origin = origin;
		Size = size;
	}

	/// <summary>
	/// Creates new region from zero point with specified <paramref name="size"/>.
	/// </summary>
	/// <param name="size">Region size.</param>
	public Region4(Vector4<T> size)
	{
		Origin = default;
		Size = size;
	}

	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Region4<T> reg && Equals(reg);
	}

	/// <inheritdoc/>
	public readonly bool Equals(Region4<T> other)
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
	public readonly bool Contains(Vector4<T> point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
			&& point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y;
	}

	/// <summary>
	/// Checks if the region intersects with another region.
	/// </summary>
	/// <param name="other">The region to check.</param>
	/// <returns><c>true</c> if the regions intersect; otherwise, <c>false</c>.</returns>
	public readonly bool Intersects(Region4<T> other)
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
	public static Region4<T> Intersection(in Region4<T> first, in Region4<T> second)
	{
		T x1 = T.Max(first.Origin.X, second.Origin.X);
		T y1 = T.Max(first.Origin.Y, second.Origin.Y);
		T z1 = T.Max(first.Origin.Z, second.Origin.Z);
		T w1 = T.Max(first.Origin.W, second.Origin.W);
		T x2 = T.Min(first.End.X, second.End.X);
		T y2 = T.Min(first.End.Y, second.End.Y);
		T z2 = T.Min(first.End.Z, second.End.Z);
		T w2 = T.Max(first.End.W, second.End.W);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1)
		{
			return default;
		}

		return new Region4<T>(new Vector4<T>(x1, y1, z1, w1), new Vector4<T>(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	/// <summary>
	/// Calculates the union of the <paramref name="first"/> region with <paramref name="second"/> region.
	/// </summary>
	/// <param name="first">The region to union with <paramref name="second"/>.</param>
	/// <param name="second">The region to union with <paramref name="first"/>.</param>
	/// <returns>A new region representing the union.</returns>
	public static Region4<T> Union(in Region4<T> first, in Region4<T> second)
	{
		T x1 = T.Min(first.Origin.X, second.Origin.X);
		T y1 = T.Min(first.Origin.Y, second.Origin.Y);
		T z1 = T.Min(first.Origin.Z, second.Origin.Z);
		T w1 = T.Min(first.Origin.W, second.Origin.W);
		T x2 = T.Max(first.End.X, second.End.X);
		T y2 = T.Max(first.End.Y, second.End.Y);
		T z2 = T.Max(first.End.Z, second.End.Z);
		T w2 = T.Max(first.End.W, second.End.W);

		return new Region4<T>(new Vector4<T>(x1, y1, z1, w1), new Vector4<T>(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	/// <summary>
	/// Perform equality comparison.
	/// </summary>
	/// <param name="left">Left parameter.</param>
	/// <param name="right">Right parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	public static bool operator ==(Region4<T> left, Region4<T> right)
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
	public static bool operator !=(Region4<T> left, Region4<T> right)
	{
		return left.Origin != right.Origin
			|| left.Size != right.Size;
	}
}