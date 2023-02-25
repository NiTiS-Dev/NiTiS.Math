using NiTiS.Math.Geometry;
using NUnit.Framework;
using System;
using System.Numerics;
using System.Security.AccessControl;
using SMat4 = System.Numerics.Matrix4x4;
using GMat4 = NiTiS.Math.Geometry.Matrix4x4<float>;
using GMat = NiTiS.Math.Geometry.Matrix4x4;

namespace NiTiS.Math.Tests;

public class Matrix4x4Tests
{
	private SMat4 sidt, sys_id1;
	private GMat4 idt, gen_id1;
	[SetUp]
	public void Setup()
	{
		sidt = SMat4.Identity;
		idt = GMat4.Identity;

		GMat4 gen_ypr1 = GMat.CreateFromYawPitchRoll(12f, 26f, 912f);
		SMat4 sys_ypr1 = SMat4.CreateFromYawPitchRoll(12f, 26f, 912f);
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
}