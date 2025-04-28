using System.Numerics;

namespace NiTiS.Mathematics;

public partial struct Vector3d
{
	public static explicit operator Vector3u(in Vector3d value)
	{
		return new Vector3u(
			(uint)value.X,
			(uint)value.Y,
			(uint)value.Z
		);
	}
	
	public static explicit operator Vector3i(in Vector3d value)
	{
		return new Vector3i(
			(int)value.X,
			(int)value.Y,
			(int)value.Z
		);
	}
	
	public static explicit operator Vector3(in Vector3d value)
	{
		return new Vector3(
			(float)value.X,
			(float)value.Y,
			(float)value.Z
		);
	}

	public static explicit operator Vector3d(in Vector3 value)
	{
		return new Vector3d(
			value.X,
			value.Y,
			value.Z
		);
	}
}