using CI.WSANative.Facebook.Models;

namespace CI.WSANative.Facebook
{
	public class WSAFacebookPicture
	{
		public WSAFacebookPictureData Data
		{
			get;
			set;
		}

		public static WSAFacebookPicture FromDto(WSAFacebookPictureDto dto)
		{
			if (dto == null)
			{
				return null;
			}
			return new WSAFacebookPicture
			{
				Data = WSAFacebookPictureData.FromDto(dto.data)
			};
		}
	}
}
