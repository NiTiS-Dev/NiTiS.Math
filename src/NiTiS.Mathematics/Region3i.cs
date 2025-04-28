using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region3i : IEquatable<Region3i>, IFormattable
{
	[DataMember(Order = 0)]
	public Vector3i Origin;
    
	[DataMember(Order = 1)]
	public Vector3i Size;

	[IgnoreDataMember]
	public readonly Vector3i End => Origin + Size;

	[IgnoreDataMember]
	public readonly Vector3i Center => Origin + (Size / 2);

	public Region3i(Vector3i origin, Vector3i size)
	{
		Origin = origin;
		Size = size;
	}

	public Region3i(Vector3i size)
	{
		Origin = default;
		Size = size;
	}

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region3i reg && Equals(reg);
	public readonly bool Equals(Region3i other) => this == other;
	public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

	public readonly override string ToString() => ToString("G", null);
	public readonly string ToString(string? format) => ToString(format, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

	public readonly bool Contains(Vector3i point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
		                           && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
		                           && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z;
	}

	public readonly bool Intersects(Region3i other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
		                              && Origin.Y < other.End.Y && End.Y > other.Origin.Y
		                              && Origin.Z < other.End.Z && End.Z > other.Origin.Z;
	}

	public static Region3i Intersection(in Region3i first, in Region3i second)
	{
		int x1 = int.Max(first.Origin.X, second.Origin.X);
		int y1 = int.Max(first.Origin.Y, second.Origin.Y);
		int z1 = int.Max(first.Origin.Z, second.Origin.Z);
		int x2 = int.Min(first.End.X, second.End.X);
		int y2 = int.Min(first.End.Y, second.End.Y);
		int z2 = int.Min(first.End.Z, second.End.Z);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1)
			return default;

		return new Region3i(new Vector3i(x1, y1, z1), new Vector3i(x2 - x1, y2 - y1, z2 - z1));
	}

	public static Region3i Union(in Region3i first, in Region3i second)
	{
		int x1 = int.Min(first.Origin.X, second.Origin.X);
		int y1 = int.Min(first.Origin.Y, second.Origin.Y);
		int z1 = int.Min(first.Origin.Z, second.Origin.Z);
		int x2 = int.Max(first.End.X, second.End.X);
		int y2 = int.Max(first.End.Y, second.End.Y);
		int z2 = int.Max(first.End.Z, second.End.Z);

		return new Region3i(new Vector3i(x1, y1, z1), new Vector3i(x2 - x1, y2 - y1, z2 - z1));
	}

	public static bool operator ==(Region3i left, Region3i right) => left.Origin == right.Origin && left.Size == right.Size;
	public static bool operator !=(Region3i left, Region3i right) => left.Origin != right.Origin || left.Size != right.Size;
}