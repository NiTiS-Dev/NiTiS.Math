using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using NiTiS.Math.Matrices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Geometry;

/// <summary>
/// <see cref="Quaternion{N}"/> operations provider.
/// </summary>
public static class Quaternion
{
	#region Add
	/// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
	/// <param name="left">The first quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <returns>The quaternion that contains the summed values of <paramref name="left" /> and <paramref name="right" />.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Add<N>(this Quaternion<N> left, Quaternion<N> right)
		where N : unmanaged, INumberBase<N>
	{
		return left + right;
	}
	#endregion

	#region Concatenate
	/// <summary>Concatenates two quaternions.</summary>
	/// <param name="left">The first quaternion rotation in the series.</param>
	/// <param name="right">The second quaternion rotation in the series.</param>
	/// <returns>A new quaternion representing the concatenation of the <paramref name="left" /> rotation followed by the <paramref name="right" /> rotation.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Quaternion<N> Concatenate<N>(this Quaternion<N> left, Quaternion<N> right)
		where N : unmanaged, INumberBase<N>
	{
		Unsafe.SkipInit(out Quaternion<N> result);

		// Concatenate rotation is actually q2 * q1 instead of q1 * q2.
		// So that's why value2 goes q1 and value1 goes q2.
		N q1x = right.X;
		N q1y = right.Y;
		N q1z = right.Z;
		N q1w = right.W;

		N q2x = left.X;
		N q2y = left.Y;
		N q2z = left.Z;
		N q2w = left.W;

		// cross(av, bv)
		N cx = q1y * q2z - q1z * q2y;
		N cy = q1z * q2x - q1x * q2z;
		N cz = q1x * q2y - q1y * q2x;

		N dot = q1x * q2x + q1y * q2y + q1z * q2z;

		result.X = q1x * q2w + q2x * q1w + cx;
		result.Y = q1y * q2w + q2y * q1w + cy;
		result.Z = q1z * q2w + q2z * q1w + cz;
		result.W = q1w * q2w - dot;

		return result;
	}
	#endregion

	#region Conjugate
	/// <summary>Returns the conjugate of a specified quaternion.</summary>
	/// <param name="operand">The quaternion.</param>
	/// <returns>A new quaternion that is the conjugate of <see langword="value" />.</returns>
	[MethodImpl(AggressiveInlining | AggressiveOptimization)]
	public static Quaternion<N> Conjugate<N>(this Quaternion<N> operand)
		where N : unmanaged, INumberBase<N>
		=> Multiply(operand, new Vector4d<N>(-N.One, -N.One, -N.One, N.One));
	#endregion

	#region CreateFrom...
	/// <summary>Creates a quaternion from a unit vector and an angle to rotate around the vector.</summary>
	/// <param name="axis">The unit vector to rotate around.</param>
	/// <param name="angle">The angle, in radians, to rotate around the vector.</param>
	/// <returns>The newly created quaternion.</returns>
	/// <remarks><paramref name="axis" /> vector must be normalized before calling this method or the resulting <see cref="Quaternion" /> will be incorrect.</remarks>
	public static Quaternion<F> CreateFromAxisAngle<F>(Vector3d<F> axis, F angle)
		where F : unmanaged, INumberBase<F>, ITrigonometricFunctions<F>
	{
		Unsafe.SkipInit(out Quaternion<F> result);

		F halfAngle = angle * Scalar<F>.Half;
		F s = F.Sin(halfAngle);
		F c = F.Cos(halfAngle);

		result.X = axis.X * s;
		result.Y = axis.Y * s;
		result.Z = axis.Z * s;
		result.W = c;

		return result;
	}

	/// <summary>Creates a quaternion from the specified rotation matrix.</summary>
	/// <param name="matrix">The rotation matrix.</param>
	/// <returns>The newly created quaternion.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Quaternion<F> CreateFromRotationMatrix<F>(Matrix4x4<F> matrix)
		where F : unmanaged, INumberBase<F>, IComparisonOperators<F, F, bool>, IRootFunctions<F>
	{
		F trace = matrix.M11 + matrix.M22 + matrix.M33;

		Unsafe.SkipInit(out Quaternion<F> result);

		if (trace > F.Zero)
		{
			F s = F.Sqrt(trace + F.One);
			result.W = s * Scalar<F>.Half;
			s = Scalar<F>.Half / s;
			result.X = (matrix.M23 - matrix.M32) * s;
			result.Y = (matrix.M31 - matrix.M13) * s;
			result.Z = (matrix.M12 - matrix.M21) * s;
		}
		else
		{
			if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
			{
				F s = F.Sqrt(F.One + matrix.M11 - matrix.M22 - matrix.M33);
				F invS = Scalar<F>.Half / s;
				result.X = Scalar<F>.Half * s;
				result.Y = (matrix.M12 + matrix.M21) * invS;
				result.Z = (matrix.M13 + matrix.M31) * invS;
				result.W = (matrix.M23 - matrix.M32) * invS;
			}
			else if (matrix.M22 > matrix.M33)
			{
				F s = F.Sqrt(F.One + matrix.M22 - matrix.M11 - matrix.M33);
				F invS = Scalar<F>.Half / s;
				result.X = (matrix.M21 + matrix.M12) * invS;
				result.Y = Scalar<F>.Half * s;
				result.Z = (matrix.M32 + matrix.M23) * invS;
				result.W = (matrix.M31 - matrix.M13) * invS;
			}
			else
			{
				F s = F.Sqrt(F.One + matrix.M33 - matrix.M11 - matrix.M22);
				F invS = Scalar<F>.Half / s;
				result.X = (matrix.M31 + matrix.M13) * invS;
				result.Y = (matrix.M32 + matrix.M23) * invS;
				result.Z = Scalar<F>.Half * s;
				result.W = (matrix.M12 - matrix.M21) * invS;
			}
		}

		return result;
	}

	/// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
	/// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
	/// <param name="pitch">The pitch angle, in radians, around the X axis.</param>
	/// <param name="roll">The roll angle, in radians, around the Z axis.</param>
	/// <returns>The resulting quaternion.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Quaternion<F> CreateFromYawPitchRoll<F>(F yaw, F pitch, F roll)
		where F : unmanaged, INumberBase<F>, ITrigonometricFunctions<F>
	{
		//  Roll first, about axis the object is facing, then
		//  pitch upward, then yaw to face into the new heading
		F sr, cr, sp, cp, sy, cy;

		F halfRoll = roll * Scalar<F>.Half;
		sr = F.Sin(halfRoll);
		cr = F.Cos(halfRoll);

		F halfPitch = pitch * Scalar<F>.Half;
		sp = F.Sin(halfPitch);
		cp = F.Cos(halfPitch);

		F halfYaw = yaw * Scalar<F>.Half;
		sy = F.Sin(halfYaw);
		cy = F.Cos(halfYaw);

		Quaternion<F> result;

		result.X = cy * sp * cr + sy * cp * sr;
		result.Y = sy * cp * cr - cy * sp * sr;
		result.Z = cy * cp * sr - sy * sp * cr;
		result.W = cy * cp * cr + sy * sp * sr;

		return result;
	}
	#endregion

	#region Divide
	/// <summary>Divides one quaternion by a second quaternion.</summary>
	/// <param name="left">The dividend.</param>
	/// <param name="right">The divisor.</param>
	/// <returns>The quaternion that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Divide<N>(this Quaternion<N> left, Quaternion<N> right)
		where N : unmanaged, INumberBase<N>
	{
		return left / right;
	}

	/// <summary>Divides the specified quaternion by a specified scalar value.</summary>
	/// <param name="left">The quaternion.</param>
	/// <param name="divisor">The scalar value.</param>
	/// <returns>The quaternion that results from the division.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Divide<N>(this Quaternion<N> left, N divisor)
		where N : unmanaged, INumberBase<N>
	{
		return new(
			left.X / divisor,
			left.Y / divisor,
			left.Z / divisor,
			left.W / divisor
		);
	}
	#endregion

	#region Dot
	/// <summary>Calculates the dot product of two quaternions.</summary>
	/// <param name="left">The first quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <returns>The dot product.</returns>
	[MethodImpl(AggressiveInlining)]
	public static N Dot<N>(this Quaternion<N> left, Quaternion<N> right)
		where N : unmanaged, INumberBase<N>
	{
		return (left.X * right.X)
			 + (left.Y * right.Y)
			 + (left.Z * right.Z)
			 + (left.W * right.W);
	}
	#endregion

	#region Inverse
	/// <summary>Returns the inverse of a quaternion.</summary>
	/// <param name="operand">The quaternion.</param>
	/// <returns>The inverted quaternion.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Inverse<N>(this Quaternion<N> operand)
		where N : unmanaged, INumberBase<N>
	{
		//  -1   (       a              -v       )
		// q   = ( -------------   ------------- )
		//       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

		return Divide(Conjugate(operand), LengthSquared(operand));
	}
	#endregion

	#region Lerp
	/// <summary>Performs a linear interpolation between two quaternions based on a value that specifies the weighting of the second quaternion.</summary>
	/// <param name="left">The first quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <param name="amount">The relative weight of <paramref name="right" /> in the interpolation.</param>
	/// <returns>The interpolated quaternion.</returns>
	public static Quaternion<N> Lerp<N>(this Quaternion<N> left, Quaternion<N> right, N amount)
		where N : unmanaged, INumberBase<N>, IComparisonOperators<N, N, bool>, IRootFunctions<N>
	{
		N t = amount;
		N t1 = N.One - t;

		Unsafe.SkipInit(out Quaternion<N> result);

		N dot = left.X * right.X + left.Y * right.Y +
					left.Z * right.Z + left.W * right.W;

		if (dot >= N.Zero)
		{
			result.X = t1 * left.X + t * right.X;
			result.Y = t1 * left.Y + t * right.Y;
			result.Z = t1 * left.Z + t * right.Z;
			result.W = t1 * left.W + t * right.W;
		}
		else
		{
			result.X = t1 * left.X - t * right.X;
			result.Y = t1 * left.Y - t * right.Y;
			result.Z = t1 * left.Z - t * right.Z;
			result.W = t1 * left.W - t * right.W;
		}

		// Normalize it.
		N ls = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
		N invNorm = N.One / N.Sqrt(ls);

		result.X *= invNorm;
		result.Y *= invNorm;
		result.Z *= invNorm;
		result.W *= invNorm;

		return result;
	}
	#endregion

	#region Length
	/// <summary>Calculates the length of the quaternion.</summary>
	/// <returns>The computed length of the quaternion.</returns>
	[MethodImpl(AggressiveInlining)]
	public static N Length<N>(this Quaternion<N> operand)
		where N : unmanaged, INumberBase<N>, IRootFunctions<N>
	{
		N lengthSquared = LengthSquared(operand);
		return N.Sqrt(lengthSquared);
	}

	/// <summary>Calculates the squared length of the quaternion.</summary>
	/// <returns>The length squared of the quaternion.</returns>
	[MethodImpl(AggressiveInlining)]
	public static N LengthSquared<N>(this Quaternion<N> operand)
		where N : unmanaged, INumberBase<N>
	{
		return Dot(operand, operand);
	}
	#endregion

	#region Multiply
	/// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
	/// <param name="left">The first quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <returns>The product quaternion.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Multiply<N>(this Quaternion<N> left, Quaternion<N> right)
		where N : unmanaged, INumberBase<N>
	{
		return left * right;
	}

	/// <summary>Returns a new quaternion whose values are the product of each pair of elements in specified quaternion and vector.</summary>
	/// <param name="left">The quaternion.</param>
	/// <param name="right">The vector.</param>
	/// <returns>The element-wise product vector.</returns>
	[MethodImpl(AggressiveInlining)]
	internal static Quaternion<N> Multiply<N>(this Quaternion<N> left, Vector4d<N> right)
		where N : unmanaged, INumberBase<N>
	{
		return new(
			left.X * right.X,
			left.Y * right.Y,
			left.Z * right.Z,
			left.W * right.W
		);
	}
	#endregion

	#region Negate
	/// <summary>Reverses the sign of each component of the quaternion.</summary>
	/// <param name="operand">The quaternion to negate.</param>
	/// <returns>The negated quaternion.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Negate<N>(this Quaternion<N> operand)
		where N : unmanaged, INumberBase<N>
	{
		return -operand;
	}
	#endregion

	#region Normalize
	/// <summary>Divides each component of a specified <see cref="Quaternion" /> by its length.</summary>
	/// <param name="operand">The quaternion to normalize.</param>
	/// <returns>The normalized quaternion.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Normalize<N>(this Quaternion<N> operand)
		where N : unmanaged, INumberBase<N>, IRootFunctions<N>
	{
		return Divide(operand, Length(operand));
	}
	#endregion

	#region Slerp
	/// <summary>Interpolates between two quaternions, using spherical linear interpolation.</summary>
	/// <param name="left">The first quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <param name="amount">The relative weight of the second quaternion in the interpolation.</param>
	/// <returns>The interpolated quaternion.</returns>
	public static Quaternion<N> Slerp<N>(this Quaternion<N> left, Quaternion<N> right, N amount)
		where N : unmanaged, INumberBase<N>, IComparisonOperators<N, N, bool>, ITrigonometricFunctions<N>
	{
		N t = amount;

		N cosOmega = left.X * right.X + left.Y * right.Y +
						 left.Z * right.Z + left.W * right.W;

		bool flip = false;

		if (cosOmega < N.One)
		{
			flip = true;
			cosOmega = -cosOmega;
		}

		N s1, s2;

		if (cosOmega > (N.One - FloatScalar<N>.SlerpEpsilon))
		{
			// Too close, do straight linear interpolation.
			s1 = N.One - t;
			s2 = (flip) ? -t : t;
		}
		else
		{
			N omega = N.Acos(cosOmega);
			N invSinOmega = N.One / N.Sin(omega);

			s1 = N.Sin((N.One - t) * omega) * invSinOmega;
			s2 = (flip)
				? -N.Sin(t * omega) * invSinOmega
				: N.Sin(t * omega) * invSinOmega;
		}

		Unsafe.SkipInit(out Quaternion<N> result);

		result.X = s1 * left.X + s2 * right.X;
		result.Y = s1 * left.Y + s2 * right.Y;
		result.Z = s1 * left.Z + s2 * right.Z;
		result.W = s1 * left.W + s2 * right.W;

		return result;
	}
	#endregion

	#region Substraction
	/// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
	/// <param name="left">The first quaternion.</param>
	/// <param name="right">The second quaternion.</param>
	/// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="right" /> from its corresponding element in <paramref name="left" />.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Quaternion<N> Subtract<N>(this Quaternion<N> left, Quaternion<N> right)
		where N : unmanaged, INumberBase<N>
	{
		return left - right;
	}
	#endregion
}