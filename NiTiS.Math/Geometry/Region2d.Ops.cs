﻿using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using NiTiS.Math.Vectors;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Geometry;

public static class Region2d
{
	/// <summary>
	/// Calculate perimeter of <paramref name="region"/>.
	/// </summary>
	/// <param name="region">Perimeter operand.</param>
	/// <returns>Perimeter of <paramref name="region"/>.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static T Perimeter<T>(Region2d<T> region)
		where T : unmanaged, INumberBase<T>
		=> Vector2d.Sum(region.Size * Scalar<T>.Two);

	/// <summary>
	/// Calculate square of <paramref name="region"/>.
	/// </summary>
	/// <param name="region">Square operand.</param>
	/// <returns>Square of <paramref name="region"/>.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static T Square<T>(Region2d<T> region)
		where T : unmanaged, INumberBase<T>
		=> region.Width * region.Height;

	/// <summary>
	/// Defines does regions <paramref name="left"/> and <paramref name="right"/> has a collision.
	/// </summary>
	/// <param name="left">Left operand.</param>
	/// <param name="right">Right operand.</param>
	/// <returns><see langword="true"/> when has a collision, otherwise <see langword="false"/>.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static bool Collision<T>(Region2d<T> left, Region2d<T> right)
		where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>
	{
		Vector2d<T>
			legin = left.Origin,
			lend = left.End,
			rend = right.End,
			regin = right.Origin;

		if (
			   lend.X > regin.X && legin.X < rend.X
			|| lend.Y > regin.Y && legin.Y < rend.Y
			)
		{
			return true;
		}

		return false;
	}
}