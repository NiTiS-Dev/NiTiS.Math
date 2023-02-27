using NiTiS.Math;
using NiTiS.Math.Geometry;
using NUnit.Framework;
using System.Reflection.Metadata;

namespace NiTiS.Math.Tests;

public class ScalarTests
{
	[SetUp]
	public void Setup()
	{
	}
	[Test]
	public void IsPrime()
	{
		int[] primes = Consts.Primes;

		for (int i = 0; i < primes.Length; i++)
		{
			int number = primes[i];
			Assert.AreEqual(true, Scalar.IsPrime(number), $"Number {number} is not prime");
		}
	}
	[Test]
	public void GreatestCommonDivisor()
	{
		Assert.AreEqual(24, Scalar.GreatestCommonDivisor(24, 48));
		Assert.AreEqual(12, Scalar.GreatestCommonDivisor(12, 48));
		Assert.AreEqual(1, Scalar.GreatestCommonDivisor(1, 2));
		Assert.AreEqual(1024, Scalar.GreatestCommonDivisor(-1024, 2048));
		Assert.AreEqual(1024, Scalar.GreatestCommonDivisor(-1024, -1024));
		Assert.AreEqual(1, Scalar.GreatestCommonDivisor(1, 1));
	}
	[Test]
	public void LeastCommonMultiple()
	{
		Assert.AreEqual(48, Scalar.LeastCommonMultiple(24, 48));
		Assert.AreEqual(21, Scalar.LeastCommonMultiple(7, 3));
		Assert.AreEqual(8, Scalar.LeastCommonMultiple(-2, 8));
		Assert.AreEqual(8, Scalar.LeastCommonMultiple(-8, 8));
		Assert.AreEqual(1, Scalar.LeastCommonMultiple(1, 1));
	}
}
