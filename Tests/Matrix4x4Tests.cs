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

		//System.Numerics.Matrix4x4.CreateConstrainedBillboard()

		mat4.M11 = 1f;

		System.Console.WriteLine(mat4);
	}
}