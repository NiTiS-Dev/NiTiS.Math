using NiTiS.Core.Annotations;
using System;
using System.Numerics;

namespace NiTiS.Math.Geometry;

public static class Plane
{
    [Obsolete(nameof(NotImplementYetAttribute))]
    [NotImplementYet]
    public static Plane<T> Normalize<T>(Plane<T> plane)
        where T : unmanaged, INumberBase<T>
    {
        return plane;
    }
}