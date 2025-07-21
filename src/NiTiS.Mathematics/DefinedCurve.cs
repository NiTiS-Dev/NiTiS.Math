using System;
using System.Numerics;
using CommunityToolkit.Diagnostics;

namespace NiTiS.Mathematics;

public sealed class DefinedCurve<T> : Curve<T, T>
	where T : unmanaged, INumber<T>
{
	private readonly Point[] _points;
	private readonly Coefficients[] _coefficients;

	public DefinedCurve(params ReadOnlySpan<Point> points)
	{
		Guard.HasSizeGreaterThanOrEqualTo(points, 2);

		_points = new Point[points.Length];
		points.CopyTo(_points);
		_points.Sort();

		_coefficients = new Coefficients[points.Length - 1];
		ComputeCubicSplineCoefficients();
	}

	public override T Get(T x)
	{
		if (_points[0].X > x || _points[^1].X < x)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException();
		}

		// Find the interval
		int i = 0;
		while (i < _points.Length - 1 && x > _points[i + 1].X)
		{
			i++;
		}
		if (i >= _points.Length - 1)
		{
			return _points[^1].Y;
		}

		// Evaluate the cubic polynomial
		T dx = x - _points[i].X;
		(T a, T b, T c, T d) = _coefficients[i];
		return a * dx * dx * dx + b * dx * dx + c * dx + d;
	}

	private void ComputeCubicSplineCoefficients()
    {
        int n = _points.Length - 1; // Number of intervals
        T[] h = new T[n];
        T[] y = new T[n + 1];
        for (int i = 0; i < n; i++)
        {
            h[i] = _points[i + 1].X - _points[i].X;
            y[i] = _points[i].Y;
        }
        y[n] = _points[n].Y;

        // Set up tridiagonal system for second derivatives (natural spline)
        T[] k = new T[n + 1]; // Second derivatives
        T[] b = new T[n]; // Right-hand side
        T[] a = new T[n - 1]; // Lower diagonal
        T[] c = new T[n - 1]; // Upper diagonal
        T[] d = new T[n + 1]; // Main diagonal

        // Compute differences and set up system
        for (int i = 0; i < n - 1; i++)
        {
            a[i] = h[i];
            c[i] = h[i + 1];
            d[i + 1] = T.CreateChecked(2) * (h[i] + h[i + 1]);
        }
        d[0] = T.One; // For natural spline boundary
        d[n] = T.One;

        for (int i = 1; i < n; i++)
        {
            b[i] = T.CreateChecked(6) * ((y[i + 1] - y[i]) / h[i] - (y[i] - y[i - 1]) / h[i - 1]);
        }

        // Solve tridiagonal system using Thomas algorithm
        T[] c_temp = new T[n - 1];
        for (int i = 0; i < n - 1; i++)
        {
            c_temp[i] = c[i];
        }

        // Forward elimination
        for (int i = 1; i < n; i++)
        {
            T m = a[i - 1] / d[i - 1];
            d[i] -= m * c_temp[i - 1];
            b[i] -= m * b[i - 1];
        }

        // Back substitution
        k[n] = T.Zero; // Natural spline: k_n = 0
        k[n - 1] = b[n - 1] / d[n - 1];
        for (int i = n - 2; i >= 0; i--)
        {
            k[i] = (b[i] - c_temp[i] * k[i + 1]) / d[i];
        }

        // Compute coefficients for each interval
        for (int i = 0; i < n; i++)
        {
            T hi = T.CreateChecked(h[i]);
            _coefficients[i] = new(
                A: (k[i + 1] - k[i]) / (T.CreateChecked(6) * hi),
                B: k[i] / T.CreateChecked(2),
                C: (y[i + 1] - y[i]) / hi - hi * (k[i + 1] + T.CreateChecked(2) * k[i]) / T.CreateChecked(6),
                D: y[i]
            );
        }
    }

	private record struct Coefficients(T A, T B, T C, T D);
}