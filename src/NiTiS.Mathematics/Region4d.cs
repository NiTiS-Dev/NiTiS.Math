using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region4d : IEquatable<Region4d>, IFormattable
{
	[DataMember(Order = 0)]
	public Vector4d Origin;

	[DataMember(Order = 1)]
	public Vector4d Size;

	[IgnoreDataMember]
	public readonly Vector4d End => Origin + Size;

	[IgnoreDataMember]
	public readonly Vector4d Center => Origin + (Size / 2);

	public Region4d(Vector4d origin, Vector4d size)
	{
		Origin = origin;
		Size = size;
	}

	public Region4d(Vector4d size)
	{
		Origin = default;
		Size = size;
	}

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region4d reg && Equals(reg);
	public readonly bool Equals(Region4d other) => this == other;
	public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

	public readonly override string ToString() => ToString("G", null);
	public readonly string ToString(string? format) => ToString(format, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

	public readonly bool Contains(Vector4d point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
		                           && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
		                           && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z
		                           && point.W >= Origin.W && point.W < Origin.W + Size.W;
	}

	public readonly bool Intersects(Region4d other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
		                              && Origin.Y < other.End.Y && End.Y > other.Origin.Y
		                              && Origin.Z < other.End.Z && End.Z > other.Origin.Z
		                              && Origin.W < other.End.W && End.W > other.Origin.W;
	}

	public static Region4d Intersection(in Region4d first, in Region4d second)
	{
		double x1 = double.Max(first.Origin.X, second.Origin.X);
		double y1 = double.Max(first.Origin.Y, second.Origin.Y);
		double z1 = double.Max(first.Origin.Z, second.Origin.Z);
		double w1 = double.Max(first.Origin.W, second.Origin.W);
		double x2 = double.Min(first.End.X, second.End.X);
		double y2 = double.Min(first.End.Y, second.End.Y);
		double z2 = double.Min(first.End.Z, second.End.Z);
		double w2 = double.Min(first.End.W, second.End.W);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1 || w2 <= w1)
			return default;

		return new Region4d(new Vector4d(x1, y1, z1, w1), new Vector4d(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	public static Region4d Union(in Region4d first, in Region4d second)
	{
		double x1 = double.Min(first.Origin.X, second.Origin.X);
		double y1 = double.Min(first.Origin.Y, second.Origin.Y);
		double z1 = double.Min(first.Origin.Z, second.Origin.Z);
		double w1 = double.Min(first.Origin.W, second.Origin.W);
		double x2 = double.Max(first.End.X, second.End.X);
		double y2 = double.Max(first.End.Y, second.End.Y);
		double z2 = double.Max(first.End.Z, second.End.Z);
		double w2 = double.Max(first.End.W, second.End.W);

		return new Region4d(new Vector4d(x1, y1, z1, w1), new Vector4d(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	public static bool operator ==(Region4d left, Region4d right) => left.Origin == right.Origin && left.Size == right.Size;
	public static bool operator !=(Region4d left, Region4d right) => left.Origin != right.Origin || left.Size != right.Size;
}