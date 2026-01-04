using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class SSPOtherGamesContentView : SSPMenuContentView
	{
		[SerializeField]
		private OtherGameItem _otherGameItemPrefab;

		[SerializeField]
		private Transform _scrollRectTransform;

		[SerializeField]
		private ScrollRect _scrollRect;

		public override ILocalizedString HeaderText => Localization.Key("our_other_games");

		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.OtherGames;

		private void OnDestroy()
		{
			if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
			{
				SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			}
		}

		private void OnEnable()
		{
			ResetScroll();
		}

		public override void Initialize(SSPMenuPopup popup, Model model)
		{
			base.Initialize(popup, model);
			model.GameServer.CrossPromo.GetOtherGamesList(includeAlreadyInstalledGames: false, PopulateGrid);
		}

		protected override void Open()
		{
			base.Open();
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}

		protected override void Close()
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			base.Close();
		}

		private void ResetScroll()
		{
			_scrollRect.verticalScrollbar.value = 1f;
		}

		private void PopulateGrid(SparkSocGame[] games)
		{
			int i = 0;
			for (int num = games.Length; i < num; i++)
			{
				SparkSocGame sparkSocGame = games[i];
				if (!sparkSocGame.IsInstalled && !(sparkSocGame.BannerImage == null))
				{
					OtherGameItem otherGameItem = Object.Instantiate(_otherGameItemPrefab, _scrollRectTransform);
					otherGameItem.name = i.ToString("D2");
					otherGameItem.Initialize(sparkSocGame);
				}
			}
			ResetScroll();
		}
	}
}
