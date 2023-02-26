using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS.Math.Geometry;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerTypeProxy(typeof(IRegionDebugView<>))]
[DebuggerDisplay("Size = {Size}")]
public unsafe struct Region2d<T>
	where T : unmanaged, INumberBase<T>
{
	/// <summary>
	/// Origin point of region.
	/// </summary>
	public Vector2d<T> Begin;
	/// <summary>
	/// Size of region.
	/// </summary>
	public Vector2d<T> Size;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="begin"></param>
	/// <param name="size"></param>
	public Region2d(Vector2d<T> begin, Vector2d<T> size)
	{
		Begin = begin;
		Size = size;
	}
	public Region2d(Vector2d<T> begin, T sizeX, T sizeY)
	{
		Begin = begin;
		Size = new(sizeX, sizeY);
	}
	public Region2d(T beginX, T beginY, T sizeX, T sizeY)
	{
		Begin = new(beginX, beginY);
		Size = new(sizeX, sizeY);
	}
	public Region2d(T beginX, T beginY, Vector2d<T> size)
	{
		Begin = new(beginX, beginY);
		Size = size;
	}
	public Region2d(ReadOnlySpan<T> data)
	{
		if (data.Length < 4)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region2d<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(data)));
	}
	public Region2d(ReadOnlySpan<byte> data)
	{
		if (data.Length < 4 * sizeof(T))
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Region2d<T>>(ref MemoryMarshal.GetReference(data));
	}

	public T Width =>
		Size.X;
	public T Height =>
		Size.Y;
	public Vector2d<T> End
		=> Begin + Size;
}