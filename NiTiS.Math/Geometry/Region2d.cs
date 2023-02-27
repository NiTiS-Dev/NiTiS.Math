using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NiTiS.Math.Vectors;

namespace NiTiS.Math.Geometry;

/// <summary>
/// Two-dimension region with origin point and size.
/// </summary>
/// <typeparam name="T">Region type.</typeparam>
[DebuggerTypeProxy(typeof(IRegionDebugView<>))]
[DebuggerDisplay("Size = {Size}")]
public unsafe struct Region2d<T>
	where T : unmanaged, INumberBase<T>
{
	/// <summary>
	/// Origin point of region.
	/// </summary>
	public Vector2d<T> Origin;
	/// <summary>
	/// Size of region.
	/// </summary>
	public Vector2d<T> Size;

	/// <summary>
	/// Creates new region with <paramref name="origin"/> point and <paramref name="size"/>.
	/// </summary>
	/// <param name="origin">Region origin point.</param>
	/// <param name="size">Region size.</param>
	public Region2d(Vector2d<T> origin, Vector2d<T> size)
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
	public Region2d(Vector2d<T> origin, T sizeX, T sizeY)
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
	public Region2d(T originX, T originY, T sizeX, T sizeY)
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
	public Region2d(T originX, T originY, Vector2d<T> size)
	{
		Origin = new(originX, originY);
		Size = size;
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region2d(ReadOnlySpan<T> data)
	{
		if (data.Length < 4)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region2d<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates region by buffer.
	/// </summary>
	/// <param name="data">Buffer with region data.</param>
	/// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="data"/> buffer not enough for creation.</exception>
	public Region2d(ReadOnlySpan<byte> data)
	{
		if (data.Length < 4 * sizeof(T))
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region2d<T>>(ref MemoryMarshal.GetReference(data));
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
	/// Point where region is ends.
	/// </summary>
	public Vector2d<T> End
		=> Origin + Size;
}