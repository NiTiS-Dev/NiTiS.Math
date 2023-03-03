using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS.Math.Geometry;

/// <summary>
/// Third-dimension region with origin point and size.
/// </summary>
/// <typeparam name="T">Region type.</typeparam>
[DebuggerDisplay("Size = {Size}")]
public unsafe struct Region3d<T>
	where T : unmanaged, INumberBase<T>
{
	/// <summary>
	/// Origin point of region.
	/// </summary>
	public Vector3d<T> Origin;
	/// <summary>
	/// Size of region.
	/// </summary>
	public Vector3d<T> Size;

	/// <summary>
	/// Creates new region with <paramref name="origin"/> point and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region3d(Vector3d<T> origin, Vector3d<T> size)
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
	public Region3d(Vector3d<T> origin, T sizeX, T sizeY, T sizeZ)
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
	public Region3d(T originX, T originY, T originZ, T sizeX, T sizeY, T sizeZ)
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
	public Region3d(T originX, T originY, T originZ, Vector3d<T> size)
	{
		Origin = new(originX, originY, originZ);
		Size = size;
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region3d(ReadOnlySpan<T> data)
	{
		if (data.Length < 6)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region3d<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region3d(ReadOnlySpan<byte> data)
	{
		if (data.Length < 6 * sizeof(T))
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region3d<T>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Region width.
	/// </summary>
	public T Width =>
		Size.X;

	/// <summary>
	/// Region height.
	/// </summary>
	public T Height =>
		Size.Y;

	/// <summary>
	/// Region depth.
	/// </summary>
	public T Depth =>
		Size.Z;

	/// <summary>
	/// Point where region is ends.
	/// </summary>
	public Vector3d<T> End
		=> Origin + Size;
}