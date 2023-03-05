using BenchmarkDotNet.Attributes;
using GMat = NiTiS.Math.Matrices.Matrix4x4;
using GMat4 = NiTiS.Math.Matrices.Matrix4x4<float>;
using SMat4 = System.Numerics.Matrix4x4;

public class Program
{
	private static void Main(string[] args)
	{
		//BenchmarkRunner.Run(new Type[] { typeof(MinusCheck), typeof(PlusCheck), typeof(MultiplyCheck) });
	}
	private static SMat4 x, y, z;
	private static GMat4 gx, gy, gz;
	[GlobalSetup]
	public void Setup()
	{
		x = SMat4.Identity;
		y = new(2, 4, 1, 5, 2, 1, 4, 5, 1, 56, 8, 1, 232, 123, 12123413, 1);
		z = default;

		gx = GMat4.Identity;
		gy = new(2, 4, 1, 5, 2, 1, 4, 5, 1, 56, 8, 1, 232, 123, 12123413, 1);
		gz = default;

		
	}

	public class PlusCheck
	{
		[Benchmark]
		public void GenPlus()
		{
			gz = gx + gy;
		}
		[Benchmark]
		public void GenPlus_P()
		{
			gz = GMat.Add(gx, gy);
		}
		[Benchmark]
		public void Plus()
		{
			z = x + y;
		}
		[Benchmark]
		public void GenPlus_S()
		{
			gz = gx + 1f;
		}
		[Benchmark]
		public void GenPlus_SP()
		{
			gz = GMat.Add(gx, 1f);
		}
		[Benchmark]
		public void Plus_S()
		{
			z = x + new SMat4(1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
		}
	}

	public class MinusCheck
	{
		[Benchmark]
		public void GenMinus()
		{
			gz = gx - gy;
		}
		[Benchmark]
		public void GenMinus_P()
		{
			gz = GMat.Subtract(gx, gy);
		}
		[Benchmark]
		public void Minus()
		{
			z = x - y;
		}
		[Benchmark]
		public void GenMinus_S()
		{
			gz = gx - 1f;
		}
		[Benchmark]
		public void GenMinus_SP()
		{
			gz = GMat.Subtract(gx, 1f);
		}
		[Benchmark]
		public void Minus_S()
		{
			z = x - new SMat4(1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
		}
	}

	public class MultiplyCheck
	{
		[Benchmark]
		public void GenMultiply()
		{
			gz = gx * gy;
		}
		[Benchmark]
		public void GenMultiply_P()
		{
			gz = GMat.Multiply(gx, gy);
		}
		[Benchmark]
		public void Multiply()
		{
			z = x * y;
		}
		[Benchmark]
		public void GenMultiply_S()
		{
			gz = gx * 1f;
		}
		[Benchmark]
		public void GenMultiply_SP()
		{
			gz = GMat.Multiply(gx, 1f);
		}
		[Benchmark]
		public void Multiply_S()
		{
			z = x * 1f;// new SMat4(1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
		}
	}
}