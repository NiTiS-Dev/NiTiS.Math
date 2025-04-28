using System.Numerics;
using System.Runtime.CompilerServices;

namespace NiTiS.Mathematics;

public partial struct Vector4i
{
	public static explicit operator Vector4u(in Vector4i value)
	{
		return Unsafe.As<Vector4i, Vector4u>(ref Unsafe.AsRef(in value));
	}
	
	public static explicit operator checked Vector4u(in Vector4i value)
	{
		checked
		{
			return new Vector4u(
				(uint)value.X,
				(uint)value.Y,
				(uint)value.Z,
				(uint)value.W
			);
		}
	}
	
	public static implicit operator Vector4(in Vector4i value)
	{
		return new Vector4(
			value.X,
			value.Y,
			value.Z,
			value.W
		);
	}
	
	public static implicit operator Vector4d(in Vector4i value)
	{
		return new Vector4d(
			value.X,
			value.Y,
			value.Z,
			value.W
		);
	}
	
	public static explicit operator Vector4i(in Vector4 value)
	{
		return new Vector4i(
			(int)value.X,
			(int)value.Y,
			(int)value.Z,
			(int)value.W
		);
	}
	
	public static explicit operator checked Vector4i(in Vector4 value)
	{
		checked
		{
			return new Vector4i(
				(int)value.X,
				(int)value.Y,
				(int)value.Z,
				(int)value.W
			);
		}
	}
}