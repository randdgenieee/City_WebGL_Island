using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	[RequireComponent(typeof(Canvas))]
	[ExecuteInEditMode]
	[AddComponentMenu("Layout/CIG Canvas Scaler", 101)]
	public class CIGCanvasScaler : UIBehaviour
	{
		public enum ScaleMode
		{
			ConstantPixelSize = 0,
			ScaleWithScreenSize = 1,
			ConstantPhysicalSize = 2,
			ScaleWithDPIScreenSize = 99
		}

		private const float LogBase = 2f;

		[Tooltip("The Canvas to which the Scale Factor and Reference PPU will be applied.")]
		[SerializeField]
		private Canvas _canvas;

		[Tooltip("Determines how UI elements in the Canvas are scaled.")]
		[SerializeField]
		private ScaleMode _uiScaleMode = ScaleMode.ScaleWithDPIScreenSize;

		[Tooltip("If a sprite has this 'Pixels Per Unit' setting, then one pixel in the sprite will cover one unit in the UI.")]
		[SerializeField]
		private float _referencePixelsPerUnit = 100f;

		[Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the UI will be scaled up, and if it's smaller, the UI will be scaled down. This is done in accordance with the Screen Match Mode.")]
		[SerializeField]
		private Vector2 _referenceResolution = new Vector2(1280f, 720f);

		[Tooltip("The DPI the UI layout is designed for. Or to use when the screen DPI is not known.")]
		[SerializeField]
		private float _referenceDPI = 96f;

		[Tooltip("Determines if the scaling is using the width or height as reference, or a mix in between.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float _matchWidthOrHeight = 1f;

		[Tooltip("Scales all UI elements in the Canvas by this factor. (Only used for Constant Pixel and Physical Size)")]
		[SerializeField]
		private float _scaleFactor = 1f;

		[Tooltip("Determines if the Large Screen Threshold formula is using the width or height as reference, or a mix in between.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float _largeScreenMatchWidthOrHeight = 1f;

		[Tooltip("The Threshold (in Inches) used to determine if a screen is considered large enough to have its scaling factor dampened.")]
		[SerializeField]
		private float _largeScreenThresholdInches = 2.69f;

		[Tooltip("The Dampening Factor used when calculating the scaling factor for Large Screens.")]
		[SerializeField]
		private float _largeScreenDampeningFactor = 0.025f;

		[Tooltip("The scale of the canvas will never go below this value.")]
		[SerializeField]
		private float _minimumCanvasScale = 0.25f;

		private float _prevScaleFactor = 1f;

		private float _prevReferencePixelsPerUnit = 100f;

		public ScaleMode UIScaleMode
		{
			get
			{
				return _uiScaleMode;
			}
			set
			{
				_uiScaleMode = value;
			}
		}

		public float ReferencePixelsPerUnit
		{
			get
			{
				return _referencePixelsPerUnit;
			}
			set
			{
				_referencePixelsPerUnit = value;
			}
		}

		public float MatchWidthOrHeight
		{
			get
			{
				return _matchWidthOrHeight;
			}
			set
			{
				_matchWidthOrHeight = value;
			}
		}

		public float ReferenceDPI
		{
			get
			{
				return _referenceDPI;
			}
			set
			{
				_referenceDPI = value;
			}
		}

		public float ScaleFactor
		{
			get
			{
				return _scaleFactor;
			}
			set
			{
				_scaleFactor = value;
			}
		}

		public float LargeScreenMatchWidthOrHeight
		{
			get
			{
				return _largeScreenMatchWidthOrHeight;
			}
			set
			{
				_largeScreenMatchWidthOrHeight = value;
			}
		}

		public float LargeScreenThresholdInches
		{
			get
			{
				return _largeScreenThresholdInches;
			}
			set
			{
				_largeScreenThresholdInches = value;
			}
		}

		public float MinimumCanvasScale
		{
			get
			{
				return _minimumCanvasScale;
			}
			set
			{
				_minimumCanvasScale = value;
			}
		}

		public float LargeScreenDampeningFactor
		{
			get
			{
				return _largeScreenDampeningFactor;
			}
			set
			{
				_largeScreenDampeningFactor = value;
			}
		}

		public float CanvasScaleFactor => _canvas.scaleFactor;

		protected override void OnEnable()
		{
			base.OnEnable();
			Handle();
		}

		protected override void OnDisable()
		{
			SetScaleFactor(1f);
			SetReferencePixelsPerUnit(100f);
			base.OnDisable();
		}

		private void Update()
		{
			Handle();
		}

		private void Handle()
		{
			if (!(_canvas == null) && _canvas.isRootCanvas && _canvas.renderMode != RenderMode.WorldSpace)
			{
				switch (_uiScaleMode)
				{
				case ScaleMode.ConstantPixelSize:
					HandleConstantPixelSize();
					break;
				case ScaleMode.ScaleWithScreenSize:
					HandleScaleWithScreenSize();
					break;
				case ScaleMode.ConstantPhysicalSize:
					HandleConstantPhysicalSize();
					break;
				case ScaleMode.ScaleWithDPIScreenSize:
					HandleScaleWithDPIScreenSize();
					break;
				}
			}
		}

		private void HandleConstantPixelSize()
		{
			SetScaleFactor(_scaleFactor);
			SetReferencePixelsPerUnit(_referencePixelsPerUnit);
		}

		private void HandleScaleWithScreenSize()
		{
			float scaleFactor = CalculateScreenSizeScaleFactor();
			SetScaleFactor(scaleFactor);
			SetReferencePixelsPerUnit(_referencePixelsPerUnit);
		}

		private float CalculateScreenSizeScaleFactor()
		{
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			float a = Mathf.Log(vector.x / _referenceResolution.x, 2f);
			float b = Mathf.Log(vector.y / _referenceResolution.y, 2f);
			float p = Mathf.Lerp(a, b, _matchWidthOrHeight);
			return Mathf.Pow(2f, p);
		}

		private void HandleConstantPhysicalSize()
		{
			float scaleFactor = CIGGameConstants.CurrentDPI / _referenceDPI * _scaleFactor;
			SetScaleFactor(scaleFactor);
			SetReferencePixelsPerUnit(_referencePixelsPerUnit);
		}

		private void HandleScaleWithDPIScreenSize()
		{
			float num = CalculateScreenSizeScaleFactor();
			float num2 = Mathf.Lerp(Screen.width, Screen.height, _matchWidthOrHeight) / CIGGameConstants.CurrentDPI;
			float num3 = 1f;
			if (num2 > _largeScreenThresholdInches)
			{
				float num4 = num2 - _largeScreenThresholdInches;
				num3 -= num4 * _largeScreenDampeningFactor;
			}
			float b = num * num3;
			b = Mathf.Max(_minimumCanvasScale, b);
			SetScaleFactor(b);
			SetReferencePixelsPerUnit(_referencePixelsPerUnit);
		}

		private void SetScaleFactor(float scaleFactor)
		{
			if (scaleFactor != _prevScaleFactor)
			{
				_canvas.scaleFactor = scaleFactor;
				_prevScaleFactor = scaleFactor;
			}
		}

		private void SetReferencePixelsPerUnit(float referencePixelsPerUnit)
		{
			if (referencePixelsPerUnit != _prevReferencePixelsPerUnit)
			{
				_canvas.referencePixelsPerUnit = referencePixelsPerUnit;
				_prevReferencePixelsPerUnit = referencePixelsPerUnit;
			}
		}
	}
}
