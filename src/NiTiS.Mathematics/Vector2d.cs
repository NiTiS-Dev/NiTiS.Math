using System.Numerics;

namespace NiTiS.Mathematics;

public partial struct Vector2d
{
	public static explicit operator Vector2u(in Vector2d value)
	{
		return new Vector2u(
			(uint)value.X,
			(uint)value.Y
		);
	}
	
	public static explicit operator Vector2i(in Vector2d value)
	{
		return new Vector2i(
			(int)value.X,
			(int)value.Y
		);
	}
	
	public static explicit operator Vector2(in Vector2d value)
	{
		return new Vector2(
			(float)value.X,
			(float)value.Y
		);
	}

	public static explicit operator Vector2d(in Vector2 value)
	{
		return new Vector2d(
			value.X,
			value.Y
		);
	}
}