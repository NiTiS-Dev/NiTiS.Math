using System.Numerics;
using System.Runtime.CompilerServices;

namespace NiTiS.Mathematics;

public partial struct Vector2i
{
	public static explicit operator Vector2u(in Vector2i value)
	{
		return Unsafe.As<Vector2i, Vector2u>(ref Unsafe.AsRef(in value));
	}
	
	public static explicit operator checked Vector2u(in Vector2i value)
	{
		checked
		{
			return new Vector2u(
				(uint)value.X,
				(uint)value.Y
			);
		}
	}
	
	public static implicit operator Vector2(in Vector2i value)
	{
		return new Vector2(
			value.X,
			value.Y
		);
	}
	
	public static implicit operator Vector2d(in Vector2i value)
	{
		return new Vector2d(
			value.X,
			value.Y
		);
	}
	
	public static explicit operator Vector2i(in Vector2 value)
	{
		return new Vector2i(
			(int)value.X,
			(int)value.Y
		);
	}
	
	public static explicit operator checked Vector2i(in Vector2 value)
	{
		checked
		{
			return new Vector2i(
				(int)value.X,
				(int)value.Y
			);
		}
	}
}