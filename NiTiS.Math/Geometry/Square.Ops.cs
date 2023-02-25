using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Geometry;

public static class Square
{
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static Square<T> CreateInflated<T>(Square<T> square, Vector2d<T> point)
        where T : unmanaged, INumber<T>
        => new(Vector2d.Min(square.Min, point), Vector2d.Max(square.Max, point));

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T GetDistanceToNearestEdge<T>(Square<T> box, Vector2d<T> point)
        where T : unmanaged, INumber<T>, IRootFunctions<T>
        => T.Sqrt(GetSquaredDistanceToNearestEdge(box, point));
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    public static T GetSquaredDistanceToNearestEdge<T>(Square<T> box, Vector2d<T> point)
        where T : unmanaged, INumber<T>, IRootFunctions<T>
    {
        T dx = T.Max(T.Max(box.Min.X - point.X, T.Zero), point.X - box.Max.X);
        T dy = T.Max(T.Max(box.Min.Y - point.Y, T.Zero), point.Y - box.Max.Y);
        return dx * dx + dy * dy;
    }
}
