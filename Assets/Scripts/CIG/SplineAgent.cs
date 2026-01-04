using Splines;
using UnityEngine;

namespace CIG
{
	public class SplineAgent : GridAgent
	{
		public delegate void PathCompletedEventHandler(SplineAgent splineAgent, bool reverse);

		private BezierSpline _spline;

		private float _moveSpeed;

		private float _travelledDistance;

		public event PathCompletedEventHandler PathCompletedEvent;

		private void FirePathCompletedEvent(bool reverse)
		{
			if (this.PathCompletedEvent != null)
			{
				this.PathCompletedEvent(this, reverse);
			}
		}

		public void Initialize(IsometricGrid isometricGrid, Timing timing, BezierSpline spline, float moveDuration)
		{
			_spline = spline;
			base.Speed = 0f;
			_moveSpeed = _spline.ArcLength / moveDuration;
			InitializeGridAgent(isometricGrid, timing);
		}

		public void StartMoving(bool reverse)
		{
			base.Speed = (reverse ? (0f - _moveSpeed) : _moveSpeed);
		}

		protected override void SetInitialPosition()
		{
		}

		protected override void UpdateGridPositionAndAngle(double deltaTime)
		{
			if (base.Speed != 0f)
			{
				_travelledDistance += base.Speed * (float)deltaTime;
				float num = _travelledDistance / _spline.ArcLength;
				Vector3 uniformPoint = _spline.GetUniformPoint(num);
				Vector3 vector = _spline.GetUniformDirection(num) * Mathf.Sign(base.Speed);
				float num2 = Vector3.Angle(Vector3.up, vector);
				if (vector.x < 0f)
				{
					num2 = 360f - num2;
				}
				_gridPosition = _isometricGrid.GetGridPointForWorldPosition(uniformPoint);
				_angle = num2;
				bool flag = base.Speed < 0f;
				if ((flag && num <= 0f) || (!flag && num >= 1f))
				{
					base.Speed = 0f;
					FirePathCompletedEvent(flag);
				}
			}
		}
	}
}
