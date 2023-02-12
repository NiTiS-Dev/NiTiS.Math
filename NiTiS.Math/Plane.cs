using NiTiS.Core.Annotations;
using System;
using System.Numerics;

namespace NiTiS.Math;

[NotImplementYet]
[Obsolete(nameof(NotImplementYetAttribute))]
public unsafe struct Plane<T>
	where T :
	unmanaged,
	INumberBase<T>
{
	public Vector3D<T> Normal;
	public T D;
}