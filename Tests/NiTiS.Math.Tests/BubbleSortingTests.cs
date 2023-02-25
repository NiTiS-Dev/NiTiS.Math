using NUnit.Framework;
using System;
using NiTiS.Math.Algoritms.Sorting;

namespace NiTiS.Math.Tests;

public class BubbleSortingTests
{
	[SetUp]
	public void Setup()
	{

	}
	[Test]
	public void BubbleSorting()
	{
		Span<int> span = stackalloc int[] { 5, 1, 9 };
		Span<int> req = stackalloc int[] { 1, 5, 9 };

		BubbleSorting<int>.Sort(span);
		Assert.IsTrue(req[0] == span[0]);
		Assert.IsTrue(req[1] == span[1]);
		Assert.IsTrue(req[2] == span[2]);
	}
	[Test]
	public void BackBubbleSorting()
	{
		Span<int> span = stackalloc int[] { 5, 1, 9, 4 };
		Span<int> req = stackalloc int[] { 9, 5, 4, 1 };

		BubbleSorting<int>.BackSort(span);
		Assert.IsTrue(req[0] == span[0]);
		Assert.IsTrue(req[1] == span[1]);
		Assert.IsTrue(req[2] == span[2]);
		Assert.IsTrue(req[3] == span[3]);
		Assert.IsTrue(req[3] == span[3]);
	}
}
