using CIG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class WorldMapCameraOperator : CameraOperator, IPointerClickHandler, IEventSystemHandler
{
	private const float DragInteractingThreshold = 20f;

	private const string DraggableWorldMapTag = "DraggableWorldMap";

	private const int VelocitySampleMaxCount = 4;

	public const float SoftMinZoom = 2000f;

	public const float SoftMaxZoom = 800f;

	public const float SoftCenterZoom = 1400f;

	private const float MinZoom = 2500f;

	private const float MaxZoom = 600f;

	private static readonly Plane Plane = new Plane(Vector3.back, 0f);

	[SerializeField]
	private Camera _cameraToOperate;

	[SerializeField]
	private Transform _cameraPanTransform;

	[SerializeField]
	private Transform _cameraZTransform;

	[SerializeField]
	private float _zoomElasticity = 0.6f;

	[SerializeField]
	private BoxCollider2D _cameraBounds;

	[SerializeField]
	private EventSystem _eventSystem;

	private float _totalFingerDragDistance;

	private bool _startedClickingOnUI;

	private readonly List<Vector2> _previousTouches = new List<Vector2>();

	private Vector2 _previousDragPosition;

	private readonly LinkedList<Vector3> _velocitySamples = new LinkedList<Vector3>();

	private bool _inputEnabled = true;

	public Camera CameraToOperate => _cameraToOperate;

	public EventSystem EventSystem => _eventSystem;

	public bool Interacting
	{
		get;
		private set;
	}

	protected override float Zoom
	{
		get
		{
			return 0f - _cameraZTransform.localPosition.z;
		}
		set
		{
			Vector3 localPosition = _cameraZTransform.localPosition;
			localPosition.z = 0f - value;
			_cameraZTransform.localPosition = localPosition;
			RestrictCameraBounds();
		}
	}

	protected override Vector2 CameraPosition
	{
		get
		{
			return _cameraPanTransform.position;
		}
		set
		{
			Vector3 position = _cameraPanTransform.position;
			position.x = value.x;
			position.y = value.y;
			_cameraPanTransform.position = position;
		}
	}

	public void Initialize(Timing timing)
	{
		InitializeCameraOperator(timing);
		Interacting = false;
		RestrictCameraBounds();
	}

	private void Update()
	{
		if (_inputEnabled)
		{
			if (UnityEngine.Input.touchCount > 0 || _previousTouches.Count > 0)
			{
				HandleTouches();
			}
			else
			{
				HandleMouse();
			}
			FillPreviousTouches();
		}
	}

	private void LateUpdate()
	{
		RestrictCameraZoom(_timing.GetDeltaTime(DeltaTimeType.Animation));
		bool flag = UnityEngine.Input.touchCount > 0 || Input.GetMouseButton(0);
		Interacting = (flag && _totalFingerDragDistance > 20f);
		if (!flag)
		{
			_totalFingerDragDistance = 0f;
		}
		if (flag || _animationMode != 0)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}
		else
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		if (HandleAnimations())
		{
			RestrictCameraBounds();
		}
	}

	private void OnEnable()
	{
		Interacting = false;
		_totalFingerDragDistance = 0f;
		_animationMode = AnimationMode.None;
		_startedClickingOnUI = false;
	}

	private void OnDisable()
	{
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Interacting)
		{
			GameEvents.Invoke(new UnemiShouldCloseEvent(this));
		}
	}

	protected override void OnVelocityAnimation(Vector3 velocity, float deltaTime)
	{
		_cameraPanTransform.localPosition += velocity * deltaTime;
	}

	protected override void OnDestinationAnimation(float progress, bool inProgress, Vector2 fromPosition, Vector2 toPosition)
	{
		_velocitySamples.Clear();
		Vector3 position = _cameraPanTransform.position;
		if (inProgress)
		{
			position = Vector2.Lerp(fromPosition, toPosition, progress);
		}
		else
		{
			position.x = toPosition.x;
			position.y = toPosition.y;
		}
		_cameraPanTransform.position = position;
	}

	protected override void OnZoomAnimation(float progress, bool inProgress, float fromZoom, float toZoom)
	{
		Zoom = (inProgress ? Mathf.Lerp(fromZoom, toZoom, progress) : toZoom);
	}

	public void DisableInput()
	{
		Interacting = false;
		_inputEnabled = false;
		_totalFingerDragDistance = 0f;
		_startedClickingOnUI = false;
	}

	public void EnableInput()
	{
		_inputEnabled = true;
	}

	private void HandleTouches()
	{
		int touchCount = UnityEngine.Input.touchCount;
		int count = _previousTouches.Count;
		bool flag = false;
		for (int i = 0; i < touchCount; i++)
		{
			Touch touch = UnityEngine.Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				flag = true;
				_startedClickingOnUI |= UIRaycast(touch.position);
			}
		}
		if (flag && !_startedClickingOnUI)
		{
			_animationMode = AnimationMode.None;
		}
		switch (touchCount)
		{
		case 0:
			if (count > 0 && !_startedClickingOnUI)
			{
				SetVelocity();
			}
			_startedClickingOnUI = false;
			return;
		case 1:
		{
			Touch touch2 = UnityEngine.Input.GetTouch(0);
			if (!_startedClickingOnUI)
			{
				Drag(touch2.position, count != 1 || touch2.phase == TouchPhase.Began);
			}
			return;
		}
		}
		Touch touch3 = UnityEngine.Input.GetTouch(0);
		Touch touch4 = UnityEngine.Input.GetTouch(1);
		bool flag2 = touchCount != count || touch3.phase == TouchPhase.Began || touch4.phase == TouchPhase.Began;
		Vector2 position = (touch3.position + touch4.position) * 0.5f;
		if (!_startedClickingOnUI)
		{
			Drag(position, flag2);
			if (!flag2)
			{
				PinchZoom(touch3.position, touch4.position);
			}
		}
	}

	private void HandleMouse()
	{
		if (Input.GetMouseButtonUp(0))
		{
			if (!_startedClickingOnUI)
			{
				SetVelocity();
			}
		}
		else if (Input.GetMouseButton(0))
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			if (mouseButtonDown)
			{
				_startedClickingOnUI = UIRaycast(UnityEngine.Input.mousePosition);
			}
			if (!_startedClickingOnUI)
			{
				_animationMode = AnimationMode.None;
				Drag(UnityEngine.Input.mousePosition, mouseButtonDown);
			}
		}
		float scrollDelta = MultiTouchInputModule.ScrollDelta;
		if (!Mathf.Approximately(0f, scrollDelta))
		{
			ZoomWith(1f - 0.1f * scrollDelta);
		}
	}

	private bool UIRaycast(Vector2 pointerPosition)
	{
		PointerEventData pointerEventData = new PointerEventData(_eventSystem);
		pointerEventData.position = pointerPosition;
		List<RaycastResult> list = new List<RaycastResult>();
		_eventSystem.RaycastAll(pointerEventData, list);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].gameObject.CompareTag("DraggableWorldMap"))
			{
				list.RemoveAt(num);
			}
		}
		return list.Count > 0;
	}

	private void Drag(Vector2 position, bool pressed)
	{
		if (!pressed)
		{
			Vector3? vector = Raycast(position);
			Vector3? vector2 = Raycast(_previousDragPosition);
			if (vector.HasValue && vector2.HasValue)
			{
				Vector3 vector3 = vector2.Value - vector.Value;
				vector3.z = 0f;
				_cameraPanTransform.localPosition += vector3;
				_velocitySamples.AddLast(vector3 / _timing.GetDeltaTime(DeltaTimeType.Animation));
				if (_velocitySamples.Count > 4)
				{
					_velocitySamples.RemoveFirst();
				}
			}
			_totalFingerDragDistance += Vector2.Distance(_previousDragPosition, position);
		}
		else
		{
			_velocitySamples.Clear();
		}
		_previousDragPosition = position;
		RestrictCameraBounds();
	}

	private void SetVelocity()
	{
		if (_velocitySamples.Count != 0)
		{
			Vector3 a = Vector3.zero;
			foreach (Vector3 velocitySample in _velocitySamples)
			{
				a += velocitySample;
			}
			a /= _velocitySamples.Count;
			if (_animationMode == AnimationMode.None && a.magnitude >= 30f)
			{
				SetVelocityAnimation(a);
			}
		}
	}

	private void PinchZoom(Vector2 positionA, Vector2 positionB)
	{
		Vector2 vector = _previousTouches[0];
		Vector2 b = _previousTouches[1];
		float magnitude = (positionA - positionB).magnitude;
		float magnitude2 = (vector - b).magnitude;
		_totalFingerDragDistance += Vector2.Distance(positionA, vector);
		_totalFingerDragDistance += Vector2.Distance(positionB, b);
		ZoomWith(magnitude2 / magnitude);
	}

	private void ZoomWith(float multiplier)
	{
		Zoom *= multiplier;
	}

	private Vector3? Raycast(Vector2 position)
	{
		Ray ray = _cameraToOperate.ScreenPointToRay(position);
		if (Plane.Raycast(ray, out float enter))
		{
			return ray.GetPoint(enter);
		}
		return null;
	}

	private void FillPreviousTouches()
	{
		_previousTouches.Clear();
		int i = 0;
		for (int touchCount = UnityEngine.Input.touchCount; i < touchCount; i++)
		{
			_previousTouches.Add(UnityEngine.Input.GetTouch(i).position);
		}
	}

	private void RestrictCameraZoom(float deltaTime)
	{
		float num = Zoom;
		if (!Interacting)
		{
			float num2 = CalculateZoomOffset(num);
			if (!Mathf.Approximately(num2, 0f))
			{
				float currentVelocity = 0f;
				num = Mathf.SmoothDamp(num, num + num2, ref currentVelocity, _zoomElasticity, float.PositiveInfinity, deltaTime);
				num += currentVelocity;
			}
		}
		num = Mathf.Clamp(num, 600f, 2500f);
		if (!Mathf.Approximately(num, Zoom))
		{
			Zoom = num;
		}
	}

	private void RestrictCameraBounds()
	{
		Rect pixelRect = _cameraToOperate.pixelRect;
		Vector3? vector = Raycast(pixelRect.min);
		Vector3? vector2 = Raycast(new Vector2(pixelRect.xMax, pixelRect.yMin));
		Vector3? vector3 = Raycast(new Vector2(pixelRect.xMin, pixelRect.yMax));
		Vector3? vector4 = Raycast(pixelRect.max);
		if (vector.HasValue && vector2.HasValue && vector3.HasValue && vector4.HasValue)
		{
			Vector3 b = new Vector2(Mathf.Min(vector.Value.x, vector3.Value.x), Mathf.Min(vector.Value.y, vector2.Value.y));
			Vector3 b2 = new Vector2(Mathf.Max(vector2.Value.x, vector4.Value.x), Mathf.Max(vector3.Value.y, vector4.Value.y));
			Bounds bounds = _cameraBounds.bounds;
			Vector3 position = _cameraPanTransform.position;
			Vector3 vector5 = bounds.min - b;
			Vector3 vector6 = bounds.max - b2;
			position.x += Mathf.Max(0f, vector5.x);
			position.y += Mathf.Max(0f, vector5.y);
			position.x += Mathf.Min(0f, vector6.x);
			position.y += Mathf.Min(0f, vector6.y);
			_cameraPanTransform.position = position;
		}
	}

	private float CalculateZoomOffset(float zoom)
	{
		float result = 0f;
		if (zoom < 800f)
		{
			result = 800f - zoom;
		}
		else if (zoom > 2000f)
		{
			result = 2000f - zoom;
		}
		return result;
	}
}
