using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region3u : IEquatable<Region3u>, IFormattable
{
	[DataMember(Order = 0)]
	public Vector3u Origin;
    
	[DataMember(Order = 1)]
	public Vector3u Size;

	[IgnoreDataMember]
	public readonly Vector3u End => Origin + Size;

	[IgnoreDataMember]
	public readonly Vector3u Center => Origin + (Size / 2);

	public Region3u(Vector3u origin, Vector3u size)
	{
		Origin = origin;
		Size = size;
	}

	public Region3u(Vector3u size)
	{
		Origin = default;
		Size = size;
	}

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region3u reg && Equals(reg);
	public readonly bool Equals(Region3u other) => this == other;
	public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

	public readonly override string ToString() => ToString("G", null);
	public readonly string ToString(string? format) => ToString(format, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

	public readonly bool Contains(Vector3u point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
		                           && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
		                           && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z;
	}

	public readonly bool Intersects(Region3u other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
		                              && Origin.Y < other.End.Y && End.Y > other.Origin.Y
		                              && Origin.Z < other.End.Z && End.Z > other.Origin.Z;
	}

	public static Region3u Intersection(in Region3u first, in Region3u second)
	{
		uint x1 = uint.Max(first.Origin.X, second.Origin.X);
		uint y1 = uint.Max(first.Origin.Y, second.Origin.Y);
		uint z1 = uint.Max(first.Origin.Z, second.Origin.Z);
		uint x2 = uint.Min(first.End.X, second.End.X);
		uint y2 = uint.Min(first.End.Y, second.End.Y);
		uint z2 = uint.Min(first.End.Z, second.End.Z);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1)
			return default;

		return new Region3u(new Vector3u(x1, y1, z1), new Vector3u(x2 - x1, y2 - y1, z2 - z1));
	}

	public static Region3u Union(in Region3u first, in Region3u second)
	{
		uint x1 = uint.Min(first.Origin.X, second.Origin.X);
		uint y1 = uint.Min(first.Origin.Y, second.Origin.Y);
		uint z1 = uint.Min(first.Origin.Z, second.Origin.Z);
		uint x2 = uint.Max(first.End.X, second.End.X);
		uint y2 = uint.Max(first.End.Y, second.End.Y);
		uint z2 = uint.Max(first.End.Z, second.End.Z);

		return new Region3u(new Vector3u(x1, y1, z1), new Vector3u(x2 - x1, y2 - y1, z2 - z1));
	}

	public static bool operator ==(Region3u left, Region3u right) => left.Origin == right.Origin && left.Size == right.Size;
	public static bool operator !=(Region3u left, Region3u right) => left.Origin != right.Origin || left.Size != right.Size;
}