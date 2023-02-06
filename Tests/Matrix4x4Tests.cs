using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		Mat4<float> mat4 = Mat4<float>.Identity;

		mat4.M11 = 1f;

	}
}