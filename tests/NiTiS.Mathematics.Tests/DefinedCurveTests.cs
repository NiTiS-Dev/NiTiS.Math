using System.Threading.Tasks;

namespace NiTiS.Mathematics.Tests;

public class DefinedCurveTests
{
	[Test]
	public async Task Points()
	{
		DefinedCurve<float> x = new(new(1, 1), new(2, 7), new(-1, 9));

		await Assert.That(x.Get(1)).IsEqualTo(1);
		await Assert.That(x.Get(-1)).IsEqualTo(9);
		await Assert.That(x.Get(2)).IsEqualTo(7);
	}

	[Test]
	public async Task Interpolation()
	{
		DefinedCurve<float> x = new(new(1, 1), new(2, 7), new(-1, 9));

		await Assert.That(x.Get(0)).IsGreaterThan(1).IsLessThan(9);
	}
}