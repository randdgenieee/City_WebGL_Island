using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class FishingLocation : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private GridTileIconManager _gridTileIconManager;

		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[SerializeField]
		private ParticleSystem _catchOneParticles;

		[SerializeField]
		private ParticleSystem _catchTwoParticles;

		private FishingEvent _fishingEvent;

		private FishingMinigame _fishingMinigame;

		private PopupManager _popupManager;

		public Sprite Sprite => _spriteRenderer.sprite;

		public bool HasPlayed
		{
			get;
			private set;
		}

		public void Initialize(FishingEvent fishingEvent, PopupManager popupManager, OverlayManager overlayManager, Vector3 position)
		{
			_fishingEvent = fishingEvent;
			_popupManager = popupManager;
			base.transform.localPosition = position;
			SetActive(active: false);
			_gridTileIconManager.Initialize(overlayManager);
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			OnClicked();
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
			HasPlayed = false;
		}

		public void SetIcon(bool show)
		{
			if (show)
			{
				_gridTileIconManager.SetIcon<ButtonGridTileIcon>(GridTileIconType.FishingMinigame).Init(OnClicked);
			}
			else
			{
				_gridTileIconManager.RemoveIcon(GridTileIconType.FishingMinigame);
			}
		}

		public void ZoomTo(Action<FishingLocation> animationFinishedCallback)
		{
			IsometricIsland.Current.CameraOperator.ScrollAndZoom(base.gameObject, IsometricIsland.Current.CameraOperator.MaxZoom, null, Vector2.up * 100f);
			this.Invoke(delegate
			{
				EventTools.Fire(animationFinishedCallback, this);
			}, 0.6f);
		}

		private void OnClicked()
		{
			if (_fishingEvent.FishingQuest.IsActive && !_fishingEvent.FishingQuest.CanCollect)
			{
				ZoomTo(null);
				StartMinigame();
				_popupManager.RequestPopup(new FishingMinigamePopupRequest(_fishingMinigame));
			}
			else
			{
				_fishingEvent.OpenQuestPopup();
			}
		}

		private void StartMinigame()
		{
			if (_fishingMinigame == null)
			{
				_fishingMinigame = _fishingEvent.StartMinigame();
				_fishingMinigame.FishCaughtEvent += OnFishCaught;
				_fishingMinigame.FinishedEvent += OnMinigameFinished;
			}
		}

		private void OnMinigameFinished(int score)
		{
			HasPlayed = true;
			_fishingMinigame.FishCaughtEvent -= OnFishCaught;
			_fishingMinigame.FinishedEvent -= OnMinigameFinished;
			_fishingMinigame = null;
		}

		private void OnFishCaught(int amount)
		{
			if (amount == 1)
			{
				_catchOneParticles.Stop();
				_catchOneParticles.Play();
			}
			else if (amount > 1)
			{
				_catchTwoParticles.Stop();
				_catchTwoParticles.Play();
			}
		}
	}
}
