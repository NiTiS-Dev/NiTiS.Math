using System;
using System.Collections.Generic;
using System.Numerics;
using CommunityToolkit.Diagnostics;

namespace NiTiS.Mathematics;

/// <summary>
/// Represents an abstract mathematical curve that defines a relationship between abscissa (X) and ordinate (Y) values.
/// </summary>
/// <typeparam name="TX">Numeric type used for abscissa.</typeparam>
/// <typeparam name="TY">Numeric type used for ordinate.</typeparam>
public abstract class Curve<TX, TY>
	where TX : INumber<TX>
	where TY : INumber<TY>
{
	/// <summary>
	/// Evaluates the curve at the specified abscissa (X) value.
	/// </summary>
	/// <param name="x">The abscissa (X-axis) value at which to evaluate the curve.</param>
	/// <returns>The ordinate (Y-axis) value corresponding to the given X value on the curve.</returns>
	public abstract TY Get(TX x);
	
	// /// <summary>
	// /// Gets the domain of the curve (the range of X values where the curve is defined).
	// /// </summary>
	// public abstract Region<TX>? Domain { get; }
	//
	// /// <summary>
	// /// Gets the range of the curve (the resulting Y values across the domain).
	// /// </summary>
	// public abstract Region<TY>? Range { get; }

	/// <summary>
	/// Represent point on the curve.
	/// </summary>
	public readonly record struct Point : IComparable<Point>
	{
		/// <summary>
		/// The X position.
		/// </summary>
		public readonly TX X;

		/// <summary>
		/// The Y position.
		/// </summary>
		public readonly TY Y;

		/// <summary>
		/// Creates a new point.
		/// </summary>
		/// <param name="x">The X position.</param>
		/// <param name="y">The Y position.</param>
		public Point(TX x, TY y)
		{
			X = x;
			Y = y;
		}

		int IComparable<Point>.CompareTo(Point other)
		{
			return X.CompareTo(other.X);
		}
	}
}