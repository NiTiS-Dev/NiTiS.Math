using NiTiS.Core.Annotations;
using NiTiS.Math.Geometry;
using NiTiS.Math.Matrices;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Matrices;

public static unsafe class Matrix4x4
{
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> ConvertToGeneric(this System.Numerics.Matrix4x4 matrix)
		=> Unsafe.As<System.Numerics.Matrix4x4, Matrix4x4<float>>(ref matrix);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static System.Numerics.Matrix4x4 ConvertToSystem(this Matrix4x4<float> matrix)
		=> Unsafe.As<Matrix4x4<float>, System.Numerics.Matrix4x4>(ref matrix);

	private const float BillboardEpsilon = 1e-4f;
	private const float BillboardMinAngle = 1.0f - 0.1f * (MathF.PI / 180.0f); // 0.1 degrees
	private const float DecomposeEpsilon = 0.0001f;

	#region Create
	/// <summary>
	/// Creates a spherical billboard that rotates around a specified object position.
	/// </summary>
	/// <param name="ojbectPosition">The position of the object that the billboard will rotate around.</param>
	/// <param name="cameraPosition">The position of the camera.</param>
	/// <param name="cameraUp">The up vector of the camera.</param>
	/// <param name="cameraForward">The forward vector of the camera.</param>
	/// <returns>The created billboard.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> CreateBillboard(Vector3d<float> ojbectPosition, Vector3d<float> cameraPosition, Vector3d<float> cameraUp, Vector3d<float> cameraForward)
	{
		Vector3d<float> zaxis = ojbectPosition - cameraPosition;
		float norm = zaxis.LengthSquared;

		if (norm < BillboardEpsilon)
		{
			zaxis = -cameraForward;
		}
		else
		{
			zaxis = Vector3d.Multiply(zaxis, 1.0f / MathF.Sqrt(norm));
		}

		Vector3d<float> xaxis = Vector3d.Normalize(Vector3d.Cross(cameraUp, zaxis));
		Vector3d<float> yaxis = Vector3d.Cross(zaxis, xaxis);

		Matrix4x4<float> result;

		result.M11 = xaxis.X;
		result.M12 = xaxis.Y;
		result.M13 = xaxis.Z;
		result.M14 = 0.0f;

		result.M21 = yaxis.X;
		result.M22 = yaxis.Y;
		result.M23 = yaxis.Z;
		result.M24 = 0.0f;

		result.M31 = zaxis.X;
		result.M32 = zaxis.Y;
		result.M33 = zaxis.Z;
		result.M34 = 0.0f;

		result.M41 = ojbectPosition.X;
		result.M42 = ojbectPosition.Y;
		result.M43 = ojbectPosition.Z;
		result.M44 = 1.0f;

		return result;
	}

	/// <summary>
	/// Creates a cylindrical billboard that rotates around a specified axis.
	/// </summary>
	/// <param name="objectPosition">The position of the object that the billboard will rotate around.</param>
	/// <param name="cameraPosition">The position of the camera.</param>
	/// <param name="rotateAxis">The axis to rotate the billboard around.</param>
	/// <param name="cameraForward">The forward vector of the camera.</param>
	/// <param name="objectForward">The forward vector of the object.</param>
	/// <returns>The billboard matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> CreateConstrainedBillboard(Vector3d<float> objectPosition, Vector3d<float> cameraPosition, Vector3d<float> rotateAxis, Vector3d<float> cameraForward, Vector3d<float> objectForward)
	{
		Vector3d<float> faceDir = objectPosition - cameraPosition;
		float norm = faceDir.LengthSquared;

		if (norm < BillboardEpsilon)
		{
			faceDir = -cameraForward;
		}
		else
		{
			faceDir = Vector3d.Multiply(faceDir, 1.0f / MathF.Sqrt(norm));
		}

		Vector3d<float> yaxis = rotateAxis;
		Vector3d<float> xaxis;
		Vector3d<float> zaxis;

		// Treat the case when angle between faceDir and rotateAxis is too close to 0.
		float dot = Vector3d.Dot(rotateAxis, faceDir);

		if (MathF.Abs(dot) > BillboardMinAngle)
		{
			zaxis = objectForward;

			// Make sure passed values are useful for compute.
			dot = Vector3d.Dot(rotateAxis, zaxis);

			if (MathF.Abs(dot) > BillboardMinAngle)
			{
				zaxis = MathF.Abs(rotateAxis.Z) > BillboardMinAngle ? Vector3d<float>.UnitX : -Vector3d<float>.UnitZ;
			}

			xaxis = Vector3d.Normalize(Vector3d.Cross(rotateAxis, zaxis));
			zaxis = Vector3d.Normalize(Vector3d.Cross(xaxis, rotateAxis));
		}
		else
		{
			xaxis = Vector3d.Normalize(Vector3d.Cross(rotateAxis, faceDir));
			zaxis = Vector3d.Normalize(Vector3d.Cross(xaxis, yaxis));
		}

		Matrix4x4<float> result;

		result.M11 = xaxis.X;
		result.M12 = xaxis.Y;
		result.M13 = xaxis.Z;
		result.M14 = 0.0f;

		result.M21 = yaxis.X;
		result.M22 = yaxis.Y;
		result.M23 = yaxis.Z;
		result.M24 = 0.0f;

		result.M31 = zaxis.X;
		result.M32 = zaxis.Y;
		result.M33 = zaxis.Z;
		result.M34 = 0.0f;

		result.M41 = objectPosition.X;
		result.M42 = objectPosition.Y;
		result.M43 = objectPosition.Z;
		result.M44 = 1.0f;

		return result;
	}

	/// <summary>
	/// Creates a matrix that rotates around an arbitrary vector.
	/// </summary>
	/// <param name="axis">The axis to rotate around.</param>
	/// <param name="angle">The angle to rotate around axis, in radians.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateFromAxisAngle<T>(Vector3d<T> axis, T angle)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		/*
		// a: angle
		// x, y, z: unit vector for axis.
		//
		// Rotation matrix M can compute by using below equation.
		//
		//        T               T
		//  M = uu + (cos a)( I-uu ) + (sin a)S
		//
		// Where:
		//
		//  u = ( x, y, z )
		//
		//      [  0 -z  y ]
		//  S = [  z  0 -x ]
		//      [ -y  x  0 ]
		//
		//      [ 1 0 0 ]
		//  I = [ 0 1 0 ]
		//      [ 0 0 1 ]
		//
		//
		//     [  xx+cosa*(1-xx)   yx-cosa*yx-sina*z zx-cosa*xz+sina*y ]
		// M = [ xy-cosa*yx+sina*z    yy+cosa(1-yy)  yz-cosa*yz-sina*x ]
		//     [ zx-cosa*zx-sina*y zy-cosa*zy+sina*x   zz+cosa*(1-zz)  ]
		*/
		T x = axis.X, y = axis.Y, z = axis.Z;
		T sa = T.Sin(angle), ca = T.Cos(angle);
		T xx = x * x, yy = y * y, zz = z * z;
		T xy = x * y, xz = x * z, yz = y * z;

		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = xx + ca * (T.One - xx);
		result.M12 = xy - ca * xy + sa * z;
		result.M13 = xz - ca * xz - sa * y;

		result.M21 = xy - ca * xy - sa * z;
		result.M22 = yy + ca * (T.One - yy);
		result.M23 = yz - ca * yz + sa * x;

		result.M31 = xz - ca * xz + sa * y;
		result.M32 = yz - ca * yz - sa * x;
		result.M33 = zz + ca * (T.One - zz);

		return result;
	}

	/// <summary>
	/// Creates a rotation matrix from the specified Quaternion rotation value.
	/// </summary>
	/// <param name="quaternion">The source Quaternion.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateFromQuaternion<T>(Quaternion<T> quaternion)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T xx = quaternion.X * quaternion.X;
		T yy = quaternion.Y * quaternion.Y;
		T zz = quaternion.Z * quaternion.Z;

		T xy = quaternion.X * quaternion.Y;
		T wz = quaternion.Z * quaternion.W;
		T xz = quaternion.Z * quaternion.X;
		T wy = quaternion.Y * quaternion.W;
		T yz = quaternion.Y * quaternion.Z;
		T wx = quaternion.X * quaternion.W;

		T Two = T.One + T.One;

		result.M11 = T.One - Two * (yy + zz);
		result.M12 = Two * (xy + wz);
		result.M13 = Two * (xz - wy);

		result.M21 = Two * (xy - wz);
		result.M22 = T.One - Two * (zz + xx);
		result.M23 = Two * (yz + wx);

		result.M31 = Two * (xz + wy);
		result.M32 = Two * (yz - wx);
		result.M33 = T.One - Two * (yy + xx);

		return result;
	}

	/// <summary>Creates a rotation matrix from the specified yaw, pitch, and roll.</summary>
	/// <param name="yaw">The angle of rotation, in radians, around the Y axis.</param>
	/// <param name="pitch">The angle of rotation, in radians, around the X axis.</param>
	/// <param name="roll">The angle of rotation, in radians, around the Z axis.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateFromYawPitchRoll<T>(T yaw, T pitch, T roll)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Quaternion<T> q = Geometry.Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
		return CreateFromQuaternion(q);
	}

	/// <summary>
	/// Creates a view matrix.
	/// </summary>
	/// <param name="cameraPosition">The position of the camera.</param>
	/// <param name="cameraTarget">The target towards which the camera is pointing.</param>
	/// <param name="cameraUpVector">The direction that is "up" from the camera's point of view.</param>
	/// <returns>The view matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateLookAt<T>(Vector3d<T> cameraPosition, Vector3d<T> cameraTarget, Vector3d<T> cameraUpVector)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
	{
		Vector3d<T> zaxis = Vector3d.Normalize(cameraPosition - cameraTarget);
		Vector3d<T> xaxis = Vector3d.Normalize(Vector3d.Cross(cameraUpVector, zaxis));
		Vector3d<T> yaxis = Vector3d.Cross(zaxis, xaxis);

		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = xaxis.X;
		result.M12 = yaxis.X;
		result.M13 = zaxis.X;

		result.M21 = xaxis.Y;
		result.M22 = yaxis.Y;
		result.M23 = zaxis.Y;

		result.M31 = xaxis.Z;
		result.M32 = yaxis.Z;
		result.M33 = zaxis.Z;

		result.M41 = -Vector3d.Dot(xaxis, cameraPosition);
		result.M42 = -Vector3d.Dot(yaxis, cameraPosition);
		result.M43 = -Vector3d.Dot(zaxis, cameraPosition);

		return result;
	}

	/// <summary>
	/// Creates an orthographic perspective matrix from the given view volume dimensions.
	/// </summary>
	/// <param name="width">The width of the view volume.</param>
	/// <param name="height">The height of the view volume.</param>
	/// <param name="zNearPlane">The minimum Z-value of the view volume.</param>
	/// <param name="zFarPlane">The maximum Z-value of the view volume.</param>
	/// <returns>The orthographic projection matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateOrthographic<T>(T width, T height, T zNearPlane, T zFarPlane)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = Scalar<T>.Two / width;
		result.M22 = Scalar<T>.Two / height;
		result.M33 = T.One / (zNearPlane - zFarPlane);
		result.M43 = zNearPlane / (zNearPlane - zFarPlane);

		return result;
	}

	/// <summary>
	/// Creates a customized orthographic projection matrix.</summary>
	/// <param name="left">The minimum X-value of the view volume.</param>
	/// <param name="right">The maximum X-value of the view volume.</param>
	/// <param name="bottom">The minimum Y-value of the view volume.</param>
	/// <param name="top">The maximum Y-value of the view volume.</param>
	/// <param name="zNearPlane">The minimum Z-value of the view volume.</param>
	/// <param name="zFarPlane">The maximum Z-value of the view volume.</param>
	/// <returns>The orthographic projection matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateOrthographicOffCenter<T>(T left, T right, T bottom, T top, T zNearPlane, T zFarPlane)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = Scalar<T>.Two / (right - left);

		result.M22 = Scalar<T>.Two / (top - bottom);

		result.M33 = T.One / (zNearPlane - zFarPlane);

		result.M41 = (left + right) / (left - right);
		result.M42 = (top + bottom) / (bottom - top);
		result.M43 = zNearPlane / (zNearPlane - zFarPlane);

		return result;
	}

	/// <summary>
	/// Creates a perspective projection matrix from the given view volume dimensions.
	/// </summary>
	/// <param name="width">The width of the view volume at the near view plane.</param>
	/// <param name="height">The height of the view volume at the near view plane.</param>
	/// <param name="nearPlaneDistance">The distance to the near view plane.</param>
	/// <param name="farPlaneDistance">The distance to the far view plane.</param>
	/// <returns>The perspective projection matrix.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	///	<paramref name="nearPlaneDistance" /> is less than or equal to zero.
	///	-or-
	///	<paramref name="farPlaneDistance" /> is less than or equal to zero.
	///	-or-
	///	<paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.
	/// </exception>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreatePerspective<T>(T width, T height, T nearPlaneDistance, T farPlaneDistance)
		where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>
	{
		if (nearPlaneDistance <= T.Zero)
			throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

		if (farPlaneDistance <= T.Zero)
			throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));

		if (nearPlaneDistance >= farPlaneDistance)
			throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

		T Two = T.One + T.One;

		Matrix4x4<T> result;

		result.M11 = Two * nearPlaneDistance / width;
		result.M12 = result.M13 = result.M14 = T.Zero;

		result.M22 = Two * nearPlaneDistance / height;
		result.M21 = result.M23 = result.M24 = T.Zero;

		T negFarRange = T.IsPositiveInfinity(farPlaneDistance) ? -T.One : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
		result.M33 = negFarRange;
		result.M31 = result.M32 = T.Zero;
		result.M34 = -T.One;

		result.M41 = result.M42 = result.M44 = T.Zero;
		result.M43 = nearPlaneDistance * negFarRange;

		return result;
	}

	/// <summary>
	/// Creates a perspective projection matrix based on a field of view, aspect ratio, and near and far view plane distances.
	/// </summary>
	/// <param name="fieldOfView">The field of view in the y direction, in radians.</param>
	/// <param name="aspectRatio">The aspect ratio, defined as view space width divided by height.</param>
	/// <param name="nearPlaneDistance">The distance to the near view plane.</param>
	/// <param name="farPlaneDistance">The distance to the far view plane.</param>
	/// <returns>The perspective projection matrix.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="fieldOfView" /> is less than or equal to zero.
	/// -or-
	/// <paramref name="fieldOfView" /> is greater than or equal to <see cref="SMath.PI" />.
	/// <paramref name="nearPlaneDistance" /> is less than or equal to zero.
	/// -or-
	/// <paramref name="farPlaneDistance" /> is less than or equal to zero.
	/// -or-
	/// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreatePerspectiveFieldOfView<T>(T fieldOfView, T aspectRatio, T nearPlaneDistance, T farPlaneDistance)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>, IComparisonOperators<T, T, bool>
	{
		if (fieldOfView <= T.Zero || fieldOfView >= T.Pi)
			throw new ArgumentOutOfRangeException(nameof(fieldOfView));

		if (nearPlaneDistance <= T.Zero)
			throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

		if (farPlaneDistance <= T.Zero)
			throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));

		if (nearPlaneDistance >= farPlaneDistance)
			throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

		T yScale = T.One / T.Tan(fieldOfView / (T.One + T.One));
		T xScale = yScale / aspectRatio;

		Matrix4x4<T> result;

		result.M11 = xScale;
		result.M12 = result.M13 = result.M14 = T.Zero;

		result.M22 = yScale;
		result.M21 = result.M23 = result.M24 = T.Zero;

		result.M31 = result.M32 = T.Zero;
		T negFarRange = T.IsPositiveInfinity(farPlaneDistance) ? -T.One : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
		result.M33 = negFarRange;
		result.M34 = -T.One;

		result.M41 = result.M42 = result.M44 = T.Zero;
		result.M43 = nearPlaneDistance * negFarRange;

		return result;
	}

	/// <summary>
	/// Creates a customized perspective projection matrix.
	/// </summary>
	/// <param name="left">The minimum x-value of the view volume at the near view plane.</param>
	/// <param name="right">The maximum x-value of the view volume at the near view plane.</param>
	/// <param name="bottom">The minimum y-value of the view volume at the near view plane.</param>
	/// <param name="top">The maximum y-value of the view volume at the near view plane.</param>
	/// <param name="nearPlaneDistance">The distance to the near view plane.</param>
	/// <param name="farPlaneDistance">The distance to the far view plane.</param>
	/// <returns>The perspective projection matrix.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="nearPlaneDistance" /> is less than or equal to zero.
	/// -or-
	/// <paramref name="farPlaneDistance" /> is less than or equal to zero.
	/// -or-
	/// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.
	/// </exception>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreatePerspectiveOffCenter<T>(T left, T right, T bottom, T top, T nearPlaneDistance, T farPlaneDistance)
		where T : unmanaged, INumberBase<T>, IComparisonOperators<T, T, bool>
	{
		if (nearPlaneDistance <= T.Zero)
			throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

		if (farPlaneDistance <= T.Zero)
			throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));

		if (nearPlaneDistance >= farPlaneDistance)
			throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

		Matrix4x4<T> result;

		T Two = T.One + T.One;

		result.M11 = Two * nearPlaneDistance / (right - left);
		result.M12 = result.M13 = result.M14 = T.Zero;

		result.M22 = Two * nearPlaneDistance / (top - bottom);
		result.M21 = result.M23 = result.M24 = T.Zero;

		result.M31 = (left + right) / (right - left);
		result.M32 = (top + bottom) / (top - bottom);
		T negFarRange = T.IsPositiveInfinity(farPlaneDistance) ? -T.One : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
		result.M33 = negFarRange;
		result.M34 = -T.One;

		result.M43 = nearPlaneDistance * negFarRange;
		result.M41 = result.M42 = result.M44 = T.Zero;

		return result;
	}

	/// <summary>
	/// Creates a matrix that reflects the coordinate system about a specified plane.
	/// </summary>
	/// <param name="value">The plane about which to create a reflection.</param>
	/// <returns>A new matrix expressing the reflection.</returns>
	[NotImplementYet]
	[Obsolete(nameof(NotImplementYetAttribute))]
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateReflection<T>(Plane<T> value)
		where T : unmanaged, INumberBase<T>
	{
		value = Geometry.Plane.Normalize(value);

		T a = value.Normal.X;
		T b = value.Normal.Y;
		T c = value.Normal.Z;

		T MinusTwo = -Scalar<T>.Two;

		T fa = MinusTwo * a;
		T fb = MinusTwo * b;
		T fc = MinusTwo * c;

		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = fa * a + T.One;
		result.M12 = fb * a;
		result.M13 = fc * a;

		result.M21 = fa * b;
		result.M22 = fb * b + T.One;
		result.M23 = fc * b;

		result.M31 = fa * c;
		result.M32 = fb * c;
		result.M33 = fc * c + T.One;

		result.M41 = fa * value.D;
		result.M42 = fb * value.D;
		result.M43 = fc * value.D;

		return result;
	}

	/// <summary>
	/// Creates a matrix for rotating points around the X axis.
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the X axis.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateRotationX<T>(T radians)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T c = T.Cos(radians);
		T s = T.Sin(radians);

		// [  1  0  0  0 ]
		// [  0  c  s  0 ]
		// [  0 -s  c  0 ]
		// [  0  0  0  1 ]

		result.M22 = c;
		result.M23 = s;
		result.M32 = -s;
		result.M33 = c;

		return result;
	}

	/// <summary>
	/// Creates a matrix for rotating points around the X axis from a center point.
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the X axis.</param>
	/// <param name="centerPoint">The center point.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateRotationX<T>(T radians, Vector3d<T> centerPoint)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T c = T.Cos(radians);
		T s = T.Sin(radians);

		T y = centerPoint.Y * (T.One - c) + centerPoint.Z * s;
		T z = centerPoint.Z * (T.One - c) - centerPoint.Y * s;

		// [  1  0  0  0 ]
		// [  0  c  s  0 ]
		// [  0 -s  c  0 ]
		// [  0  y  z  1 ]

		result.M22 = c;
		result.M23 = s;
		result.M32 = -s;
		result.M33 = c;
		result.M42 = y;
		result.M43 = z;

		return result;
	}

	/// <summary>
	/// Creates a matrix for rotating points around the Y axis.
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Y-axis.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateRotationY<T>(T radians)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T c = T.Cos(radians);
		T s = T.Sin(radians);

		// [  c  0 -s  0 ]
		// [  0  1  0  0 ]
		// [  s  0  c  0 ]
		// [  0  0  0  1 ]
		result.M11 = c;
		result.M13 = -s;
		result.M31 = s;
		result.M33 = c;

		return result;
	}

	/// <summary>
	/// The amount, in radians, by which to rotate around the Y axis from a center point.
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Y-axis.</param>
	/// <param name="centerPoint">The center point.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateRotationY<T>(T radians, Vector3d<T> centerPoint)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T c = T.Cos(radians);
		T s = T.Sin(radians);

		T x = centerPoint.X * (T.One - c) - centerPoint.Z * s;
		T z = centerPoint.Z * (T.One - c) + centerPoint.X * s;

		// [  c  0 -s  0 ]
		// [  0  1  0  0 ]
		// [  s  0  c  0 ]
		// [  x  0  z  1 ]
		result.M11 = c;
		result.M13 = -s;
		result.M31 = s;
		result.M33 = c;
		result.M41 = x;
		result.M43 = z;

		return result;
	}

	/// <summary>
	/// Creates a matrix for rotating points around the Z axis.
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Z-axis.</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateRotationZ<T>(T radians)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T c = T.Cos(radians);
		T s = T.Sin(radians);

		// [  c  s  0  0 ]
		// [ -s  c  0  0 ]
		// [  0  0  1  0 ]
		// [  0  0  0  1 ]
		result.M11 = c;
		result.M12 = s;
		result.M21 = -s;
		result.M22 = c;

		return result;
	}

	/// <summary>
	/// Creates a matrix for rotating points around the Z axis from a center point.
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Z-axis.</param>
	/// <param name="centerPoint">The center point.</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateRotationZ<T>(T radians, Vector3d<T> centerPoint)
		where T : unmanaged, INumberBase<T>, ITrigonometricFunctions<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T c = T.Cos(radians);
		T s = T.Sin(radians);

		T x = centerPoint.X * (T.One - c) + centerPoint.Y * s;
		T y = centerPoint.Y * (T.One - c) - centerPoint.X * s;

		// [  c  s  0  0 ]
		// [ -s  c  0  0 ]
		// [  0  0  1  0 ]
		// [  x  y  0  1 ]
		result.M11 = c;
		result.M12 = s;
		result.M21 = -s;
		result.M22 = c;
		result.M41 = x;
		result.M42 = y;

		return result;
	}

	/// <summary>
	/// Creates a scaling matrix from the specified X, Y, and Z components.
	/// </summary>
	/// <param name="xScale">The value to scale by on the X axis.</param>
	/// <param name="yScale">The value to scale by on the Y axis.</param>
	/// <param name="zScale">The value to scale by on the Z axis.</param>
	/// <returns>The scaling matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(T xScale, T yScale, T zScale)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;
		result.M11 = xScale;
		result.M22 = yScale;
		result.M33 = zScale;
		return result;
	}

	/// <summary>
	/// Creates a scaling matrix that is offset by a given center point.
	/// </summary>
	/// <param name="xScale">The value to scale by on the X axis.</param>
	/// <param name="yScale">The value to scale by on the Y axis.</param>
	/// <param name="zScale">The value to scale by on the Z axis.</param>
	/// <param name="centerPoint">The center point.</param>
	/// <returns>The scaling matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateScale<T>(T xScale, T yScale, T zScale, Vector3d<T> centerPoint)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T tx = centerPoint.X * (T.One - xScale);
		T ty = centerPoint.Y * (T.One - yScale);
		T tz = centerPoint.Z * (T.One - zScale);

		result.M11 = xScale;
		result.M22 = yScale;
		result.M33 = zScale;
		result.M41 = tx;
		result.M42 = ty;
		result.M43 = tz;
		return result;
	}

	/// <summary>
	/// Creates a scaling matrix from the specified vector scale.
	/// </summary>
	/// <param name="scales">The scale to use.</param>
	/// <returns>The scaling matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(Vector3d<T> scales)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;
		result.M11 = scales.X;
		result.M22 = scales.Y;
		result.M33 = scales.Z;
		return result;
	}

	/// <summary>
	/// Creates a scaling matrix with a center point.
	/// </summary>
	/// <param name="scales">The vector that contains the amount to scale on each axis.</param>
	/// <param name="centerPoint">The center point.</param>
	/// <returns>The scaling matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(Vector3d<T> scales, Vector3d<T> centerPoint)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T tx = centerPoint.X * (T.One - scales.X);
		T ty = centerPoint.Y * (T.One - scales.Y);
		T tz = centerPoint.Z * (T.One - scales.Z);

		result.M11 = scales.X;
		result.M22 = scales.Y;
		result.M33 = scales.Z;
		result.M41 = tx;
		result.M42 = ty;
		result.M43 = tz;
		return result;
	}

	/// <summary>
	/// Creates a uniform scaling matrix that scale equally on each axis.
	/// </summary>
	/// <param name="scale">The uniform scaling factor.</param>
	/// <returns>The scaling matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(T scale)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = scale;
		result.M22 = scale;
		result.M33 = scale;

		return result;
	}

	/// <summary>
	/// Creates a uniform scaling matrix that scales equally on each axis with a center point.
	/// </summary>
	/// <param name="scale">The uniform scaling factor.</param>
	/// <param name="centerPoint">The center point.</param>
	/// <returns>The scaling matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateScale<T>(T scale, Vector3d<T> centerPoint)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T tx = centerPoint.X * (T.One - scale);
		T ty = centerPoint.Y * (T.One - scale);
		T tz = centerPoint.Z * (T.One - scale);

		result.M11 = scale;
		result.M22 = scale;
		result.M33 = scale;

		result.M41 = tx;
		result.M42 = ty;
		result.M43 = tz;

		return result;
	}

	/// <summary>
	/// Creates a matrix that flattens geometry into a specified plane as if casting a shadow from a specified light source.</summary>
	/// <param name="lightDirection">The direction from which the light that will cast the shadow is coming.</param>
	/// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
	/// <returns>A new matrix that can be used to flatten geometry onto the specified plane from the specified direction.</returns>

	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateShadow<T>(Vector3d<T> lightDirection, Plane<T> plane)
		where T : unmanaged, INumberBase<T>
	{
		Plane<T> p = Geometry.Plane.Normalize(plane);

		T dot = p.Normal.X * lightDirection.X + p.Normal.Y * lightDirection.Y + p.Normal.Z * lightDirection.Z;
		T a = -p.Normal.X;
		T b = -p.Normal.Y;
		T c = -p.Normal.Z;
		T d = -p.D;

		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = a * lightDirection.X + dot;
		result.M21 = b * lightDirection.X;
		result.M31 = c * lightDirection.X;
		result.M41 = d * lightDirection.X;

		result.M12 = a * lightDirection.Y;
		result.M22 = b * lightDirection.Y + dot;
		result.M32 = c * lightDirection.Y;
		result.M42 = d * lightDirection.Y;

		result.M13 = a * lightDirection.Z;
		result.M23 = b * lightDirection.Z;
		result.M33 = c * lightDirection.Z + dot;
		result.M43 = d * lightDirection.Z;

		result.M44 = dot;

		return result;
	}

	/// <summary>
	/// Creates a translation matrix from the specified 3-dimensional vector.
	/// </summary>
	/// <param name="position">The amount to translate in each axis.</param>
	/// <returns>The translation matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateTranslation<T>(Vector3d<T> position)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;
		result.M41 = position.X;
		result.M42 = position.Y;
		result.M43 = position.Z;
		return result;
	}

	/// <summary>
	/// Creates a translation matrix from the specified X, Y, and Z components.
	/// </summary>
	/// <param name="xPosition">The amount to translate on the X axis.</param>
	/// <param name="yPosition">The amount to translate on the Y axis.</param>
	/// <param name="zPosition">The amount to translate on the Z axis.</param>
	/// <returns>The translation matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateTranslation<T>(T xPosition, T yPosition, T zPosition)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;
		result.M41 = xPosition;
		result.M42 = yPosition;
		result.M43 = zPosition;
		return result;
	}

	/// <summary>
	/// Creates a world matrix with the specified parameters.
	/// </summary>
	/// <param name="position">The position of the object.</param>
	/// <param name="forward">The forward direction of the object.</param>
	/// <param name="up">The upward direction of the object. Its value is usually <c>[0, 1, 0]</c>.</param>
	/// <returns>The world matrix.</returns>
	/// <remarks><paramref name="position" /> is used in translation operations.</remarks>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<T> CreateWorld<T>(Vector3d<T> position, Vector3d<T> forward, Vector3d<T> up)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
	{
		Vector3d<T> zaxis = Vector3d.Normalize(-forward);
		Vector3d<T> xaxis = Vector3d.Normalize(Vector3d.Cross(up, zaxis));
		Vector3d<T> yaxis = Vector3d.Cross(zaxis, xaxis);

		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		result.M11 = xaxis.X;
		result.M12 = xaxis.Y;
		result.M13 = xaxis.Z;

		result.M21 = yaxis.X;
		result.M22 = yaxis.Y;
		result.M23 = yaxis.Z;

		result.M31 = zaxis.X;
		result.M32 = zaxis.Y;
		result.M33 = zaxis.Z;

		result.M41 = position.X;
		result.M42 = position.Y;
		result.M43 = position.Z;

		return result;
	}
	#endregion

	/// <summary>Adds each element in one matrix with its corresponding element in a second matrix.</summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second matrix.</param>
	/// <returns>The matrix that contains the summed values of <paramref name="left" /> and <paramref name="right" />.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<float> Add(Matrix4x4<float> left, Matrix4x4<float> right)
	{
		if (AdvSimd.IsSupported)
		{
			AdvSimd.Store(&left.M11, AdvSimd.Add(AdvSimd.LoadVector128(&left.M11), AdvSimd.LoadVector128(&right.M11)));
			AdvSimd.Store(&left.M21, AdvSimd.Add(AdvSimd.LoadVector128(&left.M21), AdvSimd.LoadVector128(&right.M21)));
			AdvSimd.Store(&left.M31, AdvSimd.Add(AdvSimd.LoadVector128(&left.M31), AdvSimd.LoadVector128(&right.M31)));
			AdvSimd.Store(&left.M41, AdvSimd.Add(AdvSimd.LoadVector128(&left.M41), AdvSimd.LoadVector128(&right.M41)));
			return left;
		}
		else if (Sse.IsSupported)
		{
			Sse.Store(&left.M11, Sse.Add(Sse.LoadVector128(&left.M11), Sse.LoadVector128(&right.M11)));
			Sse.Store(&left.M21, Sse.Add(Sse.LoadVector128(&left.M21), Sse.LoadVector128(&right.M21)));
			Sse.Store(&left.M31, Sse.Add(Sse.LoadVector128(&left.M31), Sse.LoadVector128(&right.M31)));
			Sse.Store(&left.M41, Sse.Add(Sse.LoadVector128(&left.M41), Sse.LoadVector128(&right.M41)));
			return left;
		}

		Matrix4x4<float> m;

		m.M11 = left.M11 + right.M11;
		m.M12 = left.M12 + right.M12;
		m.M13 = left.M13 + right.M13;
		m.M14 = left.M14 + right.M14;
		m.M21 = left.M21 + right.M21;
		m.M22 = left.M22 + right.M22;
		m.M23 = left.M23 + right.M23;
		m.M24 = left.M24 + right.M24;
		m.M31 = left.M31 + right.M31;
		m.M32 = left.M32 + right.M32;
		m.M33 = left.M33 + right.M33;
		m.M34 = left.M34 + right.M34;
		m.M41 = left.M41 + right.M41;
		m.M42 = left.M42 + right.M42;
		m.M43 = left.M43 + right.M43;
		m.M44 = left.M44 + right.M44;

		return m;
	}

	/// <summary>Adds <paramref name="right"/> value in each element of a second matrix.</summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second value.</param>
	/// <returns>The matrix that contains the summed values of <paramref name="left" /> and <paramref name="right" />.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<float> Add(Matrix4x4<float> left, float right)
	{
		if (AdvSimd.IsSupported)
		{
			Span<float> span = stackalloc float[4];
			fixed (float* pRight = span)
			{
				pRight[0] = right;
				pRight[1] = right;
				pRight[2] = right;
				pRight[3] = right;
				AdvSimd.Store(&left.M11, AdvSimd.Add(AdvSimd.LoadVector128(&left.M11), AdvSimd.LoadVector128(pRight)));
				AdvSimd.Store(&left.M21, AdvSimd.Add(AdvSimd.LoadVector128(&left.M21), AdvSimd.LoadVector128(pRight)));
				AdvSimd.Store(&left.M31, AdvSimd.Add(AdvSimd.LoadVector128(&left.M31), AdvSimd.LoadVector128(pRight)));
				AdvSimd.Store(&left.M41, AdvSimd.Add(AdvSimd.LoadVector128(&left.M41), AdvSimd.LoadVector128(pRight)));
				return left;
			}
		}
		else if (Sse.IsSupported)
		{
			Span<float> span = stackalloc float[4];
			fixed (float* pRight = span)
			{
				pRight[0] = right;
				pRight[1] = right;
				pRight[2] = right;
				pRight[3] = right;
				Sse.Store(&left.M11, Sse.Add(Sse.LoadVector128(&left.M11), Sse.LoadVector128(pRight)));
				Sse.Store(&left.M21, Sse.Add(Sse.LoadVector128(&left.M21), Sse.LoadVector128(pRight)));
				Sse.Store(&left.M31, Sse.Add(Sse.LoadVector128(&left.M31), Sse.LoadVector128(pRight)));
				Sse.Store(&left.M41, Sse.Add(Sse.LoadVector128(&left.M41), Sse.LoadVector128(pRight)));
				return left;
			}
		}

		Matrix4x4<float> m;

		m.M11 = left.M11 + right;
		m.M12 = left.M12 + right;
		m.M13 = left.M13 + right;
		m.M14 = left.M14 + right;
		m.M21 = left.M21 + right;
		m.M22 = left.M22 + right;
		m.M23 = left.M23 + right;
		m.M24 = left.M24 + right;
		m.M31 = left.M31 + right;
		m.M32 = left.M32 + right;
		m.M33 = left.M33 + right;
		m.M34 = left.M34 + right;
		m.M41 = left.M41 + right;
		m.M42 = left.M42 + right;
		m.M43 = left.M43 + right;
		m.M44 = left.M44 + right;

		return m;
	}

	/// <summary>Adds each element in one matrix with its corresponding element in a second matrix.</summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second matrix.</param>
	/// <returns>The matrix that contains the summed values of <paramref name="left" /> and <paramref name="right" />.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> Add<T>(Matrix4x4<T> left, Matrix4x4<T> right)
		where T : unmanaged, INumberBase<T>
	{
		return left + right;
	}

	/// <summary>Adds <paramref name="right"/> value in each element of a second matrix.</summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second value.</param>
	/// <returns>The matrix that contains the summed values of <paramref name="left" /> and <paramref name="right" />.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> Add<T>(Matrix4x4<T> left, T right)
		where T : unmanaged, INumberBase<T>
	{
		return left + right;
	}

	/// <summary>
	/// Tries to invert the specified matrix. The return value indicates whether the operation succeeded.
	/// </summary>
	/// <param name="matrix">The matrix to invert.</param>
	/// <param name="result">When this method returns, contains the inverted matrix if the operation succeeded.</param>
	/// <returns><see langword="true" /> if <paramref name="matrix" /> was converted successfully; otherwise,  <see langword="false" />.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static unsafe bool Invert(Matrix4x4<float> matrix, out Matrix4x4<float> result)
	{
		bool p = System.Numerics.Matrix4x4.Invert(matrix.ConvertToSystem(), out System.Numerics.Matrix4x4 mat);

		result = mat.ConvertToGeneric();

		return p;
	}

	/// <summary>
	/// Tries to invert the specified matrix. The return value indicates whether the operation succeeded.
	/// </summary>
	/// <param name="matrix">The matrix to invert.</param>
	/// <param name="result">When this method returns, contains the inverted matrix if the operation succeeded.</param>
	/// <returns><see langword="true" /> if <paramref name="matrix" /> was converted successfully; otherwise,  <see langword="false" />.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static unsafe bool Invert<T>(Matrix4x4<T> matrix, out Matrix4x4<T> result)
		where T : unmanaged, INumberBase<T>, IFloatingPointIeee754<T>
	{
		T a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
		T e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
		T i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
		T m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

		T kp_lo = k * p - l * o;
		T jp_ln = j * p - l * n;
		T jo_kn = j * o - k * n;
		T ip_lm = i * p - l * m;
		T io_km = i * o - k * m;
		T in_jm = i * n - j * m;

		T a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
		T a12 = -(e * kp_lo - g * ip_lm + h * io_km);
		T a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
		T a14 = -(e * jo_kn - f * io_km + g * in_jm);

		T det = a * a11 + b * a12 + c * a13 + d * a14;

		if (T.Abs(det) < T.Epsilon)
		{
			result = new Matrix4x4<T>(T.NaN, T.NaN, T.NaN, T.NaN,
								   T.NaN, T.NaN, T.NaN, T.NaN,
								   T.NaN, T.NaN, T.NaN, T.NaN,
								   T.NaN, T.NaN, T.NaN, T.NaN);
			return false;
		}

		T invDet = T.One / det;

		result.M11 = a11 * invDet;
		result.M21 = a12 * invDet;
		result.M31 = a13 * invDet;
		result.M41 = a14 * invDet;

		result.M12 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
		result.M22 = +(a * kp_lo - c * ip_lm + d * io_km) * invDet;
		result.M32 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
		result.M42 = +(a * jo_kn - b * io_km + c * in_jm) * invDet;

		T gp_ho = g * p - h * o;
		T fp_hn = f * p - h * n;
		T fo_gn = f * o - g * n;
		T ep_hm = e * p - h * m;
		T eo_gm = e * o - g * m;
		T en_fm = e * n - f * m;

		result.M13 = +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
		result.M23 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
		result.M33 = +(a * fp_hn - b * ep_hm + d * en_fm) * invDet;
		result.M43 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

		T gl_hk = g * l - h * k;
		T fl_hj = f * l - h * j;
		T fk_gj = f * k - g * j;
		T el_hi = e * l - h * i;
		T ek_gi = e * k - g * i;
		T ej_fi = e * j - f * i;

		result.M14 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
		result.M24 = +(a * gl_hk - c * el_hi + d * ek_gi) * invDet;
		result.M34 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
		result.M44 = +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet;

		return true;
	}

	/// <summary>
	/// Multiplies two matrices together to compute the product.
	/// </summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second matrix.</param>
	/// <returns>The product matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<float> Multiply(Matrix4x4<float> left, Matrix4x4<float> right)
	{
		if (AdvSimd.Arm64.IsSupported)
		{
			Unsafe.SkipInit(out Matrix4x4<float> result);

			// Perform the operation on the first row

			Vector128<float> M11 = AdvSimd.LoadVector128(&left.M11);

			Vector128<float> vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), M11, 0);
			Vector128<float> vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M21), M11, 1);
			Vector128<float> vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M31), M11, 2);
			Vector128<float> vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M41), M11, 3);

			AdvSimd.Store(&result.M11, AdvSimd.Add(vZ, vW));

			// Repeat for the other 3 rows

			Vector128<float> M21 = AdvSimd.LoadVector128(&left.M21);

			vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), M21, 0);
			vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M21), M21, 1);
			vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M31), M21, 2);
			vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M41), M21, 3);

			AdvSimd.Store(&result.M21, AdvSimd.Add(vZ, vW));

			Vector128<float> M31 = AdvSimd.LoadVector128(&left.M31);

			vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), M31, 0);
			vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M21), M31, 1);
			vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M31), M31, 2);
			vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M41), M31, 3);

			AdvSimd.Store(&result.M31, AdvSimd.Add(vZ, vW));

			Vector128<float> M41 = AdvSimd.LoadVector128(&left.M41);

			vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), M41, 0);
			vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M21), M41, 1);
			vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M31), M41, 2);
			vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M41), M41, 3);

			AdvSimd.Store(&result.M41, AdvSimd.Add(vZ, vW));

			return result;
		}
		else if (Sse.IsSupported)
		{
			Vector128<float> row = Sse.LoadVector128(&left.M11);
			Sse.Store(&left.M11,
				Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
								Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M21))),
						Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M31)),
								Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M41)))));

			// 0x00 is _MM_SHUFFLE(0,0,0,0), 0x55 is _MM_SHUFFLE(1,1,1,1), etc.
			// TODO: Replace with a method once it's added to the API.

			row = Sse.LoadVector128(&left.M21);
			Sse.Store(&left.M21,
				Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
								Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M21))),
						Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M31)),
								Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M41)))));

			row = Sse.LoadVector128(&left.M31);
			Sse.Store(&left.M31,
				Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
								Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M21))),
						Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M31)),
								Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M41)))));

			row = Sse.LoadVector128(&left.M41);
			Sse.Store(&left.M41,
				Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
								Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M21))),
						Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M31)),
								Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M41)))));
			return left;
		}

		Matrix4x4<float> m;

		// First row
		m.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
		m.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
		m.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
		m.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

		// Second row
		m.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
		m.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
		m.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
		m.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

		// Third row
		m.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
		m.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
		m.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
		m.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

		// Fourth row
		m.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
		m.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
		m.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
		m.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

		return m;
	}

	/// <summary>
	/// Multiplies a matrix by a float to compute the product.
	/// </summary>
	/// <param name="left">The matrix to scale.</param>
	/// <param name="right">The scaling value to use.</param>
	/// <returns>The scaled matrix.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<float> Multiply(Matrix4x4<float> left, float right)
	{
		if (AdvSimd.IsSupported)
		{
			Vector128<float> rightVec = Vector128.Create(right);
			AdvSimd.Store(&left.M11, AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M11), rightVec));
			AdvSimd.Store(&left.M21, AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M21), rightVec));
			AdvSimd.Store(&left.M31, AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M31), rightVec));
			AdvSimd.Store(&left.M41, AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M41), rightVec));
			return left;
		}
		else if (Sse.IsSupported)
		{
			Vector128<float> rightVec = Vector128.Create(right);
			Sse.Store(&left.M11, Sse.Multiply(Sse.LoadVector128(&left.M11), rightVec));
			Sse.Store(&left.M21, Sse.Multiply(Sse.LoadVector128(&left.M21), rightVec));
			Sse.Store(&left.M31, Sse.Multiply(Sse.LoadVector128(&left.M31), rightVec));
			Sse.Store(&left.M41, Sse.Multiply(Sse.LoadVector128(&left.M41), rightVec));
			return left;
		}

		Unsafe.SkipInit(out Matrix4x4<float> m);

		// First row
		m.M11 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;
		m.M12 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;
		m.M13 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;
		m.M14 = left.M11 * right + left.M12 * right + left.M13 * right + left.M14 * right;

		// Second row
		m.M21 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;
		m.M22 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;
		m.M23 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;
		m.M24 = left.M21 * right + left.M22 * right + left.M23 * right + left.M24 * right;

		// Third row
		m.M31 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;
		m.M32 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;
		m.M33 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;
		m.M34 = left.M31 * right + left.M32 * right + left.M33 * right + left.M34 * right;

		// Fourth row
		m.M41 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;
		m.M42 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;
		m.M43 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;
		m.M44 = left.M41 * right + left.M42 * right + left.M43 * right + left.M44 * right;

		return m;
	}

	/// <summary>
	/// Multiplies two matrices together to compute the product.
	/// </summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second matrix.</param>
	/// <returns>The product matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> Multiply<T>(Matrix4x4<T> left, Matrix4x4<T> right)
		where T : unmanaged, INumberBase<T>
	{
		return left * right;
	}

	/// <summary>
	/// Multiplies a matrix by a float to compute the product.
	/// </summary>
	/// <param name="left">The matrix to scale.</param>
	/// <param name="right">The scaling value to use.</param>
	/// <returns>The scaled matrix.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Matrix4x4<T> Multiply<T>(Matrix4x4<T> left, T right)
		where T : unmanaged, INumberBase<T>
	{
		return left * right;
	}

	/// <summary>
	/// Negates the specified matrix by multiplying all its values by -1.
	/// </summary>
	/// <param name="value">The matrix to negate.</param>
	/// <returns>The negated matrix.</returns>
	[MethodImpl(AggressiveInlining)]
	public static Matrix4x4<T> Negate<T>(Matrix4x4<T> value)
		where T : unmanaged, INumberBase<T>
	{
		return value * -T.One;
	}

	/// <summary>
	/// Subtracts each element in a second matrix from its corresponding element in a first matrix.
	/// </summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second matrix.</param>
	/// <returns>The matrix containing the values that result from subtracting each element in <paramref name="right" /> from its corresponding element in <paramref name="left" />.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<float> Subtract(Matrix4x4<float> left, Matrix4x4<float> right)
	{
		if (AdvSimd.IsSupported)
		{
			AdvSimd.Store(&left.M11, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M11), AdvSimd.LoadVector128(&right.M11)));
			AdvSimd.Store(&left.M21, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M21), AdvSimd.LoadVector128(&right.M21)));
			AdvSimd.Store(&left.M31, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M31), AdvSimd.LoadVector128(&right.M31)));
			AdvSimd.Store(&left.M41, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M41), AdvSimd.LoadVector128(&right.M41)));
			return left;
		}
		else if (Sse.IsSupported)
		{
			Sse.Store(&left.M11, Sse.Subtract(Sse.LoadVector128(&left.M11), Sse.LoadVector128(&right.M11)));
			Sse.Store(&left.M21, Sse.Subtract(Sse.LoadVector128(&left.M21), Sse.LoadVector128(&right.M21)));
			Sse.Store(&left.M31, Sse.Subtract(Sse.LoadVector128(&left.M31), Sse.LoadVector128(&right.M31)));
			Sse.Store(&left.M41, Sse.Subtract(Sse.LoadVector128(&left.M41), Sse.LoadVector128(&right.M41)));
			return left;
		}

		Matrix4x4<float> m;

		m.M11 = left.M11 - right.M11;
		m.M12 = left.M12 - right.M12;
		m.M13 = left.M13 - right.M13;
		m.M14 = left.M14 - right.M14;
		m.M21 = left.M21 - right.M21;
		m.M22 = left.M22 - right.M22;
		m.M23 = left.M23 - right.M23;
		m.M24 = left.M24 - right.M24;
		m.M31 = left.M31 - right.M31;
		m.M32 = left.M32 - right.M32;
		m.M33 = left.M33 - right.M33;
		m.M34 = left.M34 - right.M34;
		m.M41 = left.M41 - right.M41;
		m.M42 = left.M42 - right.M42;
		m.M43 = left.M43 - right.M43;
		m.M44 = left.M44 - right.M44;

		return m;
	}

	/// <summary>
	/// Subtracts second value from its corresponding element in a first matrix.
	/// </summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second value.</param>
	/// <returns>The matrix containing the values that result from subtracting <paramref name="right" /> value from its corresponding element in <paramref name="left" />.</returns>
	[MethodImpl(AggressiveOptimization)]
	public static Matrix4x4<float> Subtract(Matrix4x4<float> left, float right)
	{
		if (AdvSimd.IsSupported)
		{
			Span<float> span = stackalloc float[4];
			fixed (float* pRight = span)
			{
				pRight[0] = right;
				pRight[1] = right;
				pRight[2] = right;
				pRight[3] = right;
				AdvSimd.Store(&left.M11, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M11), AdvSimd.LoadVector128(pRight)));
				AdvSimd.Store(&left.M21, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M21), AdvSimd.LoadVector128(pRight)));
				AdvSimd.Store(&left.M31, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M31), AdvSimd.LoadVector128(pRight)));
				AdvSimd.Store(&left.M41, AdvSimd.Subtract(AdvSimd.LoadVector128(&left.M41), AdvSimd.LoadVector128(pRight)));
				return left;
			}
		}
		else if (Sse.IsSupported)
		{
			Span<float> span = stackalloc float[4];
			fixed (float* pRight = span)
			{
				pRight[0] = right;
				pRight[1] = right;
				pRight[2] = right;
				pRight[3] = right;
				Sse.Store(&left.M11, Sse.Subtract(Sse.LoadVector128(&left.M11), Sse.LoadVector128(pRight)));
				Sse.Store(&left.M21, Sse.Subtract(Sse.LoadVector128(&left.M21), Sse.LoadVector128(pRight)));
				Sse.Store(&left.M31, Sse.Subtract(Sse.LoadVector128(&left.M31), Sse.LoadVector128(pRight)));
				Sse.Store(&left.M41, Sse.Subtract(Sse.LoadVector128(&left.M41), Sse.LoadVector128(pRight)));
				return left;
			}
		}

		Matrix4x4<float> m;

		m.M11 = left.M11 - right;
		m.M12 = left.M12 - right;
		m.M13 = left.M13 - right;
		m.M14 = left.M14 - right;
		m.M21 = left.M21 - right;
		m.M22 = left.M22 - right;
		m.M23 = left.M23 - right;
		m.M24 = left.M24 - right;
		m.M31 = left.M31 - right;
		m.M32 = left.M32 - right;
		m.M33 = left.M33 - right;
		m.M34 = left.M34 - right;
		m.M41 = left.M41 - right;
		m.M42 = left.M42 - right;
		m.M43 = left.M43 - right;
		m.M44 = left.M44 - right;

		return m;
	}

	/// <summary>
	/// Subtracts each element in a second matrix from its corresponding element in a first matrix.
	/// </summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second matrix.</param>
	/// <returns>The matrix containing the values that result from subtracting each element in <paramref name="right" /> from its corresponding element in <paramref name="left" />.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> Subtract<T>(Matrix4x4<T> left, Matrix4x4<T> right)
		where T : unmanaged, INumberBase<T>
	{
		return left - right;
	}

	/// <summary>
	/// Subtracts second value from its corresponding element in a first matrix.
	/// </summary>
	/// <param name="left">The first matrix.</param>
	/// <param name="right">The second value.</param>
	/// <returns>The matrix containing the values that result from subtracting <paramref name="right" /> value from its corresponding element in <paramref name="left" />.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> Subtract<T>(Matrix4x4<T> left, T right)
		where T : unmanaged, INumberBase<T>
	{
		return left - right;
	}

	[MethodImpl(AggressiveInlining)]
	private static Vector128<float> Permute(Vector128<float> value, byte control)
	{
		if (Avx.IsSupported)
		{
			return Avx.Permute(value, control);
		}
		else if (Sse.IsSupported)
		{
			return Sse.Shuffle(value, value, control);
		}
		else
		{
			// Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
			throw new PlatformNotSupportedException();
		}
	}

	/// <summary>Attempts to extract the scale, translation, and rotation components from the given scale, rotation, or translation matrix. The return value indicates whether the operation succeeded.</summary>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="scale">When this method returns, contains the scaling component of the transformation matrix if the operation succeeded.</param>
	/// <param name="rotation">When this method returns, contains the rotation component of the transformation matrix if the operation succeeded.</param>
	/// <param name="translation">When the method returns, contains the translation component of the transformation matrix if the operation succeeded.</param>
	/// <returns><see langword="true" /> if <paramref name="matrix" /> was decomposed successfully; otherwise, <see langword="false" />.</returns>
	[NotImplementYet]
	[Obsolete(nameof(NotImplementYetAttribute))]
	public static bool Decompose(Matrix4x4<float> matrix, out Vector3d<float> scale, out Quaternion<float> rotation, out Vector3d<float> translation)
	{
		bool p = System.Numerics.Matrix4x4.Decompose(matrix.ConvertToSystem(), out Vector3 scale2, out System.Numerics.Quaternion rotation2, out Vector3 translation2);

		scale = scale2.ConvertToGeneric();
		rotation = Unsafe.As<System.Numerics.Quaternion, Quaternion<float>>(ref rotation2);
		translation = translation2.ConvertToGeneric();

		return p;
	}

	/// <summary>Attempts to extract the scale, translation, and rotation components from the given scale, rotation, or translation matrix. The return value indicates whether the operation succeeded.</summary>
	/// <param name="matrix">The source matrix</param>
	/// <param name="scale">When this method returns, contains the scaling component of the transformation matrix if the operation succeeded.</param>
	/// <param name="rotation">When this method returns, contains the rotation component of the transformation matrix if the operation succeeded.</param>
	/// <param name="translation">When the method returns, contains the translation component of the transformation matrix if the operation succeeded.</param>
	/// <returns><see langword="true" /> if <paramref name="matrix" /> was decomposed successfully; otherwise, <see langword="false" />.</returns>
	[NotImplementYet]
	[Obsolete(nameof(NotImplementYetAttribute))]
	public static bool Decompose<T>(Matrix4x4<T> matrix, out Vector3d<T> scale, out Quaternion<T> rotation, out Vector3d<T> translation)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>, IComparisonOperators<T, T, bool>, IComparisonOperators<T, float, bool>
	{
		bool result = true;

		unsafe
		{
			fixed (Vector3d<T>* scaleBase = &scale)
			{
				T* pfScales = (T*)scaleBase;
				T det;

				VectorBasis<T> vectorBasis;
				Vector3d<T>** pVectorBasis = (Vector3d<T>**)&vectorBasis;

				Matrix4x4<T> matTemp = Matrix4x4<T>.Identity;
				CanonicalBasis<T> canonicalBasis = default;
				Vector3d<T>* pCanonicalBasis = &canonicalBasis.Row0;

				canonicalBasis.Row0 = new(T.One, T.Zero, T.Zero);
				canonicalBasis.Row1 = new(T.Zero, T.One, T.Zero);
				canonicalBasis.Row2 = new(T.Zero, T.Zero, T.One);

				translation = new Vector3d<T>(
					matrix.M41,
					matrix.M42,
					matrix.M43);

				pVectorBasis[0] = (Vector3d<T>*)&matTemp.M11;
				pVectorBasis[1] = (Vector3d<T>*)&matTemp.M21;
				pVectorBasis[2] = (Vector3d<T>*)&matTemp.M31;

				*pVectorBasis[0] = new Vector3d<T>(matrix.M11, matrix.M12, matrix.M13);
				*pVectorBasis[1] = new Vector3d<T>(matrix.M21, matrix.M22, matrix.M23);
				*pVectorBasis[2] = new Vector3d<T>(matrix.M31, matrix.M32, matrix.M33);

				scale.X = Vector3d.Length(*pVectorBasis[0]);
				scale.Y = Vector3d.Length(*pVectorBasis[1]);
				scale.Z = Vector3d.Length(*pVectorBasis[2]);

				uint a, b, c;
				#region Ranking
				T x = pfScales[0], y = pfScales[1], z = pfScales[2];
				if (x < y)
				{
					if (y < z)
					{
						a = 2;
						b = 1;
						c = 0;
					}
					else
					{
						a = 1;

						if (x < z)
						{
							b = 2;
							c = 0;
						}
						else
						{
							b = 0;
							c = 2;
						}
					}
				}
				else
				{
					if (x < z)
					{
						a = 2;
						b = 0;
						c = 1;
					}
					else
					{
						a = 0;

						if (y < z)
						{
							b = 2;
							c = 1;
						}
						else
						{
							b = 1;
							c = 2;
						}
					}
				}
				#endregion

				if (pfScales[a] < DecomposeEpsilon)
				{
					*pVectorBasis[a] = pCanonicalBasis[a];
				}

				*pVectorBasis[a] = Vector3d.Normalize(*pVectorBasis[a]);

				if (pfScales[b] < DecomposeEpsilon)
				{
					uint cc;
					T fAbsX, fAbsY, fAbsZ;

					fAbsX = T.Abs(pVectorBasis[a]->X);
					fAbsY = T.Abs(pVectorBasis[a]->Y);
					fAbsZ = T.Abs(pVectorBasis[a]->Z);

					#region Ranking
					if (fAbsX < fAbsY)
					{
						if (fAbsY < fAbsZ)
						{
							cc = 0;
						}
						else
						{
							if (fAbsX < fAbsZ)
							{
								cc = 0;
							}
							else
							{
								cc = 2;
							}
						}
					}
					else
					{
						if (fAbsX < fAbsZ)
						{
							cc = 1;
						}
						else
						{
							if (fAbsY < fAbsZ)
							{
								cc = 1;
							}
							else
							{
								cc = 2;
							}
						}
					}
					#endregion

					*pVectorBasis[b] = Vector3d.Cross(*pVectorBasis[a], *(pCanonicalBasis + cc));
				}

				*pVectorBasis[b] = Vector3d.Normalize(*pVectorBasis[b]);

				if (pfScales[c] < DecomposeEpsilon)
				{
					*pVectorBasis[c] = Vector3d.Cross(*pVectorBasis[a], *pVectorBasis[b]);
				}

				*pVectorBasis[c] = Vector3d.Normalize(*pVectorBasis[c]);

				det = matTemp.GetDeterminant();

				// use Kramer's rule to check for handedness of coordinate system
				if (det < 0.0f)
				{
					// switch coordinate system by negating the scale and inverting the basis vector on the x-axis
					pfScales[a] = -pfScales[a];
					*pVectorBasis[a] = -*pVectorBasis[a];

					det = -det;
				}

				det -= T.One;
				det *= det;

				if (FloatScalar<T>.DecomposeEpsilon < det)
				{
					// Non-SRT matrix encountered
					rotation = Quaternion<T>.Identity;
					result = false;
				}
				else
				{
					// generate the quaternion from the matrix
					rotation = Geometry.Quaternion.CreateFromRotationMatrix(matTemp);
				}
			}
		}

		return result;
	}

	private struct CanonicalBasis<T>
		where T : unmanaged, INumberBase<T>
	{
		public Vector3d<T> Row0;
		public Vector3d<T> Row1;
		public Vector3d<T> Row2;
	};

#pragma warning disable CS0649 // Element never use
	private struct VectorBasis<T>
		where T : unmanaged, INumberBase<T>
	{
		public unsafe Vector3d<T>* Element0;
		public unsafe Vector3d<T>* Element1;
		public unsafe Vector3d<T>* Element2;
	}
#pragma warning restore
}