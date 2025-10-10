using CommunityToolkit.Diagnostics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NiTiS.Mathematics;

/// <summary>
/// Represents a 4 dimension vector, where for each dimension used <typeparamref name="T"/> type.
/// </summary>
/// <typeparam name="T">Number type to describe dimension, can be either integer or float.</typeparam>
[DebuggerDisplay($"{{{nameof(ToString)}()}}")]
[StructLayout(LayoutKind.Sequential)]
[DataContract]
public struct Vector4<T> : IEquatable<Vector4<T>>
	where T : unmanaged
{
	/// <summary>
	/// Vector elements count.
	/// </summary>
	public const int Count = 4;

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
	
	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	[DataMember(Order = 3)]
	public T W;
	
	/// <summary>Creates a new <see cref="Vector4" /> object whose all elements have the same value.</summary>
	/// <param name="value">The value to assign to all elements.</param>
	public Vector4(T value) : this(value, value, value, value) {}
	
	/// <summary>
	/// Creates a new <see cref="Vector4" /> instance, with declared elements.
	/// </summary>
	/// <param name="x">The X value.</param>
	/// <param name="y">The Y value.</param>
	/// <param name="z">The Z value.</param>
	/// <param name="w">The W value.</param>
	public Vector4(T x, T y, T z, T w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	/// <summary>
	/// Initialize instance from provided span.
	/// </summary>
	/// <param name="values">Span containing packed vector elements.</param>
	public Vector4(ReadOnlySpan<T> values)
	{
		if (values.Length < Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(values));
		}

		this = Unsafe.ReadUnaligned<Vector4<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	/// <summary>
	/// Initialize instance from provided bytes span.
	/// </summary>
	/// <param name="data">Span containing raw packed vector elements.</param>
	public unsafe Vector4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(Vector4<T>))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(data));
		}

		this = Unsafe.ReadUnaligned<Vector4<T>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>
	/// Deconstructs vector to components.
	/// </summary>
	/// <param name="x">The X component of the vector.</param>
	/// <param name="y">The Y component of the vector.</param>
	/// <param name="z">The Z component of the vector.</param>
	/// <param name="w">The W component of the vector.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly void Deconstruct(out T x, out T y, out T z, out T w)
	{
		x = X;
		y = Y;
		z = Z;
		w = W;
	}

	/// <summary>
	/// Reference to first vector element.
	/// </summary>
	public readonly ref T FirstRef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref Unsafe.As<Vector4<T>, T>(ref Unsafe.AsRef(in this));
	}

	/// <summary>
	/// Read-only reference to first vector element.
	/// </summary>
	public readonly ref readonly T FirstRORef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref Unsafe.As<Vector4<T>, T>(ref Unsafe.AsRef(in this));
	}

	/// <inheritdoc/>
	public readonly override bool Equals(object? other)
	{
		return other is Vector4<T> vector && Equals(vector);
	}

	/// <summary>
	/// Check equality with <paramref name="other"/> <see cref="Vector4{T}" /> instance.
	/// </summary>
	/// <param name="other">Other vector instance.</param>
	/// <returns>Equality of presented values.</returns>
	public readonly bool Equals(Vector4<T> other)
	{
		return X.Equals(other.X)
			&& Y.Equals(other.Y)
			&& Z.Equals(other.Z)
			&& W.Equals(other.W);
	}

	/// <summary>Returns the hash code for this instance.</summary>
	/// <returns>The hash code.</returns>
	public readonly override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}

	/// <summary>
	/// Perform equality comparing.
	/// </summary>
	/// <param name="lhs">The left operation parameter.</param>
	/// <param name="rhs">The right operation parameter.</param>
	/// <returns>Equality of input parameters.</returns>
	public static bool operator ==(Vector4<T> lhs, Vector4<T> rhs)
	{
		return lhs.Equals(rhs);
	}

	/// <summary>
	/// Perform inequality comparing.
	/// </summary>
	/// <param name="lhs">The left operation parameter.</param>
	/// <param name="rhs">The right operation parameter.</param>
	/// <returns>Inequality of input parameters.</returns>
	public static bool operator !=(Vector4<T> lhs, Vector4<T> rhs)
	{
		return !(lhs == rhs);
	}

	/// <inheritdoc />
	public readonly override string ToString()
	{
		return $"<{X}, {Y}, {Z}, {W}>";
	}
}