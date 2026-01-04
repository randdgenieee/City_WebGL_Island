using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class CrossPromo
	{
		public enum Referer
		{
			Promo,
			MoreGames,
			OtherGames
		}

		private const int JsonCacheExperationDurationInDays = 1;

		private const int TextureCacheExperationDurationInDays = 7;

		private const string RefererMoreGames = "sparksoc_moregames";

		private const string RefererIngamePromo = "sparksoc_promo";

		private const string RefererOtherGames = "sparksoc_othergames";

		private readonly RoutineRunner _routineRunner;

		private readonly string _applicationId;

		private string MoreGamesURL
		{
			get
			{
				string empty = string.Empty;
				return "https://crosspromo.sparklingsociety.net/googleplay/more_games.php?app=" + _applicationId;
			}
		}

		private string PromoURL
		{
			get
			{
				string empty = string.Empty;
				return "https://crosspromo.sparklingsociety.net/googleplay/promo.php?app=" + _applicationId;
			}
		}

		private string OtherGamesURL
		{
			get
			{
				string empty = string.Empty;
				return "https://crosspromo.sparklingsociety.net/googleplay/other_games.php?app=" + _applicationId;
			}
		}

		public CrossPromo(RoutineRunner routineRunner)
		{
			_routineRunner = routineRunner;
			_applicationId = Application.identifier;
		}

		public void GetMoreGamesList(bool includeAlreadyInstalledGames, Action<SparkSocGame[]> callback, bool downloadIcons = true)
		{
			if (callback != null)
			{
				GetSparkSocGames(Referer.MoreGames, delegate(List<SparkSocGame> games)
				{
					if (downloadIcons)
					{
						LoadIcons(games, delegate
						{
							if (!includeAlreadyInstalledGames)
							{
								RemoveInstalled(games);
							}
							Sort(games);
							callback(games.ToArray());
						});
					}
					else
					{
						callback(games.ToArray());
					}
				});
			}
		}

		public void DownloadGameIcon(SparkSocGame item, Action<SparkSocGame> callback)
		{
			_routineRunner.StartCoroutine(DoDownloadIconRoutine(item, callback));
		}

		public void GetOtherGamesList(bool includeAlreadyInstalledGames, Action<SparkSocGame[]> callback, bool downloadBanners = true)
		{
			if (callback != null)
			{
				GetSparkSocGames(Referer.OtherGames, delegate(List<SparkSocGame> games)
				{
					if (downloadBanners)
					{
						LoadBanners(games, delegate
						{
							if (!includeAlreadyInstalledGames)
							{
								RemoveInstalled(games);
							}
							Sort(games);
							callback(games.ToArray());
						});
					}
					else
					{
						callback(games.ToArray());
					}
				});
			}
		}

		public void GetPromoItem(Action<SparkSocGame> callback)
		{
			GetSparkSocGames(Referer.Promo, delegate(List<SparkSocGame> games)
			{
				RemoveInstalled(games);
				List<SparkSocGame> list = new List<SparkSocGame>();
				int i = 0;
				for (int count = games.Count; i < count; i++)
				{
					SparkSocGame sparkSocGame = games[i];
					for (int j = 0; j <= sparkSocGame.Factor; j++)
					{
						list.Add(sparkSocGame);
					}
				}
				if (list.Count > 0)
				{
					SparkSocGame item = list[UnityEngine.Random.Range(0, list.Count)];
					_routineRunner.StartCoroutine(DoDownloadLargeImageRoutine(item, delegate(SparkSocGame game)
					{
						callback((game.PromoImage == null) ? null : game);
					}));
				}
				else
				{
					callback(null);
				}
			});
		}

		private void GetSparkSocGames(Referer referer, Action<List<SparkSocGame>> callback)
		{
			if (callback != null)
			{
				string url = RefererToUrl(referer);
				DownloadJSON(url, delegate(bool hasError, string result)
				{
					if (hasError)
					{
						callback(new List<SparkSocGame>());
					}
					else
					{
						List<SparkSocGame> list = ParseJSON(result);
						InitializeItems(referer, list);
						callback(list);
					}
				});
			}
		}

		private void DownloadJSON(string url, Action<bool, string> callback)
		{
			_routineRunner.StartCoroutine(DoDownloadJSONRoutine(url, callback));
		}

		private IEnumerator DoDownloadJSONRoutine(string url, Action<bool, string> callback)
		{
			using (CachedWebRequest download = new CachedWebRequest(url))
			{
				yield return download;
				if (download.Error == null)
				{
					callback(arg1: false, download.Text);
				}
				else
				{
					UnityEngine.Debug.LogError("Failed downloading more games list from " + url + Environment.NewLine + " Error: " + download.Error);
					callback(arg1: true, download.Error);
				}
			}
		}

		private List<SparkSocGame> ParseJSON(string json)
		{
			List<SparkSocGame> list = new List<SparkSocGame>();
			SparkSocGame[] array = null;
			try
			{
				array = JsonHelper.GetJsonArray<SparkSocGame>(json);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning("Failed parsing gameslist JSON: " + Environment.NewLine + json + Environment.NewLine + ex);
			}
			if (array != null)
			{
				list.AddRange(array);
			}
			return list;
		}

		private void InitializeItems(Referer referer, List<SparkSocGame> items)
		{
			for (int num = items.Count - 1; num >= 0; num--)
			{
				SparkSocGame sparkSocGame = items[num];
				string text = RefererToCampaign(referer);
				string refererLink = "&referrer=utm_source%3D" + _applicationId + "%26utm_medium%3D" + text + "%26utm_campaign%3D" + text;
				sparkSocGame.Initialize(refererLink);
				if (sparkSocGame.AppId == _applicationId)
				{
					items.Remove(sparkSocGame);
				}
			}
		}

		private void Sort(List<SparkSocGame> items)
		{
			items.Sort((SparkSocGame a, SparkSocGame b) => (a.IsInstalled != b.IsInstalled) ? a.IsInstalled.CompareTo(b.IsInstalled) : b.Factor.CompareTo(a.Factor));
		}

		private void RemoveInstalled(List<SparkSocGame> items)
		{
			for (int num = items.Count - 1; num >= 0; num--)
			{
				if (items[num].IsInstalled)
				{
					items.Remove(items[num]);
				}
			}
		}

		private void LoadIcons(List<SparkSocGame> items, Action callback)
		{
			_routineRunner.StartCoroutine(DoLoadIcons(items, callback));
		}

		private IEnumerator DoLoadIcons(List<SparkSocGame> items, Action callback)
		{
			List<SparkSocGame> moreGamesItems = new List<SparkSocGame>(items);
			int i = 0;
			for (int count = items.Count; i < count; i++)
			{
				SparkSocGame item = items[i];
				_routineRunner.StartCoroutine(DoDownloadIconRoutine(item, delegate(SparkSocGame processedItem)
				{
					moreGamesItems.Remove(processedItem);
				}));
			}
			while (moreGamesItems.Count != 0)
			{
				yield return null;
			}
			callback?.Invoke();
		}

		private void LoadBanners(List<SparkSocGame> items, Action callback)
		{
			_routineRunner.StartCoroutine(DoLoadBanners(items, callback));
		}

		private IEnumerator DoLoadBanners(List<SparkSocGame> items, Action callback)
		{
			List<SparkSocGame> otherGamesItems = new List<SparkSocGame>(items);
			int i = 0;
			for (int count = otherGamesItems.Count; i < count; i++)
			{
				SparkSocGame item = items[i];
				_routineRunner.StartCoroutine(DoDownloadBannerRoutine(item, delegate(SparkSocGame processedItem)
				{
					otherGamesItems.Remove(processedItem);
				}));
			}
			while (otherGamesItems.Count != 0)
			{
				yield return null;
			}
			callback?.Invoke();
		}

		private IEnumerator DoDownloadIconRoutine(SparkSocGame item, Action<SparkSocGame> callback)
		{
			using (CachedWebRequestTexture appIconWebRequest = new CachedWebRequestTexture(item.AppIconUrl))
			{
				yield return appIconWebRequest;
				if (appIconWebRequest.Error == null)
				{
					item.SetAppIcon(appIconWebRequest.Texture);
				}
				else
				{
					UnityEngine.Debug.LogError("Error downloading from " + appIconWebRequest.Url + " " + appIconWebRequest.Error);
				}
				callback?.Invoke(item);
			}
		}

		private IEnumerator DoDownloadLargeImageRoutine(SparkSocGame item, Action<SparkSocGame> callback)
		{
			using (CachedWebRequestTexture webRequest = new CachedWebRequestTexture(item.LargeImageUrl, 7))
			{
				yield return webRequest;
				if (webRequest.Error != null)
				{
					UnityEngine.Debug.LogError("Error downloading from " + webRequest.Url + " " + webRequest.Error);
				}
				if (callback != null)
				{
					item.SetPromoImage((webRequest.Error == null) ? webRequest.Texture : null);
					callback(item);
				}
			}
		}

		private IEnumerator DoDownloadBannerRoutine(SparkSocGame item, Action<SparkSocGame> callback)
		{
			using (CachedWebRequestTexture webRequest = new CachedWebRequestTexture(item.BannerUrl, 7))
			{
				yield return webRequest;
				if (webRequest.Error == null)
				{
					item.SetBannerImage(webRequest.Texture);
				}
				else
				{
					UnityEngine.Debug.LogErrorFormat("Error downloading from {0}, {1}", webRequest.Url, webRequest.Error);
				}
				callback?.Invoke(item);
			}
		}

		private string RefererToCampaign(Referer r)
		{
			string result = string.Empty;
			switch (r)
			{
			case Referer.Promo:
				result = "sparksoc_promo";
				break;
			case Referer.MoreGames:
				result = "sparksoc_moregames";
				break;
			case Referer.OtherGames:
				result = "sparksoc_othergames";
				break;
			}
			return result;
		}

		private string RefererToUrl(Referer r)
		{
			string empty = string.Empty;
			switch (r)
			{
			default:
				return MoreGamesURL;
			case Referer.Promo:
				return PromoURL;
			case Referer.OtherGames:
				return OtherGamesURL;
			}
		}
	}
}
