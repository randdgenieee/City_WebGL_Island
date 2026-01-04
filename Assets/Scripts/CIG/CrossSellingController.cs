using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class CrossSellingController : MonoBehaviour
	{
		[SerializeField]
		private GameObject _root;

		[SerializeField]
		private ScrollRect _scrollView;

		[SerializeField]
		private LocalizedText _subtitleText;

		[SerializeField]
		private RectTransform _otherGameParent;

		[SerializeField]
		private MoreGamesIcon _otherGamePrefab;

		private static bool CanShowMultiple => false;

		private void Awake()
		{
			Hide();
		}

		public void Initialize(CrossPromo crossPromo)
		{
			crossPromo.GetMoreGamesList(includeAlreadyInstalledGames: true, OnGetMoreGamesListCallback);
			_subtitleText.LocalizedString = Localization.Key(CanShowMultiple ? "MORE_GAMES_BUTTON_LABEL" : "ad");
		}

		private void Show(SparkSocGame[] items)
		{
			if (CanShowMultiple)
			{
				ShowMultiple(items);
			}
			else
			{
				ShowSingle(GetSingleItem(items));
			}
		}

		private void ShowSingle(SparkSocGame item)
		{
			if (item != null)
			{
				CreateMoreGameItem(item);
				_root.SetActive(value: true);
			}
			else
			{
				Hide();
			}
		}

		private void ShowMultiple(SparkSocGame[] items)
		{
			if (items.Length != 0)
			{
				_root.SetActive(value: true);
				foreach (SparkSocGame item in items)
				{
					CreateMoreGameItem(item);
				}
				_scrollView.verticalNormalizedPosition = 1f;
			}
			else
			{
				Hide();
			}
		}

		private void Hide()
		{
			_root.SetActive(value: false);
		}

		private void CreateMoreGameItem(SparkSocGame item)
		{
			MoreGamesIcon moreGamesIcon = UnityEngine.Object.Instantiate(_otherGamePrefab, _otherGameParent);
			moreGamesIcon.name = item.DisplayName;
			moreGamesIcon.Initialize(item);
		}

		private SparkSocGame GetSingleItem(SparkSocGame[] items)
		{
			int i = 0;
			for (int num = items.Length; i < num; i++)
			{
				SparkSocGame sparkSocGame = items[i];
				if (!sparkSocGame.IsInstalled)
				{
					return sparkSocGame;
				}
			}
			if (items.Length != 0)
			{
				return items[0];
			}
			return null;
		}

		private void OnGetMoreGamesListCallback(SparkSocGame[] items)
		{
			if (this != null)
			{
				Show(items);
			}
		}
	}
}
