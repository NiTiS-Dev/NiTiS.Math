using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS.Math.Geometry;

/// <summary>
/// Third-dimension region with origin point and size.
/// </summary>
/// <typeparam name="N">Region type.</typeparam>
[DebuggerDisplay("Size = {Size}")]
public unsafe struct Region3d<N>
	where N : unmanaged, INumberBase<N>
{
	/// <summary>
	/// Origin point of region.
	/// </summary>
	public Vector3d<N> Origin;
	/// <summary>
	/// Size of region.
	/// </summary>
	public Vector3d<N> Size;

	/// <summary>
	/// Creates new region with <paramref name="origin"/> point and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region3d(Vector3d<N> origin, Vector3d<N> size)
	{
		Origin = origin;
		Size = size;
	}

	/// <summary>
	/// Creates new region with <paramref name="origin"/> point and size(<paramref name="sizeX"/>, <paramref name="sizeY"/>).
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="sizeX">Region size X.</param>
	/// <param name="sizeY">Region size Y.</param>
	/// <param name="sizeZ">Region size Z.</param>
	public Region3d(Vector3d<N> origin, N sizeX, N sizeY, N sizeZ)
	{
		Origin = origin;
		Size = new(sizeX, sizeY, sizeZ);
	}

	/// <summary>
	/// Creates new region with origin point(<paramref name="originX"/>, <paramref name="originY"/>) and size(<paramref name="sizeX"/>, <paramref name="sizeY"/>).
	/// </summary>
	/// <param name="originX">Region origin point X.</param>
	/// <param name="originY">Region origin point Y.</param>
	/// <param name="originZ">Region origin point Z.</param>
	/// <param name="sizeX">Region size X.</param>
	/// <param name="sizeY">Region size Y.</param>
	/// <param name="sizeZ">Region size Z.</param>
	public Region3d(N originX, N originY, N originZ, N sizeX, N sizeY, N sizeZ)
	{
		Origin = new(originX, originY, originZ);
		Size = new(sizeX, sizeY, sizeZ);
	}

	/// <summary>
	/// Creates new region with origin point(<paramref name="originX"/>, <paramref name="originY"/>) and <paramref name="size"/>.
	/// </summary>
	/// <param name="originX">Region origin point X.</param>
	/// <param name="originY">Region origin point Y.</param>
	/// <param name="originZ">Region origin point Z.</param>
	/// <param name="size">Region size.</param>
	public Region3d(N originX, N originY, N originZ, Vector3d<N> size)
	{
		Origin = new(originX, originY, originZ);
		Size = size;
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region3d(ReadOnlySpan<N> data)
	{
		if (data.Length < 6)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region3d<N>>(ref Unsafe.As<N, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region3d(ReadOnlySpan<byte> data)
	{
		if (data.Length < 6 * sizeof(N))
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region3d<N>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Region width.
	/// </summary>
	public N Width =>
		Size.X;

	/// <summary>
	/// Region height.
	/// </summary>
	public N Height =>
		Size.Y;

	/// <summary>
	/// Region depth.
	/// </summary>
	public N Depth =>
		Size.Z;

	/// <summary>
	/// Center of region.
	/// </summary>
	public Vector3d<N> Center
		=> Origin + HalfSize;

	/// <summary>
	/// Half size of region.
	/// </summary>
	public Vector3d<N> HalfSize
		=> Size / Scalar<N>.Two;

	/// <summary>
	/// Point where region is ends.
	/// </summary>
	public Vector3d<N> End
		=> Origin + Size;
}