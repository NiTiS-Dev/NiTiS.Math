using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CommunityToolkit.Diagnostics;

namespace NiTiS.Mathematics;

/// <summary>
/// Represents a 2 dimension vector, where for each dimension used <typeparamref name="T"/> type.
/// </summary>
/// <remarks>
/// Instead of using <see cref="Vector2{T}"/> with <see cref="float"/> type argument, use standard vector type <see cref="System.Numerics.Vector2"/>
/// </remarks>
/// <typeparam name="T">Number type to describe dimension, can be either integer or float.</typeparam>
[DebuggerDisplay($"{{{nameof(ToString)}()}}")]
[StructLayout(LayoutKind.Sequential)]
[DataContract]
public struct Vector2<T> : IEquatable<Vector2<T>>, IFormattable
	where T : unmanaged, INumber<T>
{
	/// <summary>
	/// Vector elements count.
	/// </summary>
	public const int Count = 2;

	/// <summary>
	/// The X component of the vector.
	/// </summary>
	[DataMember(Order = 0)]
	public T X;

	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	[DataMember(Order = 1)]
	public T Y;
	
	/// <summary>Creates a new <see cref="Vector2" /> object whose all elements have the same value.</summary>
	/// <param name="value">The value to assign to all elements.</param>
	public Vector2(T value) : this(value, value) {}
	
	/// <summary>
	/// Creates a new <see cref="Vector2" /> instance, with declared elements.
	/// </summary>
	/// <param name="x">The X value.</param>
	/// <param name="y">The Y value.</param>
	public Vector2(T x, T y)
	{
		X = x;
		Y = y;
	}

	/// <summary>
	/// Initialize instance from provided span.
	/// </summary>
	/// <param name="values">Span containing packed vector elements.</param>
	public Vector2(ReadOnlySpan<T> values)
	{
		if (values.Length < Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(values));
		}

		this = Unsafe.ReadUnaligned<Vector2<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	/// <summary>
	/// Initialize instance from provided bytes span.
	/// </summary>
	/// <param name="data">Span containing raw packed vector elements.</param>
	public unsafe Vector2(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(Vector2<T>))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(data));
		}

		this = Unsafe.ReadUnaligned<Vector2<T>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Vector instance5 with all zeroes.
	/// </summary>
	public static Vector2<T> Zero => default;

	/// <summary>
	/// Vector (1, 0).
	/// </summary>
	public static Vector2<T> UnitX => new(T.One, T.Zero);

	/// <summary>
	/// Vector (0, 1).
	/// </summary>
	public static Vector2<T> UnitY => new(T.Zero, T.One);

	/// <summary>
	/// Reference to first vector element.
	/// </summary>
	public readonly ref T FirstRef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref Unsafe.As<Vector2<T>, T>(ref Unsafe.AsRef(in this));
	}

	/// <summary>
	/// Read-only reference to first vector element.
	/// </summary>
	public readonly ref readonly T FirstRORef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref Unsafe.As<Vector2<T>, T>(ref Unsafe.AsRef(in this));
	}
	
	/// <summary>
	/// Perform vector addition.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X + right.X, 
			left.Y + right.Y);
	}

	/// <summary>
	/// Perform vector substraction.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X - right.X, 
			left.Y - right.Y);
	}

	/// <summary>
	/// Perform vector multiplication.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X * right.X, 
			left.Y * right.Y);
	}

	/// <summary>
	/// Perform vector by scalar multiplication.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator *(T left, Vector2<T> right)
	{
		return new( 
			left * right.X, 
			left * right.Y);
	}

	/// <summary>
	/// Perform vector by scalar multiplication.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator *(Vector2<T> left, T right)
	{
		return new( 
			left.X * right, 
			left.Y * right);
	}

	/// <summary>
	/// Perform vector division.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X / right.X, 
			left.Y / right.Y);
	}

	/// <summary>
	/// Perform vector by scalar division.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator /(T left, Vector2<T> right)
	{
		return new( 
			left / right.X, 
			left / right.Y);
	}

	/// <summary>
	/// Perform vector by scalar division.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator /(Vector2<T> left, T right)
	{
		return new( 
			left.X / right, 
			left.Y / right);
	}

	/// <summary>
	/// Perform vector modulo operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator %(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X % right.X, 
			left.Y % right.Y);
	}

	/// <summary>
	/// Perform vector by scalar modulo operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator %(T left, Vector2<T> right)
	{
		return new( 
			left % right.X, 
			left % right.Y);
	}

	/// <summary>
	/// Perform vector by scalar modulo operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator %(Vector2<T> left, T right)
	{
		return new( 
			left.X % right, 
			left.Y % right);
	}

	/* TODO: Somehow fix it
	/// <summary>
	/// Perform bitwise or operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator |(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X | right.X, 
			left.Y | right.Y);
	}

	/// <summary>
	/// Perform bitwise and operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator &(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X & right.X, 
			left.Y & right.Y);
	}

	/// <summary>
	/// Perform bitwise xor operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> operator ^(Vector2<T> left, Vector2<T> right)
	{
		return new( 
			left.X ^ right.X, 
			left.Y ^ right.Y);
	}
	*/

	/// <summary>
	/// Perform equality comparing.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector2<T> left, Vector2<T> right)
	{
		return left.X == right.X
			&& left.Y == right.Y;
	}

	/// <summary>
	/// Perform inequality comparing.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Inequality of input parameters.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector2<T> left, Vector2<T> right)
	{
		return left.X != right.X
			|| left.Y != right.Y;
	}

	/// <inheritdoc/>
	public readonly override bool Equals(object? other)
	{
		return other is Vector2<T> vector && Equals(vector);
	}

	/// <summary>
	/// Check equality with <paramref name="other"/> <see cref="Vector2{T}" /> instance.
 	/// </summary>
	/// <param name="other">Other vector instance.</param>
	/// <returns>Equality of presented values.</returns>
	public readonly bool Equals(Vector2<T> other)
	{
		return this == other;
	}

	/// <summary>Restricts a vector between a minimum and a maximum value.</summary>
	/// <param name="value">The vector to restrict.</param>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>The restricted vector.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> Clamp(Vector2<T> value, Vector2<T> min, Vector2<T> max)
	{
		return Min(Max(value, min), max);
	}

	/// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
	/// <param name="value1">The first vector.</param>
	/// <param name="value2">The second vector.</param>
	/// <returns>The maximized vector.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> Max(Vector2<T> value1, Vector2<T> value2)
	{
		return new(
			(value1.X > value2.X) ? value1.X : value2.X, 
			(value1.Y > value2.Y) ? value1.Y : value2.Y
		);
	}
	
	/// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
	/// <param name="value1">The first vector.</param>
	/// <param name="value2">The second vector.</param>
	/// <returns>The minimized vector.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2<T> Min(Vector2<T> value1, Vector2<T> value2)
	{
		return new(
			(value1.X < value2.X) ? value1.X : value2.X, 
			(value1.Y < value2.Y) ? value1.Y : value2.Y
		);
	}

	/// <summary>Returns the hash code for this instance.</summary>
	/// <returns>The hash code.</returns>
	public readonly override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	/// <inheritdoc />
	public readonly override string ToString()
	{
		return ToString("G", CultureInfo.CurrentCulture);
	}

	/// <summary>Formats the value of the current instance using the specified format.</summary>
	/// <param name="format">The format to use.
	/// -or-
	/// A null reference (<see langword="Nothing" /> in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.
	/// </param>
	/// <returns>The value of the current instance in the specified format.</returns>
	public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
	{
		return ToString(format, CultureInfo.CurrentCulture);
	}

	/// <inheritdoc />
	public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
	{
		string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

		return $"{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}";
	}
}