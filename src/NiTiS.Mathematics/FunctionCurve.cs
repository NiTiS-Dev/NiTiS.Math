using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NiTiS.Mathematics;

public sealed class FunctionCurve<TX, TY>  : Curve<TX, TY>
	where TX : INumber<TX>
	where TY : INumber<TY>
{
	private readonly Func<TX, TY> _func;

	public FunctionCurve(Func<TX, TY> func)
	{
		_func = func;
	}

	public override TY Get(TX x)
	{
		return _func(x);
	}

	// public override Region<TX>? Domain => null;
	// public override Region<TY>? Range => null;
}