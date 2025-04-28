using System.Numerics;
using System.Runtime.CompilerServices;

namespace NiTiS.Mathematics;

public partial struct Vector3i
{
	public static explicit operator Vector3u(in Vector3i value)
	{
		return Unsafe.As<Vector3i, Vector3u>(ref Unsafe.AsRef(in value));
	}
	
	public static explicit operator checked Vector3u(in Vector3i value)
	{
		checked
		{
			return new Vector3u(
				(uint)value.X,
				(uint)value.Y,
				(uint)value.Z
			);
		}
	}
	
	public static implicit operator Vector3(in Vector3i value)
	{
		return new Vector3(
			value.X,
			value.Y,
			value.Z
		);
	}
	
	public static implicit operator Vector3d(in Vector3i value)
	{
		return new Vector3d(
			value.X,
			value.Y,
			value.Z
		);
	}
	
	public static explicit operator Vector3i(in Vector3 value)
	{
		return new Vector3i(
			(int)value.X,
			(int)value.Y,
			(int)value.Z
		);
	}
	
	public static explicit operator checked Vector3i(in Vector3 value)
	{
		checked
		{
			return new Vector3i(
				(int)value.X,
				(int)value.Y,
				(int)value.Z
			);
		}
	}
}