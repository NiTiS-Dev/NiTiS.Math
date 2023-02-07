using NUnit.Framework;

namespace NiTiS.Math.Tests;

public class Matrix4x4Tests
{
	[SetUp]
	public void Setup()
	{
	}
	[Test]
	public void Tesst()
	{
		Matrix4x4<float> mat4 = Matrix4x4<float>.Identity;

		mat4.M11 = 1f;

	}
}