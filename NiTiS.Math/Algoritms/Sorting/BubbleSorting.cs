using System;
using System.Numerics;

namespace NiTiS.Math.Algoritms.Sorting;

// Why i can't implement interfaces in static classes? 
public sealed class BubbleSorting<T> : ISortingAlgorithm<T>
	where T : IComparisonOperators<T, T, bool>
{
	private BubbleSorting() { }

	public static void Sort(Span<T> collection)
	{
		if (collection.IsEmpty)
			return;

		bool sorted;
		do
		{
			sorted = true;
			for (int i = 1; i < collection.Length; i++)
			{
				if (collection[i] < collection[i - 1])
				{
					ISortingAlgorithm<T>.Swap(collection, i - 1, i);
					sorted = false;
				}
			}
		} while (!sorted);
	}
	public static void BackSort(Span<T> collection)
	{
		if (collection.IsEmpty)
			return;

		bool sorted;
		do
		{
			sorted = true;
			for (int i = 1; i < collection.Length; i++)
			{
				if (collection[i] > collection[i - 1])
				{
					ISortingAlgorithm<T>.Swap(collection, i - 1, i);
					sorted = false;
				}
			}
		} while (!sorted);
	}
	public static void Sort(T[] collection)
	{
		if (collection is null || collection.Length < 2)
			return;

		bool sorted;
		do
		{
			sorted = true;
			for (int i = 1; i < collection.Length; i++)
			{
				if (collection[i] < collection[i - 1])
				{
					ISortingAlgorithm<T>.Swap(collection, i - 1, i);
					sorted = false;
				}
			}
		} while (!sorted);
	}
	public static void BackSort(T[] collection)
	{
		if (collection is null || collection.Length < 2)
			return;

		bool sorted;
		do
		{
			sorted = true;
			for (int i = 1; i < collection.Length; i++)
			{
				if (collection[i] > collection[i - 1])
				{
					ISortingAlgorithm<T>.Swap(collection, i - 1, i);
					sorted = false;
				}
			}
		} while (!sorted);
	}
}
