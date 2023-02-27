using NiTiS.Core.Annotations;
using NiTiS.Math.Vectors;
using System;
using System.Numerics;

namespace NiTiS.Math.Geometry;

[NotImplementYet]
[Obsolete(nameof(NotImplementYetAttribute))]
public unsafe struct Plane<T>
    where T :
    unmanaged,
    INumberBase<T>
{
    public Vector3d<T> Normal;
    public T D;
}