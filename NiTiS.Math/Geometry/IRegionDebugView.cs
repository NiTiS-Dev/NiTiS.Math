using System.Diagnostics;
using System.Numerics;
using NiTiS.Math.Vectors;

namespace NiTiS.Math.Geometry;

internal sealed class IRegionDebugView<T>
	where T : unmanaged, INumberBase<T>
{
	public Vector4d<T> Begin;
	public Vector4d<T> Size;
	public Vector4d<T> End;

	public IRegionDebugView(object obj)
	{
		if (obj is null)
			return;

		if (obj is Region2d<T> region2d)
		{
			Begin = new(region2d.Origin, default, default);
			Size = new(region2d.Size, default, default);
			End = new(region2d.End, default, default);
		}
	}
}