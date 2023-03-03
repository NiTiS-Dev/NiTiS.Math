using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Geometry;

/// <summary>Represents a vector that is used to encode three-dimensional physical rotations.</summary>
/// <remarks>The <see cref="Quaternion{T}" /> structure is used to efficiently rotate an object about the (x,y,z) vector by the angle theta, where:
/// <c>w = cos(theta/2)</c></remarks>
/// <typeparam name="N">Quaternion data type.</typeparam>
[DebuggerDisplay($"{{{nameof(ToString)}()}}")]
public unsafe struct Quaternion<N> :
	IAdditionOperators<Quaternion<N>, Quaternion<N>, Quaternion<N>>,
	ISubtractionOperators<Quaternion<N>, Quaternion<N>, Quaternion<N>>,
	IMultiplyOperators<Quaternion<N>, Quaternion<N>, Quaternion<N>>,
	IDivisionOperators<Quaternion<N>, Quaternion<N>, Quaternion<N>>,
	IEqualityOperators<Quaternion<N>, Quaternion<N>, bool>,

	IUnaryNegationOperators<Quaternion<N>, Quaternion<N>>,

	IEquatable<Quaternion<N>>
	where N :
	unmanaged,
	INumberBase<N>
{
	/// <summary>The X value of the vector component of the quaternion</summary>
	public N X;
	/// <summary>The Y value of the vector component of the quaternion</summary>
	public N Y;
	/// <summary>The Z value of the vector component of the quaternion</summary>
	public N Z;
	/// <summary>The rotation component of the quaternion</summary>
	public N W;

	/// <summary>Calculates the squared length of the quaternion.</summary>
	/// <returns>The length squared of the quaternion.</returns>
	public readonly N LengthSquared
	{
		[MethodImpl(AggressiveInlining | AggressiveOptimization)]
		get => Quaternion.Dot(this, this);
	}

	/// <summary>Constructs a quaternion from the specified components.</summary>
	/// <param name="x">The value to assign to the X component of the quaternion.</param>
	/// <param name="y">The value to assign to the Y component of the quaternion.</param>
	/// <param name="z">The value to assign to the Z component of the quaternion.</param>
	/// <param name="w">The value to assign to the W component of the quaternion.</param>
	public Quaternion(N x, N y, N z, N w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	/// <summary>Constructs a quaternion from the 4d vector.</summary>
	/// <param name="xyzw">The vector to assign values of the quaternion.</param>
	public Quaternion(Vector4d<N> xyzw)
	{
		X = xyzw.X;
		Y = xyzw.Y;
		Z = xyzw.Z;
		W = xyzw.W;
	}

	/// <summary>Creates a quaternion from the specified vector and rotation parts.</summary>
	/// <param name="vectorPart">The vector part of the quaternion.</param>
	/// <param name="scalarPart">The rotation part of the quaternion.</param>
	public Quaternion(Vector3d<N> vectorPart, N scalarPart)
	{
		X = vectorPart.X;
		Y = vectorPart.Y;
		Z = vectorPart.Z;
		W = scalarPart;
	}

	/// <summary>
	/// Creates a quaternion from <paramref name="data"/> values.
	/// </summary>
	/// <param name="data">Buffer with elements data.</param>
	/// <exception cref="ArgumentOutOfRangeException">The size of <paramref name="data"/> buffer is less than 4 elements.</exception>
	public Quaternion(ReadOnlySpan<N> data)
	{
		if (data.Length < 4)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Quaternion<N>>(ref Unsafe.As<N, byte>(ref MemoryMarshal.GetReference(data)));
	}

	/// <summary>
	/// Creates a quaternion from <paramref name="data"/> values.
	/// </summary>
	/// <param name="data">Buffer with elements data.</param>
	/// <exception cref="ArgumentOutOfRangeException">The size of <paramref name="data"/> buffer is less than 4 elements.</exception>
	public Quaternion(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(N) * 4)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Quaternion<N>>(ref MemoryMarshal.GetReference(data));
	}

	/// <summary>Gets a quaternion that represents no rotation.</summary>
	/// <value>A quaternion whose values are <c>(0, 0, 0, 1)</c>.</value>
	public static Quaternion<N> Identity
		=> new(N.Zero, N.Zero, N.Zero, N.One);

	/// <summary>Gets a quaternion that represents a zero.</summary>
	/// <value>A quaternion whose values are <c>(0, 0, 0, 0)</c>.</value>
	public static Quaternion<N> Zero
		=> new(N.Zero, N.Zero, N.Zero, N.Zero);

	/// <summary>Gets a value that indicates whether the current instance is the identity quaternion.</summary>
	/// <value><see langword="true" /> if the current instance is the identity quaternion; otherwise, <see langword="false" />.</value>
	public readonly bool IsIdentity
		=> W == N.One
		&& X == N.Zero
		&& Y == N.Zero
		&& Z == N.Zero;

	/// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
	/// <param name="left">The left quaternion.</param>
	/// <param name="right">The right quaternion.</param>
	/// <returns>The quaternion that contains the summed values of <paramref name="left" /> and <paramref name="right" />.</returns>
	/// <remarks>The <see cref="Quaternion{T}.op_Addition" /> method defines the operation of the addition operator for <see cref="Quaternion{T}" /> objects.</remarks>
	public static Quaternion<N> operator +(Quaternion<N> left, Quaternion<N> right)
	{
		left.X = left.X + right.X;
		left.Y = left.Y + right.Y;
		left.Z = left.Z + right.Z;
		left.W = left.W + right.W;

		return left;
	}

	/// <summary>Subtracts each element in a second quaternion from its corresponding element in a left quaternion.</summary>
	/// <param name="left">The left quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="right" /> from its corresponding element in <paramref name="left" />.</returns>
	/// <remarks>The <see cref="Quaternion{T}.op_Subtraction" /> method defines the operation of the subtraction operator for <see cref="Quaternion{T}" /> objects.</remarks>
	public static Quaternion<N> operator -(Quaternion<N> left, Quaternion<N> right)
	{
		left.X = left.X - right.X;
		left.Y = left.Y - right.Y;
		left.Z = left.Z - right.Z;
		left.W = left.W - right.W;

		return left;
	}

	/// <summary>Divides one quaternion by a second quaternion.</summary>
	/// <param name="left">The dividend.</param>
	/// <param name="right">The divisor.</param>
	/// <returns>The quaternion that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
	/// <remarks>The <see cref="System.Numerics.Quaternion.op_Division" /> method defines the division operation for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
	public static Quaternion<N> operator /(Quaternion<N> left, Quaternion<N> right)
	{
		Quaternion<N> ans;
		
		N q1x = left.X;
		N q1y = left.Y;
		N q1z = left.Z;
		N q1w = left.W;

		//-------------------------------------
		// Inverse part.
		N ls = right.X * right.X + right.Y * right.Y +
				   right.Z * right.Z + right.W * right.W;
		N invNorm = N.One / ls;

		N q2x = -right.X * invNorm;
		N q2y = -right.Y * invNorm;
		N q2z = -right.Z * invNorm;
		N q2w = right.W * invNorm;

		//-------------------------------------
		// Multiply part.

		// cross(av, bv)
		N cx = q1y * q2z - q1z * q2y;
		N cy = q1z * q2x - q1x * q2z;
		N cz = q1x * q2y - q1y * q2x;

		N dot = q1x * q2x + q1y * q2y + q1z * q2z;

		ans.X = q1x * q2w + q2x * q1w + cx;
		ans.Y = q1y * q2w + q2y * q1w + cy;
		ans.Z = q1z * q2w + q2z * q1w + cz;
		ans.W = q1w * q2w - dot;

		return ans;
	}

	/// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
	/// <param name="left">The left quaternion.</param>
	/// <param name="right">The right quaternion.</param>
	/// <returns>The product quaternion.</returns>
	/// <remarks>The <see cref="Quaternion{T}.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
	public static Quaternion<N> operator *(Quaternion<N> left, Quaternion<N> right)
	{
		Quaternion<N> ans;

		N q1x = left.X;
		N q1y = left.Y;
		N q1z = left.Z;
		N q1w = left.W;

		N q2x = right.X;
		N q2y = right.Y;
		N q2z = right.Z;
		N q2w = right.W;

		// cross(av, bv)
		N cx = q1y * q2z - q1z * q2y;
		N cy = q1z * q2x - q1x * q2z;
		N cz = q1x * q2y - q1y * q2x;

		N dot = q1x * q2x + q1y * q2y + q1z * q2z;

		ans.X = q1x * q2w + q2x * q1w + cx;
		ans.Y = q1y * q2w + q2y * q1w + cy;
		ans.Z = q1z * q2w + q2z * q1w + cz;
		ans.W = q1w * q2w - dot;

		return ans;
	}

	/// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
	/// <param name="left">The source quaternion.</param>
	/// <param name="right">The scalar value.</param>
	/// <returns>The scaled quaternion.</returns>
	/// <remarks>The <see cref="Quaternion{T}.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="Quaternion{T}" /> objects.</remarks>
	public static Quaternion<N> operator *(Quaternion<N> left, N right)
	{
		left.X = left.X * right;
		left.Y = left.Y * right;
		left.Z = left.Z * right;
		left.W = left.W * right;

		return left;
	}

	/// <summary>Reverses the sign of each component of the quaternion.</summary>
	/// <param name="operand">The quaternion to negate.</param>
	/// <returns>The negated quaternion.</returns>
	/// <remarks>The <see cref="Quaternion{T}.op_UnaryNegation" /> method defines the operation of the unary negation operator for <see cref="Quaternion{T}" /> objects.</remarks>
	public static Quaternion<N> operator -(Quaternion<N> operand)
	{
		operand.X = -operand.X;
		operand.Y = -operand.Y;
		operand.Z = -operand.Z;
		operand.W = -operand.W;

		return operand;
	}

	/// <summary>Returns a value that indicates whether two quaternions are equal.</summary>
	/// <param name="left">The left quaternion to compare.</param>
	/// <param name="right">The right quaternion to compare.</param>
	/// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
	/// <remarks>Two quaternions are equal if each of their corresponding components is equal.
	/// The <see cref="Quaternion{T}.op_Equality" /> method defines the operation of the equality operator for <see cref="Quaternion{T}" /> objects.</remarks>
	public static bool operator ==(Quaternion<N> left, Quaternion<N> right)
	{
		return (left.X == right.X)
			&& (left.Y == right.Y)
			&& (left.Z == right.Z)
			&& (left.W == right.W);
	}

	/// <summary>Returns a value that indicates whether two quaternions are not equal.</summary>
	/// <param name="left">The left quaternion to compare.</param>
	/// <param name="right">The right quaternion to compare.</param>
	/// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
	public static bool operator !=(Quaternion<N> left, Quaternion<N> right)
	{
		return (left.X != right.X)
			|| (left.Y != right.Y)
			|| (left.Z != right.Z)
			|| (left.W != right.W);
	}

	/// <inheritdoc/>
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Quaternion<N> q && q == this;

	/// <inheritdoc/>
	public readonly bool Equals(Quaternion<N> other)
		=> other == this;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y, Z, W);
	
	/// <inheritdoc/>
	public override string ToString()
		=> $"{{<{X}, {Y}, {Z}>, {W}}}";
}