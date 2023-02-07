using NiTiS.Core.Annotations;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math;

public static unsafe class Matrix4x4
{
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<float> ConvertFromSystem(System.Numerics.Matrix4x4 matrix)
		=> Unsafe.As<System.Numerics.Matrix4x4, Matrix4x4<float>>(ref matrix);
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static System.Numerics.Matrix4x4 ConvertToSystem(Matrix4x4<float> matrix)
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

	/*
	/// <summary>
	/// Creates a matrix that reflects the coordinate system about a specified plane
	/// </summary>
	/// <param name="value">The plane about which to create a reflection</param>
	/// <returns>A new matrix expressing the reflection</returns>
	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4 CreateReflection(Plane value)
	{
		value = Plane.Normalize(value);

		float a = value.Normal.X;
		float b = value.Normal.Y;
		float c = value.Normal.Z;

		float fa = -2.0f * a;
		float fb = -2.0f * b;
		float fc = -2.0f * c;

		Matrix4x4 result = Identity;

		result.M11 = fa * a + 1.0f;
		result.M12 = fb * a;
		result.M13 = fc * a;

		result.M21 = fa * b;
		result.M22 = fb * b + 1.0f;
		result.M23 = fc * b;

		result.M31 = fa * c;
		result.M32 = fb * c;
		result.M33 = fc * c + 1.0f;

		result.M41 = fa * value.D;
		result.M42 = fb * value.D;
		result.M43 = fc * value.D;

		return result;
	}*/

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

	[MethodImpl(AggressiveOptimization | AggressiveInlining)]
	public static Matrix4x4<T> CreateTranslation<T>(Vector3D<T> position)
		where T : unmanaged, INumberBase<T>
		=> new(
			m11: T.One,
			m12: T.Zero,
			m13: T.Zero,
			m14: T.Zero,
			m21: T.Zero,
			m22: T.One,
			m23: T.Zero,
			m24: T.Zero,
			m31: T.Zero,
			m32: T.Zero,
			m33: T.One,
			m34: T.Zero,
			m41: position.X,
			m42: position.Y,
			m43: position.Z,
			m44: T.One
			);
}