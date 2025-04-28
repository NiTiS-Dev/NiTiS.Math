using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region3d : IEquatable<Region3d>, IFormattable
{
	[DataMember(Order = 0)]
	public Vector3d Origin;
    
	[DataMember(Order = 1)]
	public Vector3d Size;

	[IgnoreDataMember]
	public readonly Vector3d End => Origin + Size;

	[IgnoreDataMember]
	public readonly Vector3d Center => Origin + (Size / 2);

	public Region3d(Vector3d origin, Vector3d size)
	{
		Origin = origin;
		Size = size;
	}

	public Region3d(Vector3d size)
	{
		Origin = default;
		Size = size;
	}

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region3d reg && Equals(reg);
	public readonly bool Equals(Region3d other) => this == other;
	public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

	public readonly override string ToString() => ToString("G", null);
	public readonly string ToString(string? format) => ToString(format, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

	public readonly bool Contains(Vector3d point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
		                           && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
		                           && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z;
	}

	public readonly bool Intersects(Region3d other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
		                              && Origin.Y < other.End.Y && End.Y > other.Origin.Y
		                              && Origin.Z < other.End.Z && End.Z > other.Origin.Z;
	}

	public static Region3d Intersection(in Region3d first, in Region3d second)
	{
		double x1 = double.Max(first.Origin.X, second.Origin.X);
		double y1 = double.Max(first.Origin.Y, second.Origin.Y);
		double z1 = double.Max(first.Origin.Z, second.Origin.Z);
		double x2 = double.Min(first.End.X, second.End.X);
		double y2 = double.Min(first.End.Y, second.End.Y);
		double z2 = double.Min(first.End.Z, second.End.Z);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1)
			return default;

		return new Region3d(new Vector3d(x1, y1, z1), new Vector3d(x2 - x1, y2 - y1, z2 - z1));
	}

	public static Region3d Union(in Region3d first, in Region3d second)
	{
		double x1 = double.Min(first.Origin.X, second.Origin.X);
		double y1 = double.Min(first.Origin.Y, second.Origin.Y);
		double z1 = double.Min(first.Origin.Z, second.Origin.Z);
		double x2 = double.Max(first.End.X, second.End.X);
		double y2 = double.Max(first.End.Y, second.End.Y);
		double z2 = double.Max(first.End.Z, second.End.Z);

		return new Region3d(new Vector3d(x1, y1, z1), new Vector3d(x2 - x1, y2 - y1, z2 - z1));
	}

	public static bool operator ==(Region3d left, Region3d right) => left.Origin == right.Origin && left.Size == right.Size;
	public static bool operator !=(Region3d left, Region3d right) => left.Origin != right.Origin || left.Size != right.Size;
}