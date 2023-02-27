using System.Numerics;
using System.Runtime.CompilerServices;
using NiTiS.Math.Vectors;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Geometry;

public static class Box
{
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Box<T> CreateInflated<T>(Box<T> square, Vector3d<T> point)
        where T : unmanaged, INumber<T>
        => new(Vector3d.Min(square.Min, point), Vector3d.Max(square.Max, point));

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T GetDistanceToNearestEdge<T>(Box<T> box, Vector3d<T> point)
        where T : unmanaged, INumber<T>, IRootFunctions<T>
        => T.Sqrt(GetSquaredDistanceToNearestEdge(box, point));
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T GetSquaredDistanceToNearestEdge<T>(Box<T> box, Vector3d<T> point)
        where T : unmanaged, INumber<T>, IRootFunctions<T>
    {
        T dx = T.Max(T.Max(box.Min.X - point.X, T.Zero), point.X - box.Max.X);
        T dy = T.Max(T.Max(box.Min.Y - point.Y, T.Zero), point.Y - box.Max.Y);
        T dz = T.Max(T.Max(box.Min.Z - point.Z, T.Zero), point.Z - box.Max.Y);
        return dx * dx + dy * dy + dz * dz;
    }
}