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
/// Represents a 3 dimension vector, where for each dimension used <typeparamref name="T"/> type.
/// </summary>
/// <remarks>
/// Instead of using <see cref="Vector3{T}"/> with <see cref="float"/> type argument, use standard vector type <see cref="System.Numerics.Vector3"/>
/// </remarks>
/// <typeparam name="T">Number type to describe dimension, can be either integer or float.</typeparam>
[DebuggerDisplay($"{{{nameof(ToString)}()}}")]
[StructLayout(LayoutKind.Sequential)]
[DataContract]
public struct Vector3<T> : IEquatable<Vector3<T>>, IFormattable
	where T : unmanaged, INumber<T>
{
	/// <summary>
	/// Vector elements count.
	/// </summary>
	public const int Count = 3;

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

	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	[DataMember(Order = 2)]
	public T Z;
	
	/// <summary>Creates a new <see cref="Vector3" /> object whose all elements have the same value.</summary>
	/// <param name="value">The value to assign to all elements.</param>
	public Vector3(T value) : this(value, value, value) {}
	
	/// <summary>
	/// Creates a new <see cref="Vector3" /> instance, with declared elements.
	/// </summary>
	/// <param name="x">The X value.</param>
	/// <param name="y">The Y value.</param>
	/// <param name="z">The Z value.</param>
	public Vector3(T x, T y, T z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// <summary>
	/// Initialize instance from provided span.
	/// </summary>
	/// <param name="values">Span containing packed vector elements.</param>
	public Vector3(ReadOnlySpan<T> values)
	{
		if (values.Length < Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(values));
		}

		this = Unsafe.ReadUnaligned<Vector3<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	/// <summary>
	/// Initialize instance from provided bytes span.
	/// </summary>
	/// <param name="data">Span containing raw packed vector elements.</param>
	public unsafe Vector3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(Vector3<T>))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(data));
		}

		this = Unsafe.ReadUnaligned<Vector3<T>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Vector instance5 with all zeroes.
	/// </summary>
	public static Vector3<T> Zero => default;

	/// <summary>
	/// Vector (1, 0, 0).
	/// </summary>
	public static Vector3<T> UnitX => new(T.One, T.Zero, T.Zero);

	/// <summary>
	/// Vector (0, 1, 0).
	/// </summary>
	public static Vector3<T> UnitY => new(T.Zero, T.One, T.Zero);

	/// <summary>
	/// Vector (0, 0, 1).
	/// </summary>
	public static Vector3<T> UnitZ => new(T.Zero, T.Zero, T.One);

	/// <summary>
	/// Reference to first vector element.
	/// </summary>
	public readonly ref T FirstRef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref Unsafe.As<Vector3<T>, T>(ref Unsafe.AsRef(in this));
	}

	/// <summary>
	/// Read-only reference to first vector element.
	/// </summary>
	public readonly ref readonly T FirstRORef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref Unsafe.As<Vector3<T>, T>(ref Unsafe.AsRef(in this));
	}
	
	/// <summary>
	/// Perform vector addition.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X + right.X, 
			left.Y + right.Y, 
			left.Z + right.Z);
	}

	/// <summary>
	/// Perform vector substraction.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X - right.X, 
			left.Y - right.Y, 
			left.Z - right.Z);
	}

	/// <summary>
	/// Perform vector multiplication.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X * right.X, 
			left.Y * right.Y, 
			left.Z * right.Z);
	}

	/// <summary>
	/// Perform vector by scalar multiplication.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator *(T left, Vector3<T> right)
	{
		return new( 
			left * right.X, 
			left * right.Y, 
			left * right.Z);
	}

	/// <summary>
	/// Perform vector by scalar multiplication.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator *(Vector3<T> left, T right)
	{
		return new( 
			left.X * right, 
			left.Y * right, 
			left.Z * right);
	}

	/// <summary>
	/// Perform vector division.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X / right.X, 
			left.Y / right.Y, 
			left.Z / right.Z);
	}

	/// <summary>
	/// Perform vector by scalar division.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator /(T left, Vector3<T> right)
	{
		return new( 
			left / right.X, 
			left / right.Y, 
			left / right.Z);
	}

	/// <summary>
	/// Perform vector by scalar division.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator /(Vector3<T> left, T right)
	{
		return new( 
			left.X / right, 
			left.Y / right, 
			left.Z / right);
	}

	/// <summary>
	/// Perform vector modulo operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator %(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X % right.X, 
			left.Y % right.Y, 
			left.Z % right.Z);
	}

	/// <summary>
	/// Perform vector by scalar modulo operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator %(T left, Vector3<T> right)
	{
		return new( 
			left % right.X, 
			left % right.Y, 
			left % right.Z);
	}

	/// <summary>
	/// Perform vector by scalar modulo operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator %(Vector3<T> left, T right)
	{
		return new( 
			left.X % right, 
			left.Y % right, 
			left.Z % right);
	}

	/* TODO: Somehow fix it
	/// <summary>
	/// Perform bitwise or operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator |(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X | right.X, 
			left.Y | right.Y, 
			left.Z | right.Z);
	}

	/// <summary>
	/// Perform bitwise and operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator &(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X & right.X, 
			left.Y & right.Y, 
			left.Z & right.Z);
	}

	/// <summary>
	/// Perform bitwise xor operation.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Operation result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> operator ^(Vector3<T> left, Vector3<T> right)
	{
		return new( 
			left.X ^ right.X, 
			left.Y ^ right.Y, 
			left.Z ^ right.Z);
	}
	*/

	/// <summary>
	/// Perform equality comparing.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector3<T> left, Vector3<T> right)
	{
		return left.X == right.X
			&& left.Y == right.Y	&& left.Z == right.Z;
	}

	/// <summary>
	/// Perform inequality comparing.
	/// </summary>
	/// <param name="left">The left operation parameter.</param>
	/// <param name="right">The right operation parameter.</param>
	/// <returns>Inequality of input parameters.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector3<T> left, Vector3<T> right)
	{
		return left.X != right.X
			|| left.Y != right.Y	|| left.Z != right.Z;
	}

	/// <inheritdoc/>
	public readonly override bool Equals(object? other)
	{
		return other is Vector3<T> vector && Equals(vector);
	}

	/// <summary>
	/// Check equality with <paramref name="other"/> <see cref="Vector3{T}" /> instance.
 	/// </summary>
	/// <param name="other">Other vector instance.</param>
	/// <returns>Equality of presented values.</returns>
	public readonly bool Equals(Vector3<T> other)
	{
		return this == other;
	}

	/// <summary>Restricts a vector between a minimum and a maximum value.</summary>
	/// <param name="value">The vector to restrict.</param>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>The restricted vector.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> Clamp(Vector3<T> value, Vector3<T> min, Vector3<T> max)
	{
		return Min(Max(value, min), max);
	}


	/// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
	/// <param name="value1">The first vector.</param>
	/// <param name="value2">The second vector.</param>
	/// <returns>The maximized vector.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> Max(Vector3<T> value1, Vector3<T> value2)
	{
		return new(
			(value1.X > value2.X) ? value1.X : value2.X, 
			(value1.Y > value2.Y) ? value1.Y : value2.Y, 
			(value1.Z > value2.Z) ? value1.Z : value2.Z 
		);
	}
	
	/// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
	/// <param name="value1">The first vector.</param>
	/// <param name="value2">The second vector.</param>
	/// <returns>The minimized vector.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3<T> Min(Vector3<T> value1, Vector3<T> value2)
	{
		return new(
			(value1.X < value2.X) ? value1.X : value2.X, 
			(value1.Y < value2.Y) ? value1.Y : value2.Y, 
			(value1.Z < value2.Z) ? value1.Z : value2.Z 
		);
	}

	/// <summary>Returns the hash code for this instance.</summary>
	/// <returns>The hash code.</returns>
	public readonly override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
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

		return $"{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}";
	}
}