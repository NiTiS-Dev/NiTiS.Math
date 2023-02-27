using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using NiTiS.Math.Vectors;

namespace NiTiS.Math.Geometry;


public readonly struct Box<T> :
	IEquatable<Box<T>>,
	IEqualityOperators<Box<T>, Box<T>, bool>
	where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>
{
	public readonly Vector3d<T> Min;
	public readonly Vector3d<T> Max;
	public readonly Vector3d<T> Size => Max - Min;
	public readonly Vector3d<T> Center => (Min + Max) / Scalar<T>.Two;

	public Box(Vector3d<T> min, Vector3d<T> max)
		=> (Min, Max) = (min, max);
	public Box(Vector3d<T> min, T maxX, T maxY, T maxZ)
		=> (Min, Max) = (min, new(maxX, maxY, maxZ));
	public Box(T minX, T minY, T minZ, T maxX, T maxY, T maxZ)
		=> (Min, Max) = (new(minX, minY, minZ), new(maxX, maxY, maxZ));
	public Box(T minX, T minY, T minZ, Vector3d<T> max)
		=> (Min, Max) = (new(minX, minY, minZ), max);

	public readonly bool Equals(Box<T> other)
		=> this == other;
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Square<T> sqr ? Equals(sqr) : false;
	public override int GetHashCode()
		=> HashCode.Combine(Min, Max);
	public readonly Box<T> GetScaled(Vector3d<T> scale, Vector3d<T> anchor)
		=> new(
			scale * (Min - anchor) + anchor,
			scale * (Max - anchor) + anchor
			);
	public readonly bool Contains(Vector3d<T> point)
		=> point.X >= Min.X && point.Y >= Min.Y && point.Z >= Min.Z
		&& point.X <= Max.X && point.Y <= Min.Y && point.Z <= Max.Z;
	public readonly bool Contains(Box<T> point)
		=> point.Min.X >= Min.X && point.Min.Y >= Min.Y && point.Min.Z >= Min.Z
		&& point.Max.X <= Max.X && point.Max.Y <= Min.Y && point.Max.Z <= Max.Z;

	public readonly Box<T> GetTranslated(Vector3d<T> distance)
		=> new(Min + distance, Max + distance);

	public static bool operator ==(Box<T> left, Box<T> right)
		=> left.Min == right.Min
		&& left.Max == right.Max;
	public static bool operator !=(Box<T> left, Box<T> right)
		=> left.Min != right.Min
		|| left.Max != right.Max;
}
