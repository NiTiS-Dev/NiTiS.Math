using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region4 : IEquatable<Region4>, IFormattable
{
    [DataMember(Order = 0)]
    public Vector4 Origin;

    [DataMember(Order = 1)]
    public Vector4 Size;

    [IgnoreDataMember]
    public readonly Vector4 End => Origin + Size;

    [IgnoreDataMember]
    public readonly Vector4 Center => Origin + (Size / 2);

    public Region4(Vector4 origin, Vector4 size)
    {
        Origin = origin;
        Size = size;
    }

    public Region4(Vector4 size)
    {
        Origin = default;
        Size = size;
    }

    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region4 reg && Equals(reg);
    public readonly bool Equals(Region4 other) => this == other;
    public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

    public readonly override string ToString() => ToString("G", null);
    public readonly string ToString(string? format) => ToString(format, null);
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

    public readonly bool Contains(Vector4 point)
    {
        return point.X >= Origin.X && point.X < Origin.X + Size.X
            && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
            && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z
            && point.W >= Origin.W && point.W < Origin.W + Size.W;
    }

    public readonly bool Intersects(Region4 other)
    {
        return Origin.X < other.End.X && End.X > other.Origin.X
            && Origin.Y < other.End.Y && End.Y > other.Origin.Y
            && Origin.Z < other.End.Z && End.Z > other.Origin.Z
            && Origin.W < other.End.W && End.W > other.Origin.W;
    }

    public static Region4 Intersection(in Region4 first, in Region4 second)
    {
        float x1 = float.Max(first.Origin.X, second.Origin.X);
        float y1 = float.Max(first.Origin.Y, second.Origin.Y);
        float z1 = float.Max(first.Origin.Z, second.Origin.Z);
        float w1 = float.Max(first.Origin.W, second.Origin.W);
        float x2 = float.Min(first.End.X, second.End.X);
        float y2 = float.Min(first.End.Y, second.End.Y);
        float z2 = float.Min(first.End.Z, second.End.Z);
        float w2 = float.Min(first.End.W, second.End.W);

        if (x2 <= x1 || y2 <= y1 || z2 <= z1 || w2 <= w1)
            return default;

        return new Region4(new Vector4(x1, y1, z1, w1), new Vector4(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
    }

    public static Region4 Union(in Region4 first, in Region4 second)
    {
        float x1 = float.Min(first.Origin.X, second.Origin.X);
        float y1 = float.Min(first.Origin.Y, second.Origin.Y);
        float z1 = float.Min(first.Origin.Z, second.Origin.Z);
        float w1 = float.Min(first.Origin.W, second.Origin.W);
        float x2 = float.Max(first.End.X, second.End.X);
        float y2 = float.Max(first.End.Y, second.End.Y);
        float z2 = float.Max(first.End.Z, second.End.Z);
        float w2 = float.Max(first.End.W, second.End.W);

        return new Region4(new Vector4(x1, y1, z1, w1), new Vector4(x2 - x1, y2 - y1, z2 - z1, w2 - w1));
    }

    public static bool operator ==(Region4 left, Region4 right) => left.Origin == right.Origin && left.Size == right.Size;
    public static bool operator !=(Region4 left, Region4 right) => left.Origin != right.Origin || left.Size != right.Size;
}