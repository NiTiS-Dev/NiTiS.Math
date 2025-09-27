using System.Threading.Tasks;

namespace NiTiS.Mathematics.Tests;

public class VetorTests
{
	[Test]
	public async Task BinaryOperators()
	{
		Vector2<int> x = Vector2<int>.UnitX;
		Vector2<int> y = Vector2<int>.UnitY;
		_ = x ^ y;
	}
}