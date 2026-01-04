using System.Collections.Generic;
using Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class TreasureChestView : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public delegate void TreasureChestTappedEventHandler(bool afterCooldown, bool finalTap);

		public delegate void TreasureChestOpenedEventHandler();

		public delegate void TreasureChestRewardFlyStartEventHandler(RewardItemData itemData);

		public delegate void TreasureChestClosedEventHandler(bool landmark);

		public delegate void TreasureChestLeftEventHandler();

		public delegate void TreasureChestLandmarkOpenedEventHandler(BuildingProperties buildingProperties);

		private static readonly int CloseHash = Animator.StringToHash("Close");

		private static readonly int TappedHash = Animator.StringToHash("Tapped");

		private static readonly int HasLandMarkHash = Animator.StringToHash("HasLandmark");

		private const float TapCooldown = 0.5f;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private SpawnedParticles _tapParticles;

		[SerializeField]
		private Collider _collider;

		[SerializeField]
		private TreasureChestFlyingReward _rewardFlyPrefab;

		[SerializeField]
		private Tweener _chestLeaveTweener;

		[SerializeField]
		private AnimationSfxTrigger _animationSfxTrigger;

		private Tweener _cameraTweener;

		private bool _colliderShouldBeActive = true;

		private bool _inputEnabled = true;

		private bool _chestOpened;

		private List<RewardItemData> _rewards;

		private int _rewardIndex;

		private float _tapTimeStamp;

		public event TreasureChestTappedEventHandler TreasureChestTappedEvent;

		public event TreasureChestOpenedEventHandler TreasureChestOpenedEvent;

		public event TreasureChestRewardFlyStartEventHandler TreasureChestRewardFlyStartEvent;

		public event TreasureChestClosedEventHandler TreasureChestClosedEvent;

		public event TreasureChestLeftEventHandler TreasureChestLeftEvent;

		public event TreasureChestLandmarkOpenedEventHandler TreasureChestLandmarkOpenedEvent;

		private void FireTreasureChestTappedEvent(bool afterCooldown, bool finalTap)
		{
			this.TreasureChestTappedEvent?.Invoke(afterCooldown, finalTap);
		}

		private void FireTreasureChestOpenedEvent()
		{
			this.TreasureChestOpenedEvent?.Invoke();
		}

		private void FireTreasureChestRewardFlyStartEvent(RewardItemData itemData)
		{
			this.TreasureChestRewardFlyStartEvent?.Invoke(itemData);
		}

		private void FireTreasureChestClosedEvent(bool landmark)
		{
			this.TreasureChestClosedEvent?.Invoke(landmark);
		}

		private void FireTreasureChestLeftEvent()
		{
			this.TreasureChestLeftEvent?.Invoke();
		}

		private void FireTreasureChestLandmarkOpenedEvent(BuildingProperties buildingProperties)
		{
			this.TreasureChestLandmarkOpenedEvent?.Invoke(buildingProperties);
		}

		public void Initialize(TreasureChestType chestType, List<RewardItemData> rewards, Tweener cameraTweener)
		{
			_cameraTweener = cameraTweener;
			_cameraTweener.StopAndReset();
			SetInputActive(active: true);
			_animationSfxTrigger.SetActive(active: true);
			_rewards = rewards;
			_chestLeaveTweener.FinishedPlaying += OnChestLeaveAnimationFinished;
		}

		public void Deinitialize()
		{
			_rewardIndex = 0;
			_chestOpened = false;
			_chestLeaveTweener.FinishedPlaying -= OnChestLeaveAnimationFinished;
			_chestLeaveTweener.StopAndReset();
		}

		private void OnDestroy()
		{
			if (_chestLeaveTweener != null)
			{
				_chestLeaveTweener.FinishedPlaying -= OnChestLeaveAnimationFinished;
			}
		}

		public void Leave()
		{
			SetInputActive(active: false);
			_chestLeaveTweener.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Woosh);
		}

		public void SetActive(bool active)
		{
			_inputEnabled = active;
			_animationSfxTrigger.SetActive(active);
			SetInputActive(_colliderShouldBeActive);
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
			Object.Instantiate(_tapParticles, worldPosition, Quaternion.identity, base.transform).Play();
			float unscaledTime = Time.unscaledTime;
			bool flag = unscaledTime - _tapTimeStamp > 0.5f;
			bool flag2 = _rewardIndex >= _rewards.Count;
			FireTreasureChestTappedEvent(flag, flag2);
			if (!(!_chestOpened | flag))
			{
				return;
			}
			_tapTimeStamp = unscaledTime;
			if (!flag2)
			{
				if (_chestOpened)
				{
					RewardItem();
				}
				_animator.SetTrigger(TappedHash);
			}
		}

		private void SetInputActive(bool active)
		{
			_colliderShouldBeActive = active;
			_collider.enabled = (_colliderShouldBeActive && _inputEnabled);
		}

		private void RewardItem()
		{
			RewardItemData rewardItemData = _rewards[_rewardIndex];
			bool flag = rewardItemData.BuildingProperties is LandmarkBuildingProperties;
			_animator.SetBool(HasLandMarkHash, flag);
			if (!flag)
			{
				StartFlyingReward(rewardItemData);
			}
			else
			{
				SetInputActive(active: false);
			}
			if (_rewardIndex == _rewards.Count - 1)
			{
				_animator.SetTrigger(CloseHash);
			}
			_rewardIndex++;
		}

		private void StartFlyingReward(RewardItemData itemData)
		{
			if (itemData.Particles.HasValue)
			{
				Object.Instantiate(SingletonMonobehaviour<ParticlesAssetCollection>.Instance.GetAsset(itemData.Particles.Value), base.transform.position + Vector3.up * 2f, Quaternion.identity, base.transform).Play();
			}
			if (itemData.ParticleClip.HasValue)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(itemData.ParticleClip.Value);
			}
			Object.Instantiate(_rewardFlyPrefab, base.transform).Initialize(itemData);
			FireTreasureChestRewardFlyStartEvent(itemData);
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.TreasureChestOpenTap);
		}

		private void OnChestOpened()
		{
			_chestOpened = true;
			RewardItem();
			FireTreasureChestOpenedEvent();
		}

		private void OnChestLandmarkOpened()
		{
			_animationSfxTrigger.SetActive(active: false);
			_animator.SetBool(HasLandMarkHash, value: false);
			FireTreasureChestLandmarkOpenedEvent(_rewards[_rewardIndex - 1].BuildingProperties);
		}

		private void OnChestClosed(int landmark)
		{
			bool flag = landmark >= 1;
			if (flag)
			{
				_cameraTweener.StopAndReset();
				_cameraTweener.Play();
				SetInputActive(active: false);
			}
			FireTreasureChestClosedEvent(flag);
		}

		private void OnChestLeaveAnimationFinished(Tweener tweener)
		{
			FireTreasureChestLeftEvent();
		}
	}
}
