using NiTiS.Core.Annotations;
using System;
using System.Numerics;

namespace NiTiS.Math;

public static unsafe class Mat4
{
	public static Mat4<float> CreateBillboard(Vector3D<float> objPos, Vector3D<float> cameraPos, Vector3D<float> cameraUp, Vector3D<float> cameraForward)
	{
		const float epsilon = 1e-4f;

		Vector3D<float> zaxis = new(
			objPos.X - cameraPos.X,
			objPos.Y - cameraPos.Y,
			objPos.Z - cameraPos.Z
			);

		float norm = zaxis.LengthSquared;

		if (norm < epsilon)
		{
			zaxis = -cameraForward;
		}
		else
		{
			zaxis = Vector3D.Multiply(zaxis, 1f / (float)SMath.Sqrt(norm));
		}

		Vector3D<float> xaxis, yaxis;


		xaxis = Vector3D.Normalize(Vector3D.Cross(cameraUp, zaxis));

		yaxis = Vector3D.Cross(zaxis, xaxis);

		return new(
				m11: xaxis.X,
				m12: xaxis.Y,
				m13: xaxis.Z,
				m14: 0f,
				m21: yaxis.X,
				m22: yaxis.Y,
				m23: yaxis.Z,
				m24: 0f,
				m31: zaxis.X,
				m32: zaxis.Y,
				m33: zaxis.Z,
				m34: 0f,
				m41: objPos.X,
				m42: objPos.Y,
				m43: objPos.Z,
				m44: 1f
				);
	}
	[NotImplementYet]
	[Obsolete(nameof(NotImplementYetAttribute))]
	public static Mat4<float> CreateConstrainedBillboard()
		=> default;

	public static Mat4<T> CreateTranslation<T>(Vector3D<T> position)
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