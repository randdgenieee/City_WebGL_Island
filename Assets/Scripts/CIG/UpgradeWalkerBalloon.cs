using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class UpgradeWalkerBalloon : WalkerBalloon
	{
		private CIGBuilding _residentialBuilding;

		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable)
				{
					return false;
				}
				if (_residentialBuilding == null)
				{
					List<CIGResidentialBuilding> list = Object.FindObjectsOfType<CIGResidentialBuilding>().ToList();
					list.Shuffle();
					_residentialBuilding = null;
					int i = 0;
					for (int count = list.Count; i < count; i++)
					{
						CIGResidentialBuilding cIGResidentialBuilding = list[i];
						if (cIGResidentialBuilding.State == BuildingState.Normal && cIGResidentialBuilding.CurrentLevel == 0 && !cIGResidentialBuilding.IsUpgrading)
						{
							_residentialBuilding = cIGResidentialBuilding;
							_residentialBuilding.DestroyedEvent += OnBuildingDestroyed;
							break;
						}
					}
				}
				return _residentialBuilding != null;
			}
		}

		public UpgradeWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			if (_residentialBuilding != null)
			{
				ShowPopup("upgrade_walker_balloon", "feedback_upgrade_home", _residentialBuilding.SpriteRenderer.sprite);
			}
			Analytics.WalkerExclamationBalloonClicked(_properties.BalloonType.ToString());
		}

		protected override void OnExpired()
		{
			_residentialBuilding.DestroyedEvent -= OnBuildingDestroyed;
			base.OnExpired();
		}

		protected override void PopupAction()
		{
			if (_residentialBuilding != null)
			{
				IsometricIsland.Current.CameraOperator.ScrollTo(_residentialBuilding.gameObject);
				_popupManager.RequestPopup(new BuildingPopupRequest(_residentialBuilding, BuildingPopupContent.Upgrade));
			}
		}

		private void OnBuildingDestroyed(double time)
		{
			Expire();
		}
	}
}
