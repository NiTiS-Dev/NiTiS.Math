using NiTiS.Math.Geometry;
using NUnit.Framework;

namespace NiTiS.Math.Tests;

public class Region2dTests
{
	private Region2d<float> x, y, z, w, q;
	
	[SetUp]
	public void Setup()
	{
		x = new(0, 0, 1, 1);
		y = new(0, 0, 2, 2);

		z = new(3, 3, 1, 5);
		w = new(2, 2, 1, 1);
		q = default;
	}
	[Test]
	public void Square()
	{
		Assert.AreEqual(Region2d.Square(x), 1);
		Assert.AreEqual(Region2d.Square(y), 4);

		Assert.AreEqual(Region2d.Square(z), 5);
		Assert.AreEqual(Region2d.Square(w), 1);
	}
	[Test]
	public void Perimeter()
	{
		Assert.AreEqual(Region2d.Perimeter(x), 4);
		Assert.AreEqual(Region2d.Perimeter(y), 8);

		Assert.AreEqual(Region2d.Perimeter(z), 12);
		Assert.AreEqual(Region2d.Perimeter(w), 4);
	}
	[Test]
	public void Collision()
	{
		Assert.AreEqual(Region2d.Collision(x, y), true);

		Assert.AreEqual(Region2d.Collision(z, w), false);
	}
}