using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace NiTiS.Math.Algoritms.Sorting;

public interface ISortingAlgorithm<T>
{
    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    protected static void Swap(Span<T> buff, int x, int y)
        => (buff[y], buff[x]) = (buff[x], buff[y]);

    [MethodImpl(AggressiveInlining | AggressiveOptimization)]
    protected static void Swap(T[] buff, int x, int y)
        => (buff[y], buff[x]) = (buff[x], buff[y]);
}
