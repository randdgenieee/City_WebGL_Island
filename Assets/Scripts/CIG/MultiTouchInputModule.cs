using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class MultiTouchInputModule : StandaloneInputModule
	{
		private struct VelocityMeasurement
		{
			public Vector2 Position;

			public float Time;
		}

		private const int MaxVelocityMeasurementCount = 3;

		private const float ScrollScaleFactor = 0.1f;

		private PinchEventData _pinchEventData;

		private VelocityMeasurement[] _velocityMeasurements = new VelocityMeasurement[3];

		private int _velocityMeasurementStart;

		private int _velocityMeasurementCount;

		private Dictionary<int, PointerEventData> _dragPointerData = new Dictionary<int, PointerEventData>();

		private bool _hasTouch;

		public static float PreviousProcessedInputTime
		{
			get;
			private set;
		}

		public static float ScrollDelta => Input.mouseScrollDelta.y;

		protected override void Awake()
		{
			base.Awake();
			_pinchEventData = new PinchEventData(base.eventSystem);
		}

		protected override void Start()
		{
			base.Start();
			_hasTouch = true;
		}

		public static void Execute(IBeginPinchHandler handler, BaseEventData pinchEvent)
		{
			handler.OnBeginPinch((PinchEventData)pinchEvent);
		}

		public static void Execute(IPinchHandler handler, BaseEventData pinchEvent)
		{
			handler.OnPinch((PinchEventData)pinchEvent);
		}

		public static void Execute(IEndPinchHandler handler, BaseEventData pinchEvent)
		{
			handler.OnEndPinch((PinchEventData)pinchEvent);
		}

		public override void Process()
		{
			base.Process();
			if (_hasTouch && UnityEngine.Input.touchCount > 0)
			{
				ProcessTouchInput();
				ProcessTouchPinch();
			}
		}

		private Vector2 FindPinchCenter(ICollection<PointerEventData> pointerEvents)
		{
			Vector2 vector = Vector2.zero;
			int num = 0;
			foreach (PointerEventData pointerEvent in pointerEvents)
			{
				vector += pointerEvent.position;
				if (++num == 2)
				{
					break;
				}
			}
			if (num > 0)
			{
				vector /= num;
			}
			return vector;
		}

		private float FindPinchDistance(ICollection<PointerEventData> pointerEvents)
		{
			if (pointerEvents.Count >= 2)
			{
				IEnumerator<PointerEventData> enumerator = pointerEvents.GetEnumerator();
				enumerator.MoveNext();
				Vector2 position = enumerator.Current.position;
				enumerator.MoveNext();
				return Vector2.Distance(position, enumerator.Current.position);
			}
			return 0f;
		}

		private void ResetVelocityMeasurements()
		{
			_velocityMeasurementStart = 0;
			_velocityMeasurementCount = 0;
		}

		private void AddVelocityMeasurement(Vector2 position, float time)
		{
			if (_velocityMeasurementCount == 3)
			{
				_velocityMeasurementStart = (_velocityMeasurementStart + 1) % 3;
				_velocityMeasurementCount--;
			}
			int num = (_velocityMeasurementStart + _velocityMeasurementCount) % 3;
			_velocityMeasurements[num].Position = position;
			_velocityMeasurements[num].Time = time;
			_velocityMeasurementCount++;
		}

		private Vector2 GetVelocity()
		{
			if (_velocityMeasurementCount <= 1)
			{
				return Vector2.zero;
			}
			int num = (_velocityMeasurementStart + _velocityMeasurementCount - 1) % 3;
			Vector2 a = _velocityMeasurements[num].Position - _velocityMeasurements[_velocityMeasurementStart].Position;
			float num2 = _velocityMeasurements[num].Time - _velocityMeasurements[_velocityMeasurementStart].Time;
			if (Mathf.Approximately(num2, 0f))
			{
				return Vector2.zero;
			}
			return a / num2;
		}

		private void ProcessTouchInput()
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				PreviousProcessedInputTime = Time.unscaledTime;
				GetPointerData(touches[i].fingerId, out PointerEventData data, create: false);
				if (data == null)
				{
					if (_dragPointerData.ContainsKey(touches[i].fingerId))
					{
						_dragPointerData.Remove(touches[i].fingerId);
						_pinchEventData.Pointers = _dragPointerData.Count;
						_pinchEventData.Center = FindPinchCenter(_dragPointerData.Values);
						_pinchEventData.Delta = Vector2.zero;
						_pinchEventData.Distance = FindPinchDistance(_dragPointerData.Values);
						_pinchEventData.ScaleDelta = 1f;
						if (_pinchEventData.Pointers == 0)
						{
							_pinchEventData.Reset();
							ExecuteEvents.Execute<IEndPinchHandler>(_pinchEventData.Target, _pinchEventData, Execute);
						}
						else
						{
							ResetVelocityMeasurements();
						}
					}
					continue;
				}
				if (touches[i].phase == TouchPhase.Began && data.pointerDrag == null)
				{
					data.pointerDrag = ExecuteEvents.GetEventHandler<IPinchHandler>(data.pointerCurrentRaycast.gameObject);
				}
				if (data.dragging && !_dragPointerData.ContainsKey(data.pointerId))
				{
					_dragPointerData.Add(data.pointerId, data);
					_pinchEventData.Pointers = _dragPointerData.Count;
					_pinchEventData.Center = FindPinchCenter(_dragPointerData.Values);
					_pinchEventData.Delta = Vector2.zero;
					_pinchEventData.Velocity = Vector2.zero;
					_pinchEventData.Distance = FindPinchDistance(_dragPointerData.Values);
					_pinchEventData.ScaleDelta = 1f;
					ResetVelocityMeasurements();
					if (_pinchEventData.Pointers == 1)
					{
						_pinchEventData.Target = data.pointerPressRaycast.gameObject;
						if (_pinchEventData.Target != null)
						{
							_pinchEventData.Target = ExecuteEvents.GetEventHandler<IPinchHandler>(_pinchEventData.Target);
						}
						_pinchEventData.Reset();
						ExecuteEvents.Execute<IBeginPinchHandler>(_pinchEventData.Target, _pinchEventData, Execute);
					}
				}
				if (data.dragging && data.eligibleForClick)
				{
					data.eligibleForClick = false;
				}
			}
		}

		private void ProcessTouchPinch()
		{
			Vector2 center = _pinchEventData.Center;
			float distance = _pinchEventData.Distance;
			if (_pinchEventData.Pointers <= 0)
			{
				return;
			}
			_pinchEventData.Center = FindPinchCenter(_dragPointerData.Values);
			_pinchEventData.Distance = FindPinchDistance(_dragPointerData.Values);
			AddVelocityMeasurement(_pinchEventData.Center, Time.unscaledTime);
			if (_pinchEventData.Center != center || !Mathf.Approximately(_pinchEventData.Distance, distance))
			{
				_pinchEventData.Delta = _pinchEventData.Center - center;
				_pinchEventData.Velocity = GetVelocity();
				if (Mathf.Abs(distance) > 1E-09f && Mathf.Abs(_pinchEventData.Distance) > 1E-09f)
				{
					_pinchEventData.ScaleDelta = _pinchEventData.Distance / distance;
				}
				else
				{
					_pinchEventData.ScaleDelta = 1f;
				}
				_pinchEventData.Reset();
				ExecuteEvents.Execute<IPinchHandler>(_pinchEventData.Target, _pinchEventData, Execute);
			}
		}
	}
}
