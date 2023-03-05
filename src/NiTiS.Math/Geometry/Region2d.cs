using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS.Math.Geometry;

/// <summary>
/// Two-dimension region with origin point and size.
/// </summary>
/// <typeparam name="N">Region type.</typeparam>
[DebuggerDisplay("Size = {Size}")]
public unsafe struct Region2d<N>
	where N : unmanaged, INumberBase<N>
{
	/// <summary>
	/// Origin point of region.
	/// </summary>
	public Vector2d<N> Origin;
	/// <summary>
	/// Size of region.
	/// </summary>
	public Vector2d<N> Size;

	/// <summary>
	/// Creates new region with <paramref name="origin"/> point and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region2d(Vector2d<N> origin, Vector2d<N> size)
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
	public Region2d(Vector2d<N> origin, N sizeX, N sizeY)
	{
		Origin = origin;
		Size = new(sizeX, sizeY);
	}

	/// <summary>
	/// Creates new region with origin point(<paramref name="originX"/>, <paramref name="originY"/>) and size(<paramref name="sizeX"/>, <paramref name="sizeY"/>).
	/// </summary>
	/// <param name="originX">Region origin point X.</param>
	/// <param name="originY">Region origin point Y.</param>
	/// <param name="sizeX">Region size X.</param>
	/// <param name="sizeY">Region size Y.</param>
	public Region2d(N originX, N originY, N sizeX, N sizeY)
	{
		Origin = new(originX, originY);
		Size = new(sizeX, sizeY);
	}

	/// <summary>
	/// Creates new region with origin point(<paramref name="originX"/>, <paramref name="originY"/>) and <paramref name="size"/>.
	/// </summary>
	/// <param name="originX">Region origin point X.</param>
	/// <param name="originY">Region origin point Y.</param>
	/// <param name="size">Region size.</param>
	public Region2d(N originX, N originY, Vector2d<N> size)
	{
		Origin = new(originX, originY);
		Size = size;
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region2d(ReadOnlySpan<N> data)
	{
		if (data.Length < 4)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region2d<N>>(ref Unsafe.As<N, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region2d(ReadOnlySpan<byte> data)
	{
		if (data.Length < 4 * sizeof(N))
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region2d<N>>(ref MemoryMarshal.GetReference(data));
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
	/// Center of region.
	/// </summary>
	public Vector2d<N> Center
		=> Origin + HalfSize;

	/// <summary>
	/// Half size of region.
	/// </summary>
	public Vector2d<N> HalfSize
		=> Size / Scalar<N>.Two;


	/// <summary>
	/// Point where region is ends.
	/// </summary>
	public Vector2d<N> End
		=> Origin + Size;
}