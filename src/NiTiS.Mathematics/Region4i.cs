using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region4i : IEquatable<Region4i>, IFormattable
{
	[DataMember(Order = 0)]
	public Vector4i Origin;

	[DataMember(Order = 1)]
	public Vector4i Size;

	[IgnoreDataMember]
	public readonly Vector4i End => Origin + Size;

	[IgnoreDataMember]
	public readonly Vector4i Center => Origin + (Size / 2);

	public Region4i(Vector4i origin, Vector4i size)
	{
		Origin = origin;
		Size = size;
	}

	public Region4i(Vector4i size)
	{
		Origin = default;
		Size = size;
	}

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region4i reg && Equals(reg);
	public readonly bool Equals(Region4i other) => this == other;
	public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

	public readonly override string ToString() => ToString("G", null);
	public readonly string ToString(string? format) => ToString(format, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

	public readonly bool Contains(Vector4i point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
		                           && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
		                           && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z
		                           && point.W >= Origin.W && point.W < Origin.W + Size.W;
	}

	public readonly bool Intersects(Region4i other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
		                              && Origin.Y < other.End.Y && End.Y > other.Origin.Y
		                              && Origin.Z < other.End.Z && End.Z > other.Origin.Z
		                              && Origin.W < other.End.W && End.W > other.Origin.W;
	}

	public static Region4i Intersection(in Region4i first, in Region4i second)
	{
		int x1 = int.Max(first.Origin.X, second.Origin.X);
		int y1 = int.Max(first.Origin.Y, second.Origin.Y);
		int z1 = int.Max(first.Origin.Z, second.Origin.Z);
		int w1 = int.Max(first.Origin.W, second.Origin.W);
		int x2 = int.Min(first.End.X, second.End.X);
		int y2 = int.Min(first.End.Y, second.End.Y);
		int z2 = int.Min(first.End.Z, second.End.Z);
		int w2 = int.Min(first.End.W, second.End.W);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1 || w2 <= w1)
			return default;

		return new Region4i(new Vector4i(x1, y1, z1, w1), new Vector4i(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	public static Region4i Union(in Region4i first, in Region4i second)
	{
		int x1 = int.Min(first.Origin.X, second.Origin.X);
		int y1 = int.Min(first.Origin.Y, second.Origin.Y);
		int z1 = int.Min(first.Origin.Z, second.Origin.Z);
		int w1 = int.Min(first.Origin.W, second.Origin.W);
		int x2 = int.Max(first.End.X, second.End.X);
		int y2 = int.Max(first.End.Y, second.End.Y);
		int z2 = int.Max(first.End.Z, second.End.Z);
		int w2 = int.Max(first.End.W, second.End.W);

		return new Region4i(new Vector4i(x1, y1, z1, w1), new Vector4i(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	public static bool operator ==(Region4i left, Region4i right) => left.Origin == right.Origin && left.Size == right.Size;
	public static bool operator !=(Region4i left, Region4i right) => left.Origin != right.Origin || left.Size != right.Size;
}