using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public sealed class OverlayManager : MonoBehaviour
	{
		public delegate void InteractionEnabledChangedEventHandler(bool interactionEnabled);

		[SerializeField]
		private GraphicRaycaster _raycaster;

		private readonly List<object> _disableInteractionRequestSources = new List<object>();

		private WorldMapView _worldMapView;

		public bool InteractionDisabled => _disableInteractionRequestSources.Count > 0;

		public event InteractionEnabledChangedEventHandler InteractionEnabledChangedEvent;

		private void FireInteractionEnabledChangedEvent(bool interactionEnabled)
		{
			this.InteractionEnabledChangedEvent?.Invoke(interactionEnabled);
		}

		public Overlay CreateOverlay(GameObject linkedTo, Overlay overlayPrefab)
		{
			OverlayLink overlayLink = linkedTo.GetComponent<OverlayLink>();
			if (overlayLink == null)
			{
				overlayLink = linkedTo.AddComponent<OverlayLink>();
				GameObject gameObject = new GameObject("Overlay(" + linkedTo.name + ")", typeof(RectTransform));
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				overlayLink.Initialize((RectTransform)gameObject.transform);
			}
			Overlay overlay = UnityEngine.Object.Instantiate(overlayPrefab, overlayLink.OverlayParent, worldPositionStays: false);
			overlay.transform.rotation = overlayLink.transform.rotation;
			if (overlay.GetComponent<RectTransform>() == null)
			{
				UnityEngine.Debug.LogError("OverlayManager was unable to find 'RectTransform' component in '" + overlayPrefab.name + "' prefab!");
				UnityEngine.Object.Destroy(overlay.gameObject);
				return null;
			}
			return overlay;
		}

		public T CreateOverlay<T>(GameObject linkedTo, OverlayType overlayType) where T : Overlay
		{
			Overlay asset = SingletonMonobehaviour<OverlayAssetCollection>.Instance.GetAsset(overlayType);
			return CreateOverlay<T>(linkedTo, asset);
		}

		public T CreateOverlay<T>(GameObject linkedTo, Overlay overlayPrefab) where T : Overlay
		{
			T component = CreateOverlay(linkedTo, overlayPrefab).GetComponent<T>();
			if ((Object)component == (Object)null)
			{
				UnityEngine.Debug.LogError("Overlay prefab '" + overlayPrefab.name + "' does not have a 'T' component ");
				return null;
			}
			return component;
		}

		public Pling CreatePling(Pling plingPrefab, Vector3 position)
		{
			return UnityEngine.Object.Instantiate(plingPrefab, position, Quaternion.identity, base.transform);
		}

		public void DisableInteractionRequest(object source)
		{
			DisableInteraction();
			_disableInteractionRequestSources.Add(source);
		}

		public void EnableInteractionRequest(object source)
		{
			_disableInteractionRequestSources.Remove(source);
			if (_disableInteractionRequestSources.Count == 0)
			{
				EnableInteraction();
			}
		}

		private void EnableInteraction()
		{
			_raycaster.enabled = true;
			FireInteractionEnabledChangedEvent(interactionEnabled: true);
		}

		private void DisableInteraction()
		{
			_raycaster.enabled = false;
			FireInteractionEnabledChangedEvent(interactionEnabled: false);
		}
	}
}
