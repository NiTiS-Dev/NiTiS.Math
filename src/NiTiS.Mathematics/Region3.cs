using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

[DataContract]
public struct Region3 : IEquatable<Region3>, IFormattable
{
    [DataMember(Order = 0)]
    public Vector3 Origin;

    [DataMember(Order = 1)]
    public Vector3 Size;

    [IgnoreDataMember]
    public readonly Vector3 End => Origin + Size;

    [IgnoreDataMember]
    public readonly Vector3 Center => Origin + (Size / 2);

    public Region3(Vector3 origin, Vector3 size)
    {
        Origin = origin;
        Size = size;
    }

    public Region3(Vector3 size)
    {
        Origin = default;
        Size = size;
    }

    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Region3 reg && Equals(reg);
    public readonly bool Equals(Region3 other) => this == other;
    public readonly override int GetHashCode() => HashCode.Combine(Origin, Size);

    public readonly override string ToString() => ToString("G", null);
    public readonly string ToString(string? format) => ToString(format, null);
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => $"{{Origin: {Origin}, Size: {Size}}}";

    public readonly bool Contains(Vector3 point)
    {
        return point.X >= Origin.X && point.X < Origin.X + Size.X
            && point.Y >= Origin.Y && point.Y < Origin.Y + Size.Y
            && point.Z >= Origin.Z && point.Z < Origin.Z + Size.Z;
    }

    public readonly bool Intersects(Region3 other)
    {
        return Origin.X < other.End.X && End.X > other.Origin.X
            && Origin.Y < other.End.Y && End.Y > other.Origin.Y
            && Origin.Z < other.End.Z && End.Z > other.Origin.Z;
    }

    public static Region3 Intersection(in Region3 first, in Region3 second)
    {
        float x1 = float.Max(first.Origin.X, second.Origin.X);
        float y1 = float.Max(first.Origin.Y, second.Origin.Y);
        float z1 = float.Max(first.Origin.Z, second.Origin.Z);
        float x2 = float.Min(first.End.X, second.End.X);
        float y2 = float.Min(first.End.Y, second.End.Y);
        float z2 = float.Min(first.End.Z, second.End.Z);

        if (x2 <= x1 || y2 <= y1 || z2 <= z1)
            return default;

        return new Region3(new Vector3(x1, y1, z1), new Vector3(x2 - x1, y2 - y1, z2 - z1));
    }

    public static Region3 Union(in Region3 first, in Region3 second)
    {
        float x1 = float.Min(first.Origin.X, second.Origin.X);
        float y1 = float.Min(first.Origin.Y, second.Origin.Y);
        float z1 = float.Min(first.Origin.Z, second.Origin.Z);
        float x2 = float.Max(first.End.X, second.End.X);
        float y2 = float.Max(first.End.Y, second.End.Y);
        float z2 = float.Max(first.End.Z, second.End.Z);

        return new Region3(new Vector3(x1, y1, z1), new Vector3(x2 - x1, y2 - y1, z2 - z1));
    }

    public static bool operator ==(Region3 left, Region3 right) => left.Origin == right.Origin && left.Size == right.Size;
    public static bool operator !=(Region3 left, Region3 right) => left.Origin != right.Origin || left.Size != right.Size;
}