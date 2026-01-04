using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class IslandCameraOperator : CameraOperator
	{
		public const float MinZoom = 1000f;

		public const float DefaultZoomSize = 650f;

		public const float HighDPIScreenMaxZoom = 250f;

		public const float LowDPIScreenMinZoom = 400f;

		private const float ZoomElasticity = 0.6f;

		private const float ZoomStiffness = 1.5f;

		[SerializeField]
		private Camera _cameraToOperate;

		[SerializeField]
		private BoxCollider2D _cameraBounds;

		private IslandInput _islandInput;

		private CIGExpansions _expansions;

		private IsometricGrid _isometricGrid;

		private bool _pinching;

		private float _pinchingOrthoSize;

		private float _cameraZ;

		private float _centerZoom;

		private bool _inputEnabled = true;

		private readonly List<object> _disableInputRequests = new List<object>();

		public Camera CameraToOperate => _cameraToOperate;

		public float MaxZoom
		{
			get;
			private set;
		}

		protected override float Zoom
		{
			get
			{
				return _cameraToOperate.orthographicSize;
			}
			set
			{
				_cameraToOperate.orthographicSize = value;
				RestrictCameraBounds();
			}
		}

		protected override Vector2 CameraPosition
		{
			get
			{
				return _cameraToOperate.transform.position;
			}
			set
			{
				Vector3 position = _cameraToOperate.transform.position;
				position.x = value.x;
				position.y = value.y;
				_cameraToOperate.transform.position = position;
			}
		}

		public void Initialize(IslandInput islandInput, Bounds islandBounds, CIGExpansions expansions, IsometricGrid isometricGrid, Timing timing)
		{
			InitializeCameraOperator(timing);
			_islandInput = islandInput;
			_expansions = expansions;
			_isometricGrid = isometricGrid;
			_pinching = false;
			_cameraZ = _cameraToOperate.transform.position.z;
			MaxZoom = SetCameraMaxZoom();
			_centerZoom = (1000f + MaxZoom) / 2f;
			SetCameraBounds(islandBounds);
			SetInitialCameraPosition();
			_islandInput.BeginPinchEvent += OnBeginPinch;
			_islandInput.PinchEvent += OnPinch;
			_islandInput.EndPinchEvent += OnEndPinch;
			_islandInput.PointerDownEvent += OnPointerDown;
		}

		private void LateUpdate()
		{
			float deltaTime = _timing.GetDeltaTime(DeltaTimeType.Animation);
			SoftRestrictCameraZoom(deltaTime);
			if (HandleAnimations())
			{
				RestrictCameraBounds();
			}
		}

		private void OnDestroy()
		{
			if (_islandInput != null)
			{
				_islandInput.BeginPinchEvent -= OnBeginPinch;
				_islandInput.PinchEvent -= OnPinch;
				_islandInput.EndPinchEvent -= OnEndPinch;
				_islandInput.PointerDownEvent -= OnPointerDown;
				_islandInput = null;
			}
			if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
			{
				SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			}
		}

		private void OnPointerDown(PointerEventData eventData)
		{
			if (_inputEnabled)
			{
				_animationMode = AnimationMode.None;
			}
		}

		private void OnBeginPinch(PinchEventData pinchEvent)
		{
			if (_inputEnabled)
			{
				_pinching = true;
				_pinchingOrthoSize = _cameraToOperate.orthographicSize;
				if (_animationMode != 0)
				{
					_animationMode = AnimationMode.None;
				}
				else
				{
					SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
				}
			}
		}

		private void OnPinch(PinchEventData pinchEvent)
		{
			if (_inputEnabled)
			{
				_pinchingOrthoSize /= pinchEvent.ScaleDelta;
				float num = CalculateZoomOffset(_pinchingOrthoSize);
				float num2 = 1000f - MaxZoom;
				if (num < 0f)
				{
					num2 /= 2f;
				}
				float num3 = DampenOffset(num, num2);
				float orthographicSize = _pinchingOrthoSize + num - num3;
				Transform transform = _cameraToOperate.transform;
				float num4 = _cameraToOperate.orthographicSize * 2f;
				float num5 = num4 * _cameraToOperate.aspect;
				Vector2 zero = Vector2.zero;
				zero.x = transform.localPosition.x - num5 * 0.5f + pinchEvent.Center.x / (float)_cameraToOperate.pixelWidth * num5;
				zero.y = transform.localPosition.y - num4 * 0.5f + pinchEvent.Center.y / (float)_cameraToOperate.pixelHeight * num4;
				Vector3 zero2 = Vector3.zero;
				zero2.x = (0f - pinchEvent.Delta.x) / (float)_cameraToOperate.pixelWidth * num5;
				zero2.y = (0f - pinchEvent.Delta.y) / (float)_cameraToOperate.pixelHeight * num4;
				zero2.x += (transform.localPosition.x - zero.x) * (1f - pinchEvent.ScaleDelta);
				zero2.y += (transform.localPosition.y - zero.y) * (1f - pinchEvent.ScaleDelta);
				transform.localPosition += zero2;
				_cameraToOperate.orthographicSize = orthographicSize;
				RestrictCameraBounds();
			}
		}

		private void OnEndPinch(PinchEventData pinchEvent)
		{
			if (_inputEnabled)
			{
				if (_animationMode == AnimationMode.None && pinchEvent.Velocity.magnitude >= 30f)
				{
					SetVelocityAnimation(pinchEvent.Velocity);
				}
				else if (pinchEvent.Pointers == 0)
				{
					SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
				}
				_pinching = false;
			}
		}

		protected override void OnVelocityAnimation(Vector3 velocity, float deltaTime)
		{
			float num = _cameraToOperate.orthographicSize * 2f;
			float num2 = num * _cameraToOperate.aspect;
			Vector3 zero = Vector3.zero;
			zero.x = (0f - velocity.x) * deltaTime / (float)_cameraToOperate.pixelWidth * num2;
			zero.y = (0f - velocity.y) * deltaTime / (float)_cameraToOperate.pixelHeight * num;
			_cameraToOperate.transform.localPosition += zero;
		}

		protected override void OnDestinationAnimation(float progress, bool inProgress, Vector2 fromPosition, Vector2 toPosition)
		{
			Vector3 position = _cameraToOperate.transform.position;
			if (inProgress)
			{
				position = Vector2.Lerp(fromPosition, toPosition, progress);
				position.z = _cameraZ;
			}
			else
			{
				position.x = toPosition.x;
				position.y = toPosition.y;
			}
			_cameraToOperate.transform.position = position;
		}

		protected override void OnZoomAnimation(float progress, bool inProgress, float fromZoom, float toZoom)
		{
			Zoom = (inProgress ? Mathf.Lerp(fromZoom, toZoom, progress) : toZoom);
		}

		public void AlignWithCamera(Camera camera)
		{
			_cameraToOperate.transform.position = camera.transform.position;
			_cameraToOperate.transform.rotation = camera.transform.rotation;
			_cameraToOperate.orthographicSize = camera.orthographicSize;
			RestrictCameraBounds();
		}

		public void PushDisableInputRequest(object obj)
		{
			_disableInputRequests.Add(obj);
			_pinching = false;
			_inputEnabled = false;
		}

		public void PopDisableInputRequest(object obj)
		{
			if (_disableInputRequests.Remove(obj))
			{
				_inputEnabled = (_disableInputRequests.Count == 0);
			}
		}

		private void SoftRestrictCameraZoom(float deltaTime)
		{
			if (!_pinching)
			{
				float orthographicSize = _cameraToOperate.orthographicSize;
				float num = CalculateZoomOffset(orthographicSize);
				if (!Mathf.Approximately(num, 0f))
				{
					float currentVelocity = 0f;
					orthographicSize = Mathf.SmoothDamp(orthographicSize, orthographicSize + num, ref currentVelocity, 0.6f, float.PositiveInfinity, deltaTime);
					orthographicSize += currentVelocity;
					_cameraToOperate.orthographicSize = orthographicSize;
					RestrictCameraBounds();
				}
			}
		}

		private float SetCameraMaxZoom()
		{
			float num = Mathf.Sqrt(2156800f) / 96f;
			float num2 = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / CIGGameConstants.CurrentDPI / num;
			return Mathf.Clamp(650f * num2, 250f, 400f);
		}

		private void SetCameraBounds(Bounds bounds)
		{
			_cameraBounds.size = bounds.size;
			_cameraBounds.offset = bounds.center;
		}

		private void SetInitialCameraPosition()
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			foreach (ExpansionBlock allBlock in _expansions.AllBlocks)
			{
				if (allBlock.Unlocked)
				{
					num += (float)allBlock.Origin.u + (float)allBlock.Size.u * 0.5f;
					num2 += (float)allBlock.Origin.v + (float)allBlock.Size.v * 0.5f;
					num3++;
				}
			}
			if (num3 == 0)
			{
				SetPosition(_cameraBounds.bounds.center);
			}
			else
			{
				num /= (float)num3;
				num2 /= (float)num3;
				SetPosition(_isometricGrid.GetWorldPositionForGridPoint(new GridPoint(num, num2)));
			}
			ZoomTo(_centerZoom, 1000f, 1.2f);
		}

		private void RestrictCameraBounds()
		{
			Transform transform = _cameraToOperate.transform;
			float num = _cameraToOperate.orthographicSize * 2f;
			float num2 = num * _cameraToOperate.aspect;
			Bounds bounds = _cameraBounds.bounds;
			if (num2 > bounds.size.x)
			{
				num2 = bounds.size.x;
				num = num2 / _cameraToOperate.aspect;
				_cameraToOperate.orthographicSize = num * 0.5f;
			}
			if (num > bounds.size.y)
			{
				num = bounds.size.y;
				num2 = num * _cameraToOperate.aspect;
				_cameraToOperate.orthographicSize = num * 0.5f;
			}
			Vector3 position = transform.position;
			if (position.x - num2 * 0.5f < bounds.min.x)
			{
				position.x = bounds.min.x + num2 * 0.5f;
			}
			if (position.x + num2 * 0.5f > bounds.max.x)
			{
				position.x = bounds.max.x - num2 * 0.5f;
			}
			if (position.y - num * 0.5f < bounds.min.y)
			{
				position.y = bounds.min.y + num * 0.5f;
			}
			if (position.y + num * 0.5f > bounds.max.y)
			{
				position.y = bounds.max.y - num * 0.5f;
			}
			transform.position = position;
		}

		private void SetPosition(Vector2 position)
		{
			_animationMode = AnimationMode.None;
			Vector3 position2 = _cameraToOperate.transform.position;
			position2.x = position.x;
			position2.y = position.y;
			_cameraToOperate.transform.position = position2;
			RestrictCameraBounds();
		}

		private float CalculateZoomOffset(float orthoSize)
		{
			float result = 0f;
			if (orthoSize < MaxZoom)
			{
				result = MaxZoom - orthoSize;
			}
			else if (orthoSize > 1000f)
			{
				result = 1000f - orthoSize;
			}
			return result;
		}

		private float DampenOffset(float overStretching, float zoomRange)
		{
			if (Mathf.Approximately(overStretching, 0f))
			{
				return 0f;
			}
			return (1f - 1f / (Mathf.Abs(overStretching) / (1.5f * zoomRange) + 1f)) * zoomRange * Mathf.Sign(overStretching);
		}
	}
}
