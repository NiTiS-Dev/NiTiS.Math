using NiTiS.Math.Matrices;
using NUnit.Framework;
using System;
using GMat = NiTiS.Math.Matrices.Matrix4x4;
using GMat4 = NiTiS.Math.Matrices.Matrix4x4<float>;
using SMat4 = System.Numerics.Matrix4x4;

namespace NiTiS.Math.Tests;

public class Matrix4x4Tests
{
	private SMat4 sidt, sys_id1;
	private GMat4 idt, gen_id1, gen_mul1;
	private Vector4d<float> gen_mul1vec;
	[SetUp]
	public void Setup()
	{
		sidt = SMat4.Identity;
		idt = GMat4.Identity;

		GMat4 gen_ypr1 = GMat.CreateFromYawPitchRoll(12f, 26f, 912f);
		SMat4 sys_ypr1 = SMat4.CreateFromYawPitchRoll(12f, 26f, 912f);

		gen_mul1vec = new(5, 2, 1, 8);
		gen_mul1 = new(
			1, 2, 3, 4,
			2, 1, 5, 9,
			4, 4, 1, 6,
			6, 9, 1, 7
			);
	}
	[Test]
	public void IsIdentity_Test1()
	{
		Assert.AreEqual(idt.IsIdentity, sidt.IsIdentity);
	}
	[Test]
	public void IsIdentity_Test2()
	{
		Assert.IsTrue(idt.ConvertToSystem().IsIdentity);
		Assert.IsTrue(sidt.ConvertToGeneric().IsIdentity);
	}
	[Test]
	public void FloatScalar_Check()
	{
		// FloatScalar.cs
		Assert.AreEqual(1.0f - 0.1f * (MathF.PI / 180.0f), 0.99825467074800567042307630923151f);
	}
	[Test]
	public void CreateFromYawPitchRoll_Test()
	{
		Assert.IsTrue(gen_id1.ConvertToSystem().Equals(sys_id1));
		Assert.IsTrue(sys_id1.ConvertToGeneric().Equals(gen_id1));
	}
	[Test]
	public void Multiplication_Vec4byMatrix4x4_Test()
	{
		Assert.AreEqual(new Vector4d<float>(61, 88, 34, 100), gen_mul1vec * gen_mul1);
	}
	[Test]
	public void Multiplication_Matrix4x4byVec4_Test()
	{
		Assert.AreEqual(new Vector4d<float>(44, 89, 77, 105), gen_mul1 * gen_mul1vec);
	}
}