using NiTiS.Core.Annotations;
using System;
using System.Numerics;

namespace NiTiS.Math;

/// <summary>
/// <b>Not implement yet</b>
/// </summary>
/// <typeparam name="T"></typeparam>
[NotImplementYet]
[Obsolete(nameof(NotImplementYetAttribute))]
public struct Quaternion<T>
	where T :
		unmanaged,
		INumberBase<T>
{
	public T X, Y, Z, W;
}