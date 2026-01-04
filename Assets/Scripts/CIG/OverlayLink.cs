using UnityEngine;

namespace CIG
{
	public class OverlayLink : MonoBehaviour
	{
		private Renderer _renderer;

		private Collider2D _collider;

		public RectTransform OverlayParent
		{
			get;
			private set;
		}

		private Renderer CachedRenderer
		{
			get
			{
				if (_renderer == null)
				{
					_renderer = base.gameObject.GetComponent<Renderer>();
				}
				return _renderer;
			}
		}

		private Collider2D CachedCollider
		{
			get
			{
				if (_collider == null)
				{
					_collider = base.gameObject.GetComponent<Collider2D>();
				}
				return _collider;
			}
		}

		private void OnDestroy()
		{
			if (OverlayParent != null)
			{
				UnityEngine.Object.Destroy(OverlayParent.gameObject);
			}
		}

		private void Update()
		{
			if (base.transform.hasChanged)
			{
				UpdateOverlayPosition();
				base.transform.hasChanged = false;
			}
		}

		private void OnEnable()
		{
			if (OverlayParent != null && !OverlayParent.gameObject.activeSelf)
			{
				OverlayParent.gameObject.SetActive(value: true);
			}
			UpdateOverlayPosition();
		}

		private void OnDisable()
		{
			if (OverlayParent != null && OverlayParent.gameObject.activeSelf)
			{
				OverlayParent.gameObject.SetActive(value: false);
			}
		}

		public void Initialize(RectTransform overlayParent)
		{
			OverlayParent = overlayParent;
		}

		private void UpdateOverlayPosition()
		{
			if (!(OverlayParent != null))
			{
				return;
			}
			Bounds bounds;
			if (CachedRenderer != null)
			{
				bounds = CachedRenderer.bounds;
			}
			else
			{
				if (!(CachedCollider != null))
				{
					UnityEngine.Debug.LogError("Was unable to update overlay position because the object it is linked to has not Renderer or Collider2D.");
					return;
				}
				bounds = CachedCollider.bounds;
			}
			OverlayParent.position = bounds.center;
			OverlayParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.extents.x * 2f);
			OverlayParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.extents.y * 2f);
		}
	}
}
