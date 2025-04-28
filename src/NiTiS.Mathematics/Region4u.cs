using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region4u : IEquatable<Region4u>, IFormattable
{
	[DataMember(Order = 0)]
	public Vector4u Origin;

	[DataMember(Order = 1)]
	public Vector4u Size;

	[IgnoreDataMember]
	public readonly Vector4u End => Origin + Size;

	[IgnoreDataMember]
	public readonly Vector4u Center => Origin + (Size / 2);

	public Region4u(Vector4u origin, Vector4u size)
	{
		Origin = origin;
		Size = size;
	}

	public Region4u(Vector4u size)
	{
		Origin = default;
		Size = size;
	}

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region4u reg && Equals(reg);
	public readonly bool Equals(Region4u other) => this == other;
	public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

	public readonly override string ToString() => ToString("G", null);
	public readonly string ToString(string? format) => ToString(format, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

	public readonly bool Contains(Vector4u point)
	{
		return point.X >= Origin.X && point.X < Origin.X + Size.X
		                           && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
		                           && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z
		                           && point.W >= Origin.W && point.W < Origin.W + Size.W;
	}

	public readonly bool Intersects(Region4u other)
	{
		return Origin.X < other.End.X && End.X > other.Origin.X
		                              && Origin.Y < other.End.Y && End.Y > other.Origin.Y
		                              && Origin.Z < other.End.Z && End.Z > other.Origin.Z
		                              && Origin.W < other.End.W && End.W > other.Origin.W;
	}

	public static Region4u Intersection(in Region4u first, in Region4u second)
	{
		uint x1 = uint.Max(first.Origin.X, second.Origin.X);
		uint y1 = uint.Max(first.Origin.Y, second.Origin.Y);
		uint z1 = uint.Max(first.Origin.Z, second.Origin.Z);
		uint w1 = uint.Max(first.Origin.W, second.Origin.W);
		uint x2 = uint.Min(first.End.X, second.End.X);
		uint y2 = uint.Min(first.End.Y, second.End.Y);
		uint z2 = uint.Min(first.End.Z, second.End.Z);
		uint w2 = uint.Min(first.End.W, second.End.W);

		if (x2 <= x1 || y2 <= y1 || z2 <= z1 || w2 <= w1)
			return default;

		return new Region4u(new Vector4u(x1, y1, z1, w1), new Vector4u(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	public static Region4u Union(in Region4u first, in Region4u second)
	{
		uint x1 = uint.Min(first.Origin.X, second.Origin.X);
		uint y1 = uint.Min(first.Origin.Y, second.Origin.Y);
		uint z1 = uint.Min(first.Origin.Z, second.Origin.Z);
		uint w1 = uint.Min(first.Origin.W, second.Origin.W);
		uint x2 = uint.Max(first.End.X, second.End.X);
		uint y2 = uint.Max(first.End.Y, second.End.Y);
		uint z2 = uint.Max(first.End.Z, second.End.Z);
		uint w2 = uint.Max(first.End.W, second.End.W);

		return new Region4u(new Vector4u(x1, y1, z1, w1), new Vector4u(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
	}

	public static bool operator ==(Region4u left, Region4u right) => left.Origin == right.Origin && left.Size == right.Size;
	public static bool operator !=(Region4u left, Region4u right) => left.Origin != right.Origin || left.Size != right.Size;
}