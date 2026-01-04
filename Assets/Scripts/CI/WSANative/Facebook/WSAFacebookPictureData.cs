using CI.WSANative.Facebook.Models;

namespace CI.WSANative.Facebook
{
	public class WSAFacebookPictureData
	{
		public string Url
		{
			get;
			set;
		}

		public bool IsSilhouette
		{
			get;
			set;
		}

		public int Width
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		public static WSAFacebookPictureData FromDto(WSAFacebookPictureDataDto dto)
		{
			if (dto == null)
			{
				return null;
			}
			return new WSAFacebookPictureData
			{
				IsSilhouette = dto.is_silhouette,
				Url = dto.url,
				Width = dto.width,
				Height = dto.height
			};
		}
	}
}
