namespace NiTiS.Math;

public static class Mat4
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

		if (norm< epsilon)
		{
			zaxis = -cameraForward;
		}
		else
		{
			zaxis = Vector3D.Multiply(zaxis, 1f / (float)SMath.Sqrt(norm));
		}
	}
}