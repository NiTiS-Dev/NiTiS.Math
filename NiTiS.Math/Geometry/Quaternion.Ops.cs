using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using NiTiS.Math.Matrices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Geometry;

public static class Quaternion
{
	/// <summary>Creates a quaternion from the specified rotation matrix.</summary>
	/// <param name="matrix">The rotation matrix.</param>
	/// <returns>The newly created quaternion.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Quaternion<T> CreateFromRotationMatrix<T>(Matrix4x4<T> matrix)
		where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>, IRootFunctions<T>
	{
		T trace = matrix.M11 + matrix.M22 + matrix.M33;

		Quaternion<T> q = default;

		if (trace > T.Zero)
		{
			T s = T.Sqrt(trace + T.One);
			q.W = s * Scalar<T>.Half;
			s = Scalar<T>.Half / s;
			q.X = (matrix.M23 - matrix.M32) * s;
			q.Y = (matrix.M31 - matrix.M13) * s;
			q.Z = (matrix.M12 - matrix.M21) * s;
		}
		else
		{
			if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
			{
				T s = T.Sqrt(T.One + matrix.M11 - matrix.M22 - matrix.M33);
				T invS = Scalar<T>.Half / s;
				q.X = Scalar<T>.Half * s;
				q.Y = (matrix.M12 + matrix.M21) * invS;
				q.Z = (matrix.M13 + matrix.M31) * invS;
				q.W = (matrix.M23 - matrix.M32) * invS;
			}
			else if (matrix.M22 > matrix.M33)
			{
				T s = T.Sqrt(T.One + matrix.M22 - matrix.M11 - matrix.M33);
				T invS = Scalar<T>.Half / s;
				q.X = (matrix.M21 + matrix.M12) * invS;
				q.Y = Scalar<T>.Half * s;
				q.Z = (matrix.M32 + matrix.M23) * invS;
				q.W = (matrix.M31 - matrix.M13) * invS;
			}
			else
			{
				T s = T.Sqrt(T.One + matrix.M33 - matrix.M11 - matrix.M22);
				T invS = Scalar<T>.Half / s;
				q.X = (matrix.M31 + matrix.M13) * invS;
				q.Y = (matrix.M32 + matrix.M23) * invS;
				q.Z = Scalar<T>.Half * s;
				q.W = (matrix.M12 - matrix.M21) * invS;
			}
		}

		return q;
	}

	/// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
	/// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
	/// <param name="pitch">The pitch angle, in radians, around the X axis.</param>
	/// <param name="roll">The roll angle, in radians, around the Z axis.</param>
	/// <returns>The resulting quaternion.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Quaternion<T> CreateFromYawPitchRoll<T>(T yaw, T pitch, T roll)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		//  Roll first, about axis the object is facing, then
		//  pitch upward, then yaw to face into the new heading
		T sr, cr, sp, cp, sy, cy;

		T halfRoll = roll * Scalar<T>.Half;
		sr = T.Sin(halfRoll);
		cr = T.Cos(halfRoll);

		T halfPitch = pitch * Scalar<T>.Half;
		sp = T.Sin(halfPitch);
		cp = T.Cos(halfPitch);

		T halfYaw = yaw * Scalar<T>.Half;
		sy = T.Sin(halfYaw);
		cy = T.Cos(halfYaw);

		Quaternion<T> result;

		result.X = cy * sp * cr + sy * cp * sr;
		result.Y = sy * cp * cr - cy * sp * sr;
		result.Z = cy * cp * sr - sy * sp * cr;
		result.W = cy * cp * cr + sy * sp * sr;

		return result;
	}
}