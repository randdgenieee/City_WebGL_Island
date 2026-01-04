using System;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class SparkSocGame
	{
		[SerializeField]
		private int factor;

		[SerializeField]
		private string packageName;

		[SerializeField]
		private string displayName;

		[SerializeField]
		private string appIconUrl;

		[SerializeField]
		private string largeImageUrl;

		[SerializeField]
		private string bannerUrl;

		[SerializeField]
		private string downloadUrl;

		[SerializeField]
		private string appId;

		[SerializeField]
		private string productId;

		private string _refererLink;

		public int Factor => factor;

		public string DisplayName => displayName;

		public string AppIconUrl => appIconUrl;

		public string LargeImageUrl => largeImageUrl;

		public string BannerUrl => bannerUrl;

		public string AppId => appId;

		public Texture2D AppIcon
		{
			get;
			private set;
		}

		public Texture2D PromoImage
		{
			get;
			private set;
		}

		public Texture2D BannerImage
		{
			get;
			private set;
		}

		public bool IsInstalled => AppInstalledChecker.IsAppInstalled(packageName);

		public void Initialize(string refererLink)
		{
			_refererLink = refererLink;
		}

		public void SetAppIcon(Texture2D icon)
		{
			AppIcon = icon;
		}

		public void SetPromoImage(Texture2D image)
		{
			PromoImage = image;
		}

		public void SetBannerImage(Texture2D image)
		{
			BannerImage = image;
		}

		public void OpenInAppStore()
		{
		}
	}
}
