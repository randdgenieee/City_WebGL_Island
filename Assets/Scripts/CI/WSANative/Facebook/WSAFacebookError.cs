using CI.WSANative.Facebook.Models;

namespace CI.WSANative.Facebook
{
	public class WSAFacebookError
	{
		public string Message
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		public bool AccessTokenExpired
		{
			get;
			set;
		}

		public static WSAFacebookError FromDto(WSAFacebookErrorDto dto)
		{
			return new WSAFacebookError
			{
				Message = dto.message,
				Type = dto.type,
				Code = dto.code
			};
		}
	}
}
