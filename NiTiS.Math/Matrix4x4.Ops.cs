using NiTiS.Core.Annotations;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Security.Principal;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static unsafe class Matrix4x4
{
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> ConvertToGeneric(this System.Numerics.Matrix4x4 matrix)
		=> Unsafe.As<System.Numerics.Matrix4x4, Matrix4x4<float>>(ref matrix);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static System.Numerics.Matrix4x4 ConvertToSystem(this Matrix4x4<float> matrix)
		=> Unsafe.As<Matrix4x4<float>, System.Numerics.Matrix4x4>(ref matrix);

	private const float BillboardEpsilon = 1e-4f;
	private const float BillboardMinAngle = 1.0f - (0.1f * (MathF.PI / 180.0f)); // 0.1 degrees
	private const float DecomposeEpsilon = 0.0001f;


	/// <summary>
	/// Creates a spherical billboard that rotates around a specified object position
	/// </summary>
	/// <param name="ojbectPosition">The position of the object that the billboard will rotate around</param>
	/// <param name="cameraPosition">The position of the camera</param>
	/// <param name="cameraUp">The up vector of the camera</param>
	/// <param name="cameraForward">The forward vector of the camera</param>
	/// <returns>The created billboard</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> CreateBillboard(Vector3D<float> ojbectPosition, Vector3D<float> cameraPosition, Vector3D<float> cameraUp, Vector3D<float> cameraForward)
	{
		Vector3D<float> zaxis = ojbectPosition - cameraPosition;
		float norm = zaxis.LengthSquared;

		if (norm < BillboardEpsilon)
		{
			zaxis = -cameraForward;
		}
		else
		{
			zaxis = Vector3D.Multiply(zaxis, 1.0f / MathF.Sqrt(norm));
		}

		Vector3D<float> xaxis = Vector3D.Normalize(Vector3D.Cross(cameraUp, zaxis));
		Vector3D<float> yaxis = Vector3D.Cross(zaxis, xaxis);

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
	/// Creates a cylindrical billboard that rotates around a specified axis
	/// </summary>
	/// <param name="objectPosition">The position of the object that the billboard will rotate around</param>
	/// <param name="cameraPosition">The position of the camera</param>
	/// <param name="rotateAxis">The axis to rotate the billboard around</param>
	/// <param name="cameraForward">The forward vector of the camera</param>
	/// <param name="objectForward">The forward vector of the object</param>
	/// <returns></returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> CreateConstrainedBillboard(Vector3D<float> objectPosition, Vector3D<float> cameraPosition, Vector3D<float> rotateAxis, Vector3D<float> cameraForward, Vector3D<float> objectForward)
	{
		Vector3D<float> faceDir = objectPosition - cameraPosition;
		float norm = faceDir.LengthSquared;

		if (norm < BillboardEpsilon)
		{
			faceDir = -cameraForward;
		}
		else
		{
			faceDir = Vector3D.Multiply(faceDir, (1.0f / MathF.Sqrt(norm)));
		}

		Vector3D<float> yaxis = rotateAxis;
		Vector3D<float> xaxis;
		Vector3D<float> zaxis;

		// Treat the case when angle between faceDir and rotateAxis is too close to 0.
		float dot = Vector3D.Dot(rotateAxis, faceDir);

		if (MathF.Abs(dot) > BillboardMinAngle)
		{
			zaxis = objectForward;

			// Make sure passed values are useful for compute.
			dot = Vector3D.Dot(rotateAxis, zaxis);

			if (MathF.Abs(dot) > BillboardMinAngle)
			{
				zaxis = (MathF.Abs(rotateAxis.Z) > BillboardMinAngle) ? Vector3D<float>.UnitX : -Vector3D<float>.UnitZ;
			}

			xaxis = Vector3D.Normalize(Vector3D.Cross(rotateAxis, zaxis));
			zaxis = Vector3D.Normalize(Vector3D.Cross(xaxis, rotateAxis));
		}
		else
		{
			xaxis = Vector3D.Normalize(Vector3D.Cross(rotateAxis, faceDir));
			zaxis = Vector3D.Normalize(Vector3D.Cross(xaxis, yaxis));
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
	/// Creates a matrix that rotates around an arbitrary vector
	/// </summary>
	/// <param name="axis">The axis to rotate around</param>
	/// <param name="angle">The angle to rotate around axis, in radians</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateFromAxisAngle<T>(Vector3D<T> axis, T angle)
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
	/// Creates a rotation matrix from the specified Quaternion rotation value
	/// </summary>
	/// <param name="quaternion">The source Quaternion</param>
	/// <returns>The rotation matrix</returns>
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

	/// <summary>
	/// Creates a rotation matrix from the specified yaw, pitch, and roll
	/// </summary>
	/// <param name="yaw">The angle of rotation, in radians, around the Y axis</param>
	/// <param name="pitch">The angle of rotation, in radians, around the X axis</param>
	/// <param name="roll">The angle of rotation, in radians, around the Z axis</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	[Obsolete(nameof(NotImplementYetAttribute))]
	[NotImplementYet]
	public static Matrix4x4<T> CreateFromYawPitchRoll<T>(float yaw, float pitch, float roll)
		where T : unmanaged, INumberBase<T>
	{
		Quaternion q = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
		return CreateFromQuaternion(Unsafe.As<Quaternion, Quaternion<T>>(ref q));
	}

	/// <summary>
	/// Creates a view matrix
	/// </summary>
	/// <param name="cameraPosition">The position of the camera</param>
	/// <param name="cameraTarget">The target towards which the camera is pointing</param>
	/// <param name="cameraUpVector">The direction that is "up" from the camera's point of view</param>
	/// <returns>The view matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateLookAt<T>(Vector3D<T> cameraPosition, Vector3D<T> cameraTarget, Vector3D<T> cameraUpVector)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
	{
		Vector3D<T> zaxis = Vector3D.Normalize(cameraPosition - cameraTarget);
		Vector3D<T> xaxis = Vector3D.Normalize(Vector3D.Cross(cameraUpVector, zaxis));
		Vector3D<T> yaxis = Vector3D.Cross(zaxis, xaxis);

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

		result.M41 = -Vector3D.Dot(xaxis, cameraPosition);
		result.M42 = -Vector3D.Dot(yaxis, cameraPosition);
		result.M43 = -Vector3D.Dot(zaxis, cameraPosition);

		return result;
	}

	/// <summary>
	/// Creates an orthographic perspective matrix from the given view volume dimensions
	/// </summary>
	/// <param name="width">The width of the view volume</param>
	/// <param name="height">The height of the view volume</param>
	/// <param name="zNearPlane">The minimum Z-value of the view volume</param>
	/// <param name="zFarPlane">The maximum Z-value of the view volume</param>
	/// <returns>The orthographic projection matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateOrthographic<T>(T width, T height, T zNearPlane, T zFarPlane)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T Two = T.One + T.One;

		result.M11 = Two / width;
		result.M22 = Two / height;
		result.M33 = T.One / (zNearPlane - zFarPlane);
		result.M43 = zNearPlane / (zNearPlane - zFarPlane);

		return result;
	}

	/// <summary>
	/// Creates a customized orthographic projection matrix</summary>
	/// <param name="left">The minimum X-value of the view volume</param>
	/// <param name="right">The maximum X-value of the view volume</param>
	/// <param name="bottom">The minimum Y-value of the view volume</param>
	/// <param name="top">The maximum Y-value of the view volume</param>
	/// <param name="zNearPlane">The minimum Z-value of the view volume</param>
	/// <param name="zFarPlane">The maximum Z-value of the view volume</param>
	/// <returns>The orthographic projection matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateOrthographicOffCenter<T>(T left, T right, T bottom, T top, T zNearPlane, T zFarPlane)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;

		T Two = T.One + T.One;

		result.M11 = Two / (right - left);

		result.M22 = Two / (top - bottom);

		result.M33 = T.One / (zNearPlane - zFarPlane);

		result.M41 = (left + right) / (left - right);
		result.M42 = (top + bottom) / (bottom - top);
		result.M43 = zNearPlane / (zNearPlane - zFarPlane);

		return result;
	}

	/// <summary>
	/// Creates a perspective projection matrix from the given view volume dimensions
	/// </summary>
	/// <param name="width">The width of the view volume at the near view plane</param>
	/// <param name="height">The height of the view volume at the near view plane</param>
	/// <param name="nearPlaneDistance">The distance to the near view plane</param>
	/// <param name="farPlaneDistance">The distance to the far view plane</param>
	/// <returns>The perspective projection matrix</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="nearPlaneDistance" /> is less than or equal to zero
	/// -or-
	/// <paramref name="farPlaneDistance" /> is less than or equal to zero
	/// -or-
	/// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" /></exception>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
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
	/// Creates a perspective projection matrix based on a field of view, aspect ratio, and near and far view plane distances
	/// </summary>
	/// <param name="fieldOfView">The field of view in the y direction, in radians</param>
	/// <param name="aspectRatio">The aspect ratio, defined as view space width divided by height</param>
	/// <param name="nearPlaneDistance">The distance to the near view plane</param>
	/// <param name="farPlaneDistance">The distance to the far view plane</param>
	/// <returns>The perspective projection matrix.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="fieldOfView" /> is less than or equal to zero
	/// -or-
	/// <paramref name="fieldOfView" /> is greater than or equal to <see cref="System.Math.PI" />
	/// <paramref name="nearPlaneDistance" /> is less than or equal to zero
	/// -or-
	/// <paramref name="farPlaneDistance" /> is less than or equal to zero
	/// -or-
	/// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" /></exception>
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
	/// Creates a customized perspective projection matrix
	/// </summary>
	/// <param name="left">The minimum x-value of the view volume at the near view plane</param>
	/// <param name="right">The maximum x-value of the view volume at the near view plane</param>
	/// <param name="bottom">The minimum y-value of the view volume at the near view plane</param>
	/// <param name="top">The maximum y-value of the view volume at the near view plane</param>
	/// <param name="nearPlaneDistance">The distance to the near view plane</param>
	/// <param name="farPlaneDistance">The distance to the far view plane</param>
	/// <returns>The perspective projection matrix</returns>
	/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="nearPlaneDistance" /> is less than or equal to zero
	/// -or-
	/// <paramref name="farPlaneDistance" /> is less than or equal to zero
	/// -or-
	/// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" /></exception>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
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
	/// Creates a matrix that reflects the coordinate system about a specified plane
	/// </summary>
	/// <param name="value">The plane about which to create a reflection</param>
	/// <returns>A new matrix expressing the reflection</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateReflection<T>(Plane<T> value)
		where T : unmanaged, INumberBase<T>
	{
		value = Plane.Normalize(value);

		T a = value.Normal.X;
		T b = value.Normal.Y;
		T c = value.Normal.Z;

		T MinusTwo = -(T.One + T.One);

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
	/// Creates a matrix for rotating points around the X axis
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the X axis</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
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
	/// Creates a matrix for rotating points around the X axis from a center point
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the X axis</param>
	/// <param name="centerPoint">The center point</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateRotationX<T>(T radians, Vector3D<T> centerPoint)
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
	/// Creates a matrix for rotating points around the Y axis
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Y-axis</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
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
	/// The amount, in radians, by which to rotate around the Y axis from a center point
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Y-axis</param>
	/// <param name="centerPoint">The center point</param>
	/// <returns>The rotation matrix.</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateRotationY<T>(T radians, Vector3D<T> centerPoint)
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
	/// Creates a matrix for rotating points around the Z axis
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Z-axis</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
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
	/// Creates a matrix for rotating points around the Z axis from a center point
	/// </summary>
	/// <param name="radians">The amount, in radians, by which to rotate around the Z-axis</param>
	/// <param name="centerPoint">The center point</param>
	/// <returns>The rotation matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateRotationZ<T>(T radians, Vector3D<T> centerPoint)
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
	/// Creates a scaling matrix from the specified X, Y, and Z components
	/// </summary>
	/// <param name="xScale">The value to scale by on the X axis</param>
	/// <param name="yScale">The value to scale by on the Y axis</param>
	/// <param name="zScale">The value to scale by on the Z axis</param>
	/// <returns>The scaling matrix</returns>
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
	/// Creates a scaling matrix that is offset by a given center point
	/// </summary>
	/// <param name="xScale">The value to scale by on the X axis</param>
	/// <param name="yScale">The value to scale by on the Y axis</param>
	/// <param name="zScale">The value to scale by on the Z axis</param>
	/// <param name="centerPoint">The center point</param>
	/// <returns>The scaling matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(T xScale, T yScale, T zScale, Vector3D<T> centerPoint)
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
	/// Creates a scaling matrix from the specified vector scale
	/// </summary>
	/// <param name="scales">The scale to use</param>
	/// <returns>The scaling matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(Vector3D<T> scales)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;
		result.M11 = scales.X;
		result.M22 = scales.Y;
		result.M33 = scales.Z;
		return result;
	}

	/// <summary>
	/// Creates a scaling matrix with a center point
	/// </summary>
	/// <param name="scales">The vector that contains the amount to scale on each axis</param>
	/// <param name="centerPoint">The center point</param>
	/// <returns>The scaling matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(Vector3D<T> scales, Vector3D<T> centerPoint)
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
	/// Creates a uniform scaling matrix that scale equally on each axis
	/// </summary>
	/// <param name="scale">The uniform scaling factor</param>
	/// <returns>The scaling matrix</returns>
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
	/// Creates a uniform scaling matrix that scales equally on each axis with a center point
	/// </summary>
	/// <param name="scale">The uniform scaling factor</param>
	/// <param name="centerPoint">The center point</param>
	/// <returns>The scaling matrix</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateScale<T>(T scale, Vector3D<T> centerPoint)
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
	/// Creates a matrix that flattens geometry into a specified plane as if casting a shadow from a specified light source</summary>
	/// <param name="lightDirection">The direction from which the light that will cast the shadow is coming</param>
	/// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow</param>
	/// <returns>A new matrix that can be used to flatten geometry onto the specified plane from the specified direction</returns>

	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateShadow<T>(Vector3D<T> lightDirection, Plane<T> plane)
		where T : unmanaged, INumberBase<T>
	{
		Plane<T> p = Plane.Normalize(plane);

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
	/// Creates a translation matrix from the specified 3-dimensional vector
	/// </summary>
	/// <param name="position">The amount to translate in each axis</param>
	/// <returns>The translation matrix</returns>
	public static Matrix4x4<T> CreateTranslation<T>(Vector3D<T> position)
		where T : unmanaged, INumberBase<T>
	{
		Matrix4x4<T> result = Matrix4x4<T>.Identity;
		result.M41 = position.X;
		result.M42 = position.Y;
		result.M43 = position.Z;
		return result;
	}

	/// <summary>
	/// Creates a translation matrix from the specified X, Y, and Z components
	/// </summary>
	/// <param name="xPosition">The amount to translate on the X axis</param>
	/// <param name="yPosition">The amount to translate on the Y axis</param>
	/// <param name="zPosition">The amount to translate on the Z axis</param>
	/// <returns>The translation matrix</returns>
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
	/// Creates a world matrix with the specified parameters
	/// </summary>
	/// <param name="position">The position of the object</param>
	/// <param name="forward">The forward direction of the object</param>
	/// <param name="up">The upward direction of the object. Its value is usually <c>[0, 1, 0]</c>.</param>
	/// <returns>The world matrix.</returns>
	/// <remarks><paramref name="position" /> is used in translation operations.</remarks>
	public static Matrix4x4<T> CreateWorld<T>(Vector3D<T> position, Vector3D<T> forward, Vector3D<T> up)
		where T : unmanaged, INumberBase<T>, IRootFunctions<T>
	{
		Vector3D<T> zaxis = Vector3D.Normalize(-forward);
		Vector3D<T> xaxis = Vector3D.Normalize(Vector3D.Cross(up, zaxis));
		Vector3D<T> yaxis = Vector3D.Cross(zaxis, xaxis);

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

	/// <summary>
	/// Tries to invert the specified matrix. The return value indicates whether the operation succeeded
	/// </summary>
	/// <param name="matrix">The matrix to invert</param>
	/// <param name="result">When this method returns, contains the inverted matrix if the operation succeeded</param>
	/// <returns><see langword="true" /> if <paramref name="matrix" /> was converted successfully; otherwise,  <see langword="false" /></returns>
	[MethodImpl(AggressiveInlining)]
	public static unsafe bool Invert<T>(Matrix4x4<T> matrix, out Matrix4x4<T> result)
		where T : unmanaged, INumberBase<T>, IFloatingPointIeee754<T>
	{
		// This implementation is based on the DirectX Math Library XMMatrixInverse method
		// https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathMatrix.inl

		if (Sse.IsSupported && matrix is Matrix4x4<float> matrixF)
		{
			bool x = SseImpl(matrixF, out Matrix4x4<float> output);

			result = Unsafe.As< Matrix4x4<float>, Matrix4x4<T>>(ref output);
			return x;
		}

		return SoftwareFallback(matrix, out result);

		static unsafe bool SseImpl(Matrix4x4<float> matrix, out Matrix4x4<float> result)
		{
			if (!Sse.IsSupported)
			{
				// Redundant test so we won't prejit remainder of this method on platforms without SSE.
				throw new PlatformNotSupportedException();
			}

			// Load the matrix values into rows
			Vector128<float> row1 = Sse.LoadVector128(&matrix.M11);
			Vector128<float> row2 = Sse.LoadVector128(&matrix.M21);
			Vector128<float> row3 = Sse.LoadVector128(&matrix.M31);
			Vector128<float> row4 = Sse.LoadVector128(&matrix.M41);

			// Transpose the matrix
			Vector128<float> vTemp1 = Sse.Shuffle(row1, row2, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
			Vector128<float> vTemp3 = Sse.Shuffle(row1, row2, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
			Vector128<float> vTemp2 = Sse.Shuffle(row3, row4, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
			Vector128<float> vTemp4 = Sse.Shuffle(row3, row4, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)

			row1 = Sse.Shuffle(vTemp1, vTemp2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
			row2 = Sse.Shuffle(vTemp1, vTemp2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
			row3 = Sse.Shuffle(vTemp3, vTemp4, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
			row4 = Sse.Shuffle(vTemp3, vTemp4, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

			Vector128<float> V00 = Permute(row3, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
			Vector128<float> V10 = Permute(row4, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
			Vector128<float> V01 = Permute(row1, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
			Vector128<float> V11 = Permute(row2, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
			Vector128<float> V02 = Sse.Shuffle(row3, row1, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
			Vector128<float> V12 = Sse.Shuffle(row4, row2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

			Vector128<float> D0 = Sse.Multiply(V00, V10);
			Vector128<float> D1 = Sse.Multiply(V01, V11);
			Vector128<float> D2 = Sse.Multiply(V02, V12);

			V00 = Permute(row3, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
			V10 = Permute(row4, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
			V01 = Permute(row1, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
			V11 = Permute(row2, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
			V02 = Sse.Shuffle(row3, row1, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
			V12 = Sse.Shuffle(row4, row2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)

			// Note:  We use this expansion pattern instead of Fused Multiply Add
			// in order to support older hardware
			D0 = Sse.Subtract(D0, Sse.Multiply(V00, V10));
			D1 = Sse.Subtract(D1, Sse.Multiply(V01, V11));
			D2 = Sse.Subtract(D2, Sse.Multiply(V02, V12));

			// V11 = D0Y,D0W,D2Y,D2Y
			V11 = Sse.Shuffle(D0, D2, 0x5D);  //_MM_SHUFFLE(1, 1, 3, 1)
			V00 = Permute(row2, 0x49);        //_MM_SHUFFLE(1, 0, 2, 1)
			V10 = Sse.Shuffle(V11, D0, 0x32); //_MM_SHUFFLE(0, 3, 0, 2)
			V01 = Permute(row1, 0x12);        //_MM_SHUFFLE(0, 1, 0, 2)
			V11 = Sse.Shuffle(V11, D0, 0x99); //_MM_SHUFFLE(2, 1, 2, 1)

			// V13 = D1Y,D1W,D2W,D2W
			Vector128<float> V13 = Sse.Shuffle(D1, D2, 0xFD); //_MM_SHUFFLE(3, 3, 3, 1)
			V02 = Permute(row4, 0x49);                        //_MM_SHUFFLE(1, 0, 2, 1)
			V12 = Sse.Shuffle(V13, D1, 0x32);                 //_MM_SHUFFLE(0, 3, 0, 2)
			Vector128<float> V03 = Permute(row3, 0x12);       //_MM_SHUFFLE(0, 1, 0, 2)
			V13 = Sse.Shuffle(V13, D1, 0x99);                 //_MM_SHUFFLE(2, 1, 2, 1)

			Vector128<float> C0 = Sse.Multiply(V00, V10);
			Vector128<float> C2 = Sse.Multiply(V01, V11);
			Vector128<float> C4 = Sse.Multiply(V02, V12);
			Vector128<float> C6 = Sse.Multiply(V03, V13);

			// V11 = D0X,D0Y,D2X,D2X
			V11 = Sse.Shuffle(D0, D2, 0x4);    //_MM_SHUFFLE(0, 0, 1, 0)
			V00 = Permute(row2, 0x9e);         //_MM_SHUFFLE(2, 1, 3, 2)
			V10 = Sse.Shuffle(D0, V11, 0x93);  //_MM_SHUFFLE(2, 1, 0, 3)
			V01 = Permute(row1, 0x7b);         //_MM_SHUFFLE(1, 3, 2, 3)
			V11 = Sse.Shuffle(D0, V11, 0x26);  //_MM_SHUFFLE(0, 2, 1, 2)

			// V13 = D1X,D1Y,D2Z,D2Z
			V13 = Sse.Shuffle(D1, D2, 0xa4);   //_MM_SHUFFLE(2, 2, 1, 0)
			V02 = Permute(row4, 0x9e);         //_MM_SHUFFLE(2, 1, 3, 2)
			V12 = Sse.Shuffle(D1, V13, 0x93);  //_MM_SHUFFLE(2, 1, 0, 3)
			V03 = Permute(row3, 0x7b);         //_MM_SHUFFLE(1, 3, 2, 3)
			V13 = Sse.Shuffle(D1, V13, 0x26);  //_MM_SHUFFLE(0, 2, 1, 2)

			C0 = Sse.Subtract(C0, Sse.Multiply(V00, V10));
			C2 = Sse.Subtract(C2, Sse.Multiply(V01, V11));
			C4 = Sse.Subtract(C4, Sse.Multiply(V02, V12));
			C6 = Sse.Subtract(C6, Sse.Multiply(V03, V13));

			V00 = Permute(row2, 0x33); //_MM_SHUFFLE(0, 3, 0, 3)

			// V10 = D0Z,D0Z,D2X,D2Y
			V10 = Sse.Shuffle(D0, D2, 0x4A); //_MM_SHUFFLE(1, 0, 2, 2)
			V10 = Permute(V10, 0x2C);        //_MM_SHUFFLE(0, 2, 3, 0)
			V01 = Permute(row1, 0x8D);       //_MM_SHUFFLE(2, 0, 3, 1)

			// V11 = D0X,D0W,D2X,D2Y
			V11 = Sse.Shuffle(D0, D2, 0x4C); //_MM_SHUFFLE(1, 0, 3, 0)
			V11 = Permute(V11, 0x93);        //_MM_SHUFFLE(2, 1, 0, 3)
			V02 = Permute(row4, 0x33);       //_MM_SHUFFLE(0, 3, 0, 3)

			// V12 = D1Z,D1Z,D2Z,D2W
			V12 = Sse.Shuffle(D1, D2, 0xEA); //_MM_SHUFFLE(3, 2, 2, 2)
			V12 = Permute(V12, 0x2C);        //_MM_SHUFFLE(0, 2, 3, 0)
			V03 = Permute(row3, 0x8D);       //_MM_SHUFFLE(2, 0, 3, 1)

			// V13 = D1X,D1W,D2Z,D2W
			V13 = Sse.Shuffle(D1, D2, 0xEC); //_MM_SHUFFLE(3, 2, 3, 0)
			V13 = Permute(V13, 0x93);        //_MM_SHUFFLE(2, 1, 0, 3)

			V00 = Sse.Multiply(V00, V10);
			V01 = Sse.Multiply(V01, V11);
			V02 = Sse.Multiply(V02, V12);
			V03 = Sse.Multiply(V03, V13);

			Vector128<float> C1 = Sse.Subtract(C0, V00);
			C0 = Sse.Add(C0, V00);
			Vector128<float> C3 = Sse.Add(C2, V01);
			C2 = Sse.Subtract(C2, V01);
			Vector128<float> C5 = Sse.Subtract(C4, V02);
			C4 = Sse.Add(C4, V02);
			Vector128<float> C7 = Sse.Add(C6, V03);
			C6 = Sse.Subtract(C6, V03);

			C0 = Sse.Shuffle(C0, C1, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
			C2 = Sse.Shuffle(C2, C3, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
			C4 = Sse.Shuffle(C4, C5, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
			C6 = Sse.Shuffle(C6, C7, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

			C0 = Permute(C0, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
			C2 = Permute(C2, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
			C4 = Permute(C4, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
			C6 = Permute(C6, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

			// Get the determinant
			vTemp2 = row1;
			float det = Vector4.Dot(C0.AsVector4(), vTemp2.AsVector4());

			// Check determinate is not zero
			if (MathF.Abs(det) < float.Epsilon)
			{
				result = new Matrix4x4<float>(float.NaN, float.NaN, float.NaN, float.NaN,
							float.NaN, float.NaN, float.NaN, float.NaN,
							float.NaN, float.NaN, float.NaN, float.NaN,
							float.NaN, float.NaN, float.NaN, float.NaN);
				return false;
			}

			// Create Vector128<float> copy of the determinant and invert them.
			Vector128<float> ones = Vector128.Create(1.0f);
			Vector128<float> vTemp = Vector128.Create(det);
			vTemp = Sse.Divide(ones, vTemp);

			row1 = Sse.Multiply(C0, vTemp);
			row2 = Sse.Multiply(C2, vTemp);
			row3 = Sse.Multiply(C4, vTemp);
			row4 = Sse.Multiply(C6, vTemp);

			Unsafe.SkipInit(out result);
			ref Vector128<float> vResult = ref Unsafe.As<Matrix4x4<float>, Vector128<float>>(ref result);

			vResult = row1;
			Unsafe.Add(ref vResult, 1) = row2;
			Unsafe.Add(ref vResult, 2) = row3;
			Unsafe.Add(ref vResult, 3) = row4;

			return true;
		}

		static bool SoftwareFallback(Matrix4x4<T> matrix, out Matrix4x4<T>result)
		{
			//                                       -1
			// If you have matrix M, inverse Matrix M   can compute
			//
			//     -1       1
			//    M   = --------- A
			//            det(M)
			//
			// A is adjugate (adjoint) of M, where,
			//
			//      T
			// A = C
			//
			// C is Cofactor matrix of M, where,
			//           i + j
			// C   = (-1)      * det(M  )
			//  ij                    ij
			//
			//     [ a b c d ]
			// M = [ e f g h ]
			//     [ i j k l ]
			//     [ m n o p ]
			//
			// First Row
			//           2 | f g h |
			// C   = (-1)  | j k l | = + ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
			//  11         | n o p |
			//
			//           3 | e g h |
			// C   = (-1)  | i k l | = - ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
			//  12         | m o p |
			//
			//           4 | e f h |
			// C   = (-1)  | i j l | = + ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
			//  13         | m n p |
			//
			//           5 | e f g |
			// C   = (-1)  | i j k | = - ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
			//  14         | m n o |
			//
			// Second Row
			//           3 | b c d |
			// C   = (-1)  | j k l | = - ( b ( kp - lo ) - c ( jp - ln ) + d ( jo - kn ) )
			//  21         | n o p |
			//
			//           4 | a c d |
			// C   = (-1)  | i k l | = + ( a ( kp - lo ) - c ( ip - lm ) + d ( io - km ) )
			//  22         | m o p |
			//
			//           5 | a b d |
			// C   = (-1)  | i j l | = - ( a ( jp - ln ) - b ( ip - lm ) + d ( in - jm ) )
			//  23         | m n p |
			//
			//           6 | a b c |
			// C   = (-1)  | i j k | = + ( a ( jo - kn ) - b ( io - km ) + c ( in - jm ) )
			//  24         | m n o |
			//
			// Third Row
			//           4 | b c d |
			// C   = (-1)  | f g h | = + ( b ( gp - ho ) - c ( fp - hn ) + d ( fo - gn ) )
			//  31         | n o p |
			//
			//           5 | a c d |
			// C   = (-1)  | e g h | = - ( a ( gp - ho ) - c ( ep - hm ) + d ( eo - gm ) )
			//  32         | m o p |
			//
			//           6 | a b d |
			// C   = (-1)  | e f h | = + ( a ( fp - hn ) - b ( ep - hm ) + d ( en - fm ) )
			//  33         | m n p |
			//
			//           7 | a b c |
			// C   = (-1)  | e f g | = - ( a ( fo - gn ) - b ( eo - gm ) + c ( en - fm ) )
			//  34         | m n o |
			//
			// Fourth Row
			//           5 | b c d |
			// C   = (-1)  | f g h | = - ( b ( gl - hk ) - c ( fl - hj ) + d ( fk - gj ) )
			//  41         | j k l |
			//
			//           6 | a c d |
			// C   = (-1)  | e g h | = + ( a ( gl - hk ) - c ( el - hi ) + d ( ek - gi ) )
			//  42         | i k l |
			//
			//           7 | a b d |
			// C   = (-1)  | e f h | = - ( a ( fl - hj ) - b ( el - hi ) + d ( ej - fi ) )
			//  43         | i j l |
			//
			//           8 | a b c |
			// C   = (-1)  | e f g | = + ( a ( fk - gj ) - b ( ek - gi ) + c ( ej - fi ) )
			//  44         | i j k |
			//
			// Cost of operation
			// 53 adds, 104 muls, and 1 div.
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
	}

	///// <summary>
	///// Multiplies two matrices together to compute the product
	///// </summary>
	///// <param name="left">The first matrix</param>
	///// <param name="right">The second matrix</param>
	///// <returns>The product matrix</returns>
	//[MethodImpl(AggressiveInlining)]
	//public static Matrix4x4<T> Multiply<T>(Matrix4x4<T> left, Matrix4x4<T> right)
	//	where T : unmanaged, INumberBase<T>
	//{
	//	return left * right;
	//}
	///// <summary>
	///// Multiplies a matrix by a float to compute the product
	///// </summary>
	///// <param name="value1">The matrix to scale</param>
	///// <param name="value2">The scaling value to use</param>
	///// <returns>The scaled matrix</returns>
	//[MethodImpl(AggressiveInlining)]
	//public static Matrix4x4<T> Multiply<T>(Matrix4x4<T> value1, T value2)
	//	where T : unmanaged, INumberBase<T>
	//{
	//	return value1 * value2;
	//}

	///// <summary>
	///// Negates the specified matrix by multiplying all its values by -1
	///// </summary>
	///// <param name="value">The matrix to negate</param>
	///// <returns>The negated matrix</returns>
	//[MethodImpl(AggressiveInlining)]
	//public static Matrix4x4<T> Negate<T>(Matrix4x4<T> value)
	//	where T : unmanaged, INumberBase<T>
	//{
	//	return -value;
	//}

	///// <summary>
	///// Subtracts each element in a second matrix from its corresponding element in a first matrix
	///// </summary>
	///// <param name="left">The first matrix</param>
	///// <param name="right">The second matrix</param>
	///// <returns>The matrix containing the values that result from subtracting each element in <paramref name="right" /> from its corresponding element in <paramref name="left" />.</returns>
	//[MethodImpl(AggressiveInlining)]
	//public static Matrix4x4<T> Subtract<T>(Matrix4x4<T> left, Matrix4x4<T> right)
	//	where T : unmanaged, INumberBase<T>
	//{
	//	return left - right;
	//}

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
	///// <summary>Attempts to extract the scale, translation, and rotation components from the given scale, rotation, or translation matrix. The return value indicates whether the operation succeeded.</summary>
	///// <param name="matrix">The source matrix.</param>
	///// <param name="scale">When this method returns, contains the scaling component of the transformation matrix if the operation succeeded.</param>
	///// <param name="rotation">When this method returns, contains the rotation component of the transformation matrix if the operation succeeded.</param>
	///// <param name="translation">When the method returns, contains the translation component of the transformation matrix if the operation succeeded.</param>
	///// <returns><see langword="true" /> if <paramref name="matrix" /> was decomposed successfully; otherwise,  <see langword="false" />.</returns>
	//public static bool Decompose<T>(Matrix4x4<T> matrix, out Vector3D<T> scale, out Quaternion rotation, out Vector3D<T> translation)
	//	where T : unmanaged, INumberBase<T>, IRootFunctions<T>, IComparisonOperators<T, T, bool>, IComparisonOperators<T, float, bool>
	//{
	//	bool result = true;

	//	unsafe
	//	{
	//		fixed (Vector3D<T>* scaleBase = &scale)
	//		{
	//			T* pfScales = (T*)scaleBase;
	//			T det;

	//			VectorBasis vectorBasis;
	//			Vector3D<T>** pVectorBasis = (Vector3D<T>**)&vectorBasis;

	//			Matrix4x4<T> matTemp = Matrix4x4<T>.Identity;
	//			CanonicalBasis canonicalBasis = default;
	//			Vector3D<T>* pCanonicalBasis = Unsafe.As<Vector3*, Vector3D<T>*>(ref &canonicalBasis.Row0);

	//			canonicalBasis.Row0 = new Vector3(1.0f, 0.0f, 0.0f);
	//			canonicalBasis.Row1 = new Vector3(0.0f, 1.0f, 0.0f);
	//			canonicalBasis.Row2 = new Vector3(0.0f, 0.0f, 1.0f);

	//			translation = new Vector3D<T>(
	//				matrix.M41,
	//				matrix.M42,
	//				matrix.M43);

	//			pVectorBasis[0] = (Vector3D<T>*)&matTemp.M11;
	//			pVectorBasis[1] = (Vector3D<T>*)&matTemp.M21;
	//			pVectorBasis[2] = (Vector3D<T>*)&matTemp.M31;

	//			*(pVectorBasis[0]) = new Vector3D<T>(matrix.M11, matrix.M12, matrix.M13);
	//			*(pVectorBasis[1]) = new Vector3D<T>(matrix.M21, matrix.M22, matrix.M23);
	//			*(pVectorBasis[2]) = new Vector3D<T>(matrix.M31, matrix.M32, matrix.M33);

	//			scale.X = Vector3D.Length(*pVectorBasis[0]);
	//			scale.Y = Vector3D.Length(*pVectorBasis[1]);
	//			scale.Z = Vector3D.Length(*pVectorBasis[2]);

	//			uint a, b, c;
	//			#region Ranking
	//			T x = pfScales[0], y = pfScales[1], z = pfScales[2];
	//			if (x < y)
	//			{
	//				if (y < z)
	//				{
	//					a = 2;
	//					b = 1;
	//					c = 0;
	//				}
	//				else
	//				{
	//					a = 1;

	//					if (x < z)
	//					{
	//						b = 2;
	//						c = 0;
	//					}
	//					else
	//					{
	//						b = 0;
	//						c = 2;
	//					}
	//				}
	//			}
	//			else
	//			{
	//				if (x < z)
	//				{
	//					a = 2;
	//					b = 0;
	//					c = 1;
	//				}
	//				else
	//				{
	//					a = 0;

	//					if (y < z)
	//					{
	//						b = 2;
	//						c = 1;
	//					}
	//					else
	//					{
	//						b = 1;
	//						c = 2;
	//					}
	//				}
	//			}
	//			#endregion

	//			if (pfScales[a] < DecomposeEpsilon)
	//			{
	//				*(pVectorBasis[a]) = pCanonicalBasis[a];
	//			}

	//			*pVectorBasis[a] = Vector3D.Normalize(*pVectorBasis[a]);

	//			if (pfScales[b] < DecomposeEpsilon)
	//			{
	//				uint cc;
	//				T fAbsX, fAbsY, fAbsZ;

	//				fAbsX = MathF.Abs(pVectorBasis[a]->X);
	//				fAbsY = MathF.Abs(pVectorBasis[a]->Y);
	//				fAbsZ = MathF.Abs(pVectorBasis[a]->Z);

	//				#region Ranking
	//				if (fAbsX < fAbsY)
	//				{
	//					if (fAbsY < fAbsZ)
	//					{
	//						cc = 0;
	//					}
	//					else
	//					{
	//						if (fAbsX < fAbsZ)
	//						{
	//							cc = 0;
	//						}
	//						else
	//						{
	//							cc = 2;
	//						}
	//					}
	//				}
	//				else
	//				{
	//					if (fAbsX < fAbsZ)
	//					{
	//						cc = 1;
	//					}
	//					else
	//					{
	//						if (fAbsY < fAbsZ)
	//						{
	//							cc = 1;
	//						}
	//						else
	//						{
	//							cc = 2;
	//						}
	//					}
	//				}
	//				#endregion

	//				*pVectorBasis[b] = Vector3.Cross(*pVectorBasis[a], *(pCanonicalBasis + cc));
	//			}

	//			*pVectorBasis[b] = Vector3.Normalize(*pVectorBasis[b]);

	//			if (pfScales[c] < DecomposeEpsilon)
	//			{
	//				*pVectorBasis[c] = Vector3.Cross(*pVectorBasis[a], *pVectorBasis[b]);
	//			}

	//			*pVectorBasis[c] = Vector3.Normalize(*pVectorBasis[c]);

	//			det = matTemp.GetDeterminant();

	//			// use Kramer's rule to check for handedness of coordinate system
	//			if (det < 0.0f)
	//			{
	//				// switch coordinate system by negating the scale and inverting the basis vector on the x-axis
	//				pfScales[a] = -pfScales[a];
	//				*pVectorBasis[a] = -(*pVectorBasis[a]);

	//				det = -det;
	//			}

	//			det -= 1.0f;
	//			det *= det;

	//			if ((DecomposeEpsilon < det))
	//			{
	//				// Non-SRT matrix encountered
	//				rotation = Quaternion.Identity;
	//				result = false;
	//			}
	//			else
	//			{
	//				// generate the quaternion from the matrix
	//				rotation = Quaternion.CreateFromRotationMatrix(matTemp);
	//			}
	//		}
	//	}

	//	return result;
	//}



	private struct CanonicalBasis
	{
		public Vector3 Row0;
		public Vector3 Row1;
		public Vector3 Row2;
	};

	private struct VectorBasis
	{
		public unsafe Vector3* Element0;
		public unsafe Vector3* Element1;
		public unsafe Vector3* Element2;
	}
}