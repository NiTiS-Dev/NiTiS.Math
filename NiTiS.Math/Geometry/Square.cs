using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using NiTiS.Math.Vectors;

namespace NiTiS.Math.Geometry;

public readonly struct Square<T> :
    IEquatable<Square<T>>,
    IEqualityOperators<Square<T>, Square<T>, bool>
    where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>
{
    public readonly Vector2d<T> Min;
    public readonly Vector2d<T> Max;
    public readonly Vector2d<T> Size => Max - Min;
    public readonly Vector2d<T> Center => (Min + Max) / Scalar<T>.Two;

    public Square(Vector2d<T> min, Vector2d<T> max)
        => (Min, Max) = (min, max);
    public Square(Vector2d<T> min, T maxX, T maxY)
        => (Min, Max) = (min, new(maxX, maxY));
    public Square(T minX, T minY, T maxX, T maxY)
        => (Min, Max) = (new(minX, minY), new(maxX, maxY));
    public Square(T minX, T minY, Vector2d<T> max)
        => (Min, Max) = (new(minX, minY), max);

    public readonly bool Equals(Square<T> other)
        => this == other;
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is Square<T> sqr ? Equals(sqr) : false;
    public override int GetHashCode()
        => HashCode.Combine(Min, Max);
    public readonly Square<T> GetScaled(Vector2d<T> scale, Vector2d<T> anchor)
        => new(
            scale * (Min - anchor) + anchor,
            scale * (Max - anchor) + anchor
            );
    public readonly bool Contains(Vector2d<T> point)
        => point.X >= Min.X && point.Y >= Min.Y
        && point.X <= Max.X && point.Y <= Min.Y;
    public readonly bool Contains(Square<T> point)
        => point.Min.X >= Min.X && point.Min.Y >= Min.Y
        && point.Max.X <= Max.X && point.Max.Y <= Min.Y;
    public readonly Square<T> GetTranslated(Vector2d<T> distance)
        => new(Min + distance, Max + distance);

    public static bool operator ==(Square<T> left, Square<T> right)
        => left.Min == right.Min
        && left.Max == right.Max;
    public static bool operator !=(Square<T> left, Square<T> right)
        => left.Min != right.Min
        || left.Max != right.Max;
}