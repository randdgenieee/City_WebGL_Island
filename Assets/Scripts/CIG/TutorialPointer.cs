using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class TutorialPointer : MonoBehaviour
	{
		[SerializeField]
		private MaskView _maskView;

		[SerializeField]
		private CIGCanvasScaler _canvasScaler;

		private Camera _uiCamera;

		private WorldMapView _worldMapView;

		private Coroutine _followTransformRoutine;

		private object _owner;

		private bool _showing;

		private readonly List<object> _blockers = new List<object>();

		public void Initialize(Camera uiCamera, WorldMapView worldMapView)
		{
			_uiCamera = uiCamera;
			_worldMapView = worldMapView;
		}

		public void Show(object owner, RectTransform targetTransform, float rotation = 0f, bool clickableMask = true)
		{
			Show(owner, targetTransform, rotation, _uiCamera, (Transform t) => t.position, (Camera c) => GetRectSize(targetTransform, c), clickableMask);
		}

		public void ShowOnWorldMap(object owner, RectTransform targetTransform, float rotation = 0f, bool clickableMask = true)
		{
			Show(owner, targetTransform, rotation, _worldMapView.CameraOperator.CameraToOperate, (Transform t) => t.position, (Camera c) => GetRectSize(targetTransform, c), clickableMask);
		}

		public void ShowOnIsland(object owner, Transform targetTransform, Sprite sprite, float rotation = 0f, bool clickableMask = true)
		{
			Show(owner, targetTransform, rotation, IsometricIsland.Current.CameraOperator.CameraToOperate, (Transform t) => BuildingToUIPosition(t, sprite), (Camera c) => GetSpriteSize(sprite, c), clickableMask);
		}

		public void Hide(object owner)
		{
			if (owner == _owner)
			{
				base.gameObject.SetActive(value: false);
				if (_followTransformRoutine != null)
				{
					StopCoroutine(_followTransformRoutine);
					_followTransformRoutine = null;
				}
				_maskView.Hide();
				_showing = false;
				_owner = null;
			}
		}

		public void RegisterBlocker(object blocker)
		{
			_blockers.Add(blocker);
			base.gameObject.SetActive(value: false);
		}

		public void UnregisterBlocker(object blocker)
		{
			_blockers.Remove(blocker);
			if (_blockers.Count == 0 && _showing)
			{
				base.gameObject.SetActive(value: true);
			}
		}

		private void Show(object owner, Transform targetTransform, float rotation, Camera c, Func<Transform, Vector3> positionFunction, Func<Camera, Vector2> sizeFunction, bool clickableMask)
		{
			if (targetTransform == null)
			{
				Hide(owner);
				return;
			}
			_owner = owner;
			_showing = true;
			if (_blockers.Count == 0)
			{
				base.gameObject.SetActive(value: true);
			}
			base.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
			if (_followTransformRoutine != null)
			{
				StopCoroutine(_followTransformRoutine);
			}
			_followTransformRoutine = StartCoroutine(FollowTransformRoutine(targetTransform, c, positionFunction, sizeFunction, clickableMask));
		}

		private Vector2 GetRectSize(RectTransform targetTransform, Camera c)
		{
			Vector3[] array = new Vector3[4];
			targetTransform.GetWorldCorners(array);
			Vector3[] array2 = array.Select(c.WorldToScreenPoint);
			return array2[2] - array2[0];
		}

		private Vector2 GetSpriteSize(Sprite sprite, Camera c)
		{
			float num = sprite.rect.height / (c.orthographicSize * 2f) * _uiCamera.pixelRect.height;
			return new Vector2(num * sprite.rect.width / sprite.rect.height, num);
		}

		private Vector3 BuildingToUIPosition(Transform targetTransform, Sprite sprite)
		{
			return targetTransform.position + sprite.rect.height * 0.5f * Vector3.up;
		}

		private IEnumerator FollowTransformRoutine(Transform targetTransform, Camera c, Func<Transform, Vector3> positionFunction, Func<Camera, Vector2> sizeFunction, bool clickableMask)
		{
			while (targetTransform != null)
			{
				Vector3 position = c.WorldToScreenPoint(positionFunction(targetTransform));
				Vector3 position2 = _uiCamera.ScreenToWorldPoint(position);
				position2.z = 0f;
				base.transform.position = position2;
				Vector2 size = sizeFunction(c) / _canvasScaler.CanvasScaleFactor;
				_maskView.Show(size, clickableMask);
				yield return null;
			}
		}
	}
}
