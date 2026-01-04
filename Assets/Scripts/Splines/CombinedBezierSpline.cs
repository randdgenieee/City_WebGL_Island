using System.Collections.Generic;
using UnityEngine;

namespace Splines
{
	public class CombinedBezierSpline
	{
		private List<BezierSpline> _splines;

		private List<float> _arcLengths;

		public float ArcLength => _arcLengths[_arcLengths.Count - 1];

		public CombinedBezierSpline(List<BezierSpline> splines)
		{
			_splines = splines;
			int count = _splines.Count;
			for (int i = 0; i < count; i++)
			{
				_splines[i].ArcLengthChanged += OnSplineArcLengthChanged;
			}
			CalcArcLengths();
		}

		public Vector3 GetPoint(float t)
		{
			CalcInterpolantAtT(t, out int index, out float tInSpline);
			return _splines[index].GetPoint(tInSpline);
		}

		public Vector3 GetVelocity(float t)
		{
			CalcInterpolantAtT(t, out int index, out float tInSpline);
			return _splines[index].GetVelocity(tInSpline);
		}

		public Vector3 GetDirection(float t)
		{
			CalcInterpolantAtT(t, out int index, out float tInSpline);
			return _splines[index].GetDirection(tInSpline);
		}

		public Vector3 GetUniformPoint(float t)
		{
			CalcInterpolantAtT(t, out int index, out float tInSpline);
			return _splines[index].GetUniformPoint(tInSpline);
		}

		public Vector3 GetUniformVelocity(float t)
		{
			CalcInterpolantAtT(t, out int index, out float tInSpline);
			return _splines[index].GetUniformVelocity(tInSpline);
		}

		public Vector3 GetUniformDirection(float t)
		{
			CalcInterpolantAtT(t, out int index, out float tInSpline);
			return _splines[index].GetUniformDirection(tInSpline);
		}

		private void CalcInterpolantAtT(float t, out int index, out float tInSpline)
		{
			float arcLengthAtT = ArcLength * Mathf.Clamp01(t);
			index = _arcLengths.FindIndex((float x) => x >= arcLengthAtT);
			if (index < 0)
			{
				UnityEngine.Debug.LogErrorFormat("Cannot find ArcLength belonging to '{0}' [t={1};AL={2}]", arcLengthAtT, t, ArcLength);
				UnityEngine.Debug.Break();
			}
			float num = (index <= 0) ? 0f : _arcLengths[index - 1];
			float num2 = arcLengthAtT - num;
			tInSpline = num2 / _splines[index].ArcLength;
		}

		private void CalcArcLengths()
		{
			int count = _splines.Count;
			if (_arcLengths == null)
			{
				_arcLengths = new List<float>(count);
			}
			else
			{
				_arcLengths.Clear();
			}
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				num += _splines[i].ArcLength;
				_arcLengths.Add(num);
			}
		}

		private void OnSplineArcLengthChanged(BezierSpline spline)
		{
			CalcArcLengths();
		}
	}
}
