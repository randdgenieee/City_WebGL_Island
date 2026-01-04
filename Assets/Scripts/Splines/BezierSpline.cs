using Splines.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Splines
{
	public class BezierSpline : MonoBehaviour
	{
		public delegate void ArcLengthChangedEventHandler(BezierSpline spline);

		[SerializeField]
		private List<Vector3> _points;

		[SerializeField]
		private List<BezierControlPointMode> _modes;

		[SerializeField]
		private List<BezierPointSpace> _pointSpaces;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		private float _arcLengthPrecision = 0.01f;

		private List<float> _arcLengths = new List<float>();

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
				if (_loop)
				{
					_modes[_modes.Count - 1] = _modes[0];
					_pointSpaces[_pointSpaces.Count - 1] = _pointSpaces[0];
					SetControlPoint(0, _points[0], _pointSpaces[0]);
				}
			}
		}

		public int ControlPointCount => _points.Count;

		public int CurveCount => GetControlPointModeIndex(_points.Count);

		public float ArcLengthPrecision
		{
			get
			{
				return _arcLengthPrecision;
			}
			set
			{
				_arcLengthPrecision = value;
				CalcArcLengths();
			}
		}

		public float ArcLength => _arcLengths[_arcLengths.Count - 1];

		public event ArcLengthChangedEventHandler ArcLengthChanged;

		private void FireArcLengthChanged()
		{
			if (this.ArcLengthChanged != null)
			{
				this.ArcLengthChanged(this);
			}
		}

		private void Awake()
		{
			CalcArcLengths();
		}

		public Vector3 GetControlPoint(int index)
		{
			return _points[index];
		}

		public void SetControlPoint(int index, Vector3 point, BezierPointSpace space)
		{
			Vector3 value = TransformPoint(point, space, GetControlPointSpace(index));
			if (index % 3 == 0)
			{
				Vector3 vector = point - GetControlPoint(index, space);
				if (_loop)
				{
					if (index == 0)
					{
						List<Vector3> points = _points;
						points[1] += vector;
						points = _points;
						int index2 = _points.Count - 2;
						points[index2] += vector;
						_points[_points.Count - 1] = value;
					}
					else if (index == _points.Count - 1)
					{
						_points[0] = value;
						List<Vector3> points = _points;
						points[1] += vector;
						points = _points;
						int index2 = index - 1;
						points[index2] += vector;
					}
					else
					{
						List<Vector3> points = _points;
						int index2 = index - 1;
						points[index2] += vector;
						points = _points;
						index2 = index + 1;
						points[index2] += vector;
					}
				}
				else
				{
					if (index > 0)
					{
						List<Vector3> points = _points;
						int index2 = index - 1;
						points[index2] += vector;
					}
					if (index + 1 < _points.Count)
					{
						List<Vector3> points = _points;
						int index2 = index + 1;
						points[index2] += vector;
					}
				}
			}
			_points[index] = value;
			EnforceMode(index);
			CalcArcLengths();
		}

		public BezierControlPointMode GetControlPointMode(int index)
		{
			return _modes[GetControlPointModeIndex(index)];
		}

		public void SetControlPointMode(int index, BezierControlPointMode mode)
		{
			int controlPointModeIndex = GetControlPointModeIndex(index);
			_modes[controlPointModeIndex] = mode;
			if (_loop)
			{
				if (controlPointModeIndex == 0)
				{
					_modes[_modes.Count - 1] = mode;
				}
				else if (controlPointModeIndex == _modes.Count - 1)
				{
					_modes[0] = mode;
				}
			}
			EnforceMode(index);
			CalcArcLengths();
		}

		public BezierPointSpace GetControlPointSpace(int index)
		{
			return _pointSpaces[GetControlPointModeIndex(index)];
		}

		public void SetControlPointSpace(int index, BezierPointSpace space)
		{
			int controlPointModeIndex = GetControlPointModeIndex(index);
			ConvertControlPointSpace(controlPointModeIndex, space);
			if (_loop)
			{
				if (controlPointModeIndex == 0)
				{
					ConvertControlPointSpace(_pointSpaces.Count - 1, space);
				}
				else if (controlPointModeIndex == _pointSpaces.Count - 1)
				{
					ConvertControlPointSpace(0, space);
				}
			}
			CalcArcLengths();
		}

		public Vector3 GetControlPoint(int index, BezierPointSpace space)
		{
			return TransformPoint(_points[index], GetControlPointSpace(index), space);
		}

		public Vector3 GetPoint(float t)
		{
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = _points.Count - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			Vector3 controlPoint = GetControlPoint(num, BezierPointSpace.Local);
			Vector3 controlPoint2 = GetControlPoint(num + 1, BezierPointSpace.Local);
			Vector3 controlPoint3 = GetControlPoint(num + 2, BezierPointSpace.Local);
			Vector3 controlPoint4 = GetControlPoint(num + 3, BezierPointSpace.Local);
			return base.transform.TransformPoint(BezierUtils.GetPoint(controlPoint, controlPoint2, controlPoint3, controlPoint4, t));
		}

		public Vector3 GetVelocity(float t)
		{
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = _points.Count - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			Vector3 controlPoint = GetControlPoint(num, BezierPointSpace.Local);
			Vector3 controlPoint2 = GetControlPoint(num + 1, BezierPointSpace.Local);
			Vector3 controlPoint3 = GetControlPoint(num + 2, BezierPointSpace.Local);
			Vector3 controlPoint4 = GetControlPoint(num + 3, BezierPointSpace.Local);
			return base.transform.TransformPoint(BezierUtils.GetFirstDerivative(controlPoint, controlPoint2, controlPoint3, controlPoint4, t)) - base.transform.position;
		}

		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}

		public Vector3 GetUniformPoint(float t)
		{
			return GetPoint(CalcNonUniformInterpolant(t));
		}

		public Vector3 GetUniformVelocity(float t)
		{
			return GetVelocity(CalcNonUniformInterpolant(t));
		}

		public Vector3 GetUniformDirection(float t)
		{
			return GetUniformVelocity(t).normalized;
		}

		public void AddCurve(int index)
		{
			index -= index % 3;
			Vector3 item = _points[index];
			item.x += 1f;
			item.y += 1f;
			_points.Insert(index + 1, item);
			item.x += 1f;
			item.y -= 2f;
			_points.Insert(index + 2, item);
			item.x += 1f;
			item.y += 1f;
			_points.Insert(index + 3, item);
			int controlPointModeIndex = GetControlPointModeIndex(index);
			_modes.Insert(controlPointModeIndex + 1, _modes[controlPointModeIndex]);
			_pointSpaces.Insert(controlPointModeIndex + 1, _pointSpaces[controlPointModeIndex]);
			EnforceMode(index);
			if (_loop)
			{
				_points[_points.Count - 1] = _points[0];
				_modes[_modes.Count - 1] = _modes[0];
				_pointSpaces[_modes.Count - 1] = _pointSpaces[0];
				EnforceMode(0);
			}
			CalcArcLengths();
		}

		public void DeleteCurve(int index)
		{
			if (ControlPointCount > 4)
			{
				index -= index % 3;
				bool flag = index == 0;
				bool flag2 = index == _points.Count - 1;
				if (flag && _loop)
				{
					SetControlPointMode(_points.Count - 1, BezierControlPointMode.Free);
				}
				else if (!flag)
				{
					SetControlPointMode(index - 3, BezierControlPointMode.Free);
				}
				if (flag2 && _loop)
				{
					SetControlPointMode(0, BezierControlPointMode.Free);
				}
				else if (!flag2)
				{
					SetControlPointMode(index + 3, BezierControlPointMode.Free);
				}
				int controlPointModeIndex = GetControlPointModeIndex(index);
				_modes.RemoveAt(controlPointModeIndex);
				_pointSpaces.RemoveAt(controlPointModeIndex);
				if (flag)
				{
					_points.RemoveAt(index + 2);
					_points.RemoveAt(index + 1);
					_points.RemoveAt(index);
					Loop = _loop;
				}
				else if (flag2)
				{
					_points.RemoveAt(index);
					_points.RemoveAt(index - 1);
					_points.RemoveAt(index - 2);
					Loop = _loop;
				}
				else
				{
					_points.RemoveAt(index + 1);
					_points.RemoveAt(index);
					_points.RemoveAt(index - 1);
				}
			}
		}

		public void SnapPoints(int snapPrecision)
		{
			_points.Floor(snapPrecision, EnforceMode);
			CalcArcLengths();
		}

		public void MoveSpline(Vector3 delta)
		{
			int i = 0;
			for (int count = _points.Count; i < count; i++)
			{
				List<Vector3> points = _points;
				int index = i;
				points[index] += delta;
			}
		}

		public void ReverseSpline()
		{
			_points.Reverse();
			_modes.Reverse();
			_pointSpaces.Reverse();
			CalcArcLengths();
		}

		private int GetControlPointModeIndex(int index)
		{
			return (index + 1) / 3;
		}

		private void EnforceMode(int index)
		{
			int controlPointModeIndex = GetControlPointModeIndex(index);
			BezierControlPointMode bezierControlPointMode = _modes[controlPointModeIndex];
			if (bezierControlPointMode == BezierControlPointMode.Free || (!_loop && (controlPointModeIndex == 0 || controlPointModeIndex == _modes.Count - 1)))
			{
				return;
			}
			int num = controlPointModeIndex * 3;
			int num2;
			int num3;
			if (index <= num)
			{
				num2 = num - 1;
				if (num2 < 0)
				{
					num2 = _points.Count - 2;
				}
				num3 = num + 1;
				if (num3 >= _points.Count)
				{
					num3 = 1;
				}
			}
			else
			{
				num2 = num + 1;
				if (num2 >= _points.Count)
				{
					num2 = 1;
				}
				num3 = num - 1;
				if (num3 < 0)
				{
					num3 = _points.Count - 2;
				}
			}
			Vector3 a = _points[num];
			Vector3 b = a - _points[num2];
			if (bezierControlPointMode == BezierControlPointMode.Aligned)
			{
				b = b.normalized * Vector3.Distance(a, _points[num3]);
			}
			_points[num3] = a + b;
		}

		private void ConvertControlPointSpace(int modeIndex, BezierPointSpace space)
		{
			BezierPointSpace bezierPointSpace = _pointSpaces[modeIndex];
			if (bezierPointSpace != space)
			{
				int num = modeIndex * 3;
				ConvertControlPointSpace(num, bezierPointSpace, space);
				int num2 = num - 1;
				if (num2 != num && num2 >= 0 && num2 < _points.Count)
				{
					ConvertControlPointSpace(num2, bezierPointSpace, space);
				}
				num2 = num + 1;
				if (num2 != num && num2 >= 0 && num2 < _points.Count)
				{
					ConvertControlPointSpace(num2, bezierPointSpace, space);
				}
				_pointSpaces[modeIndex] = space;
			}
		}

		private void ConvertControlPointSpace(int index, BezierPointSpace currentSpace, BezierPointSpace newSpace)
		{
			_points[index] = TransformPoint(_points[index], currentSpace, newSpace);
		}

		private Vector3 TransformPoint(Vector3 point, BezierPointSpace currentSpace, BezierPointSpace newSpace)
		{
			if (currentSpace == newSpace)
			{
				return point;
			}
			switch (currentSpace)
			{
			case BezierPointSpace.Local:
				if (newSpace == BezierPointSpace.World)
				{
					return base.transform.TransformPoint(point);
				}
				break;
			case BezierPointSpace.World:
				if (newSpace == BezierPointSpace.Local)
				{
					return base.transform.InverseTransformPoint(point);
				}
				break;
			}
			return point;
		}

		private void CalcArcLengths()
		{
			float num = _arcLengthPrecision / (float)CurveCount;
			int num2 = Mathf.RoundToInt(1f / num);
			_arcLengths.Clear();
			_arcLengths.Add(0f);
			Vector3 a = GetPoint(0f);
			float num3 = 0f;
			for (int i = 1; i <= num2; i++)
			{
				Vector3 point = GetPoint((float)i * num);
				num3 += (a - point).magnitude;
				_arcLengths.Add(num3);
				a = point;
			}
			FireArcLengthChanged();
		}

		private float CalcNonUniformInterpolant(float uniformInterpolant)
		{
			int num = _arcLengths.Count - 1;
			float num2 = Mathf.Clamp01(uniformInterpolant) * _arcLengths[num];
			int num3 = 0;
			int num4 = num;
			int num5 = 0;
			while (num3 < num4)
			{
				num5 = num3 + Mathf.Max(0, (num4 - num3) / 2);
				if (_arcLengths[num5] < num2)
				{
					num3 = num5 + 1;
				}
				else
				{
					num4 = num5;
				}
			}
			if (_arcLengths[num5] > num2)
			{
				num5--;
			}
			float num6 = _arcLengths[num5];
			if (Mathf.Approximately(num6, num2))
			{
				return (float)num5 / (float)num;
			}
			return ((float)num5 + (num2 - num6) / (_arcLengths[num5 + 1] - num6)) / (float)num;
		}
	}
}
