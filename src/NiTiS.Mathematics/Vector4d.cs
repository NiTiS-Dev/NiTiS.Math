using System.Numerics;

namespace NiTiS.Mathematics;

public partial struct Vector4d
{
	public static explicit operator Vector4u(in Vector4d value)
	{
		return new Vector4u(
			(uint)value.X,
			(uint)value.Y,
			(uint)value.Z,
			(uint)value.W
		);
	}
	
	public static explicit operator Vector4i(in Vector4d value)
	{
		return new Vector4i(
			(int)value.X,
			(int)value.Y,
			(int)value.Z,
			(int)value.W
		);
	}
	
	public static explicit operator Vector4(in Vector4d value)
	{
		return new Vector4(
			(float)value.X,
			(float)value.Y,
			(float)value.Z,
			(float)value.W
		);
	}

	public static explicit operator Vector4d(in Vector4 value)
	{
		return new Vector4d(
			value.X,
			value.Y,
			value.Z,
			value.W
		);
	}
}