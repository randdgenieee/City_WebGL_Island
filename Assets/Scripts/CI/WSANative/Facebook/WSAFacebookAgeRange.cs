using CI.WSANative.Facebook.Models;

namespace CI.WSANative.Facebook
{
	public class WSAFacebookAgeRange
	{
		public int Min
		{
			get;
			set;
		}

		public int Max
		{
			get;
			set;
		}

		public static WSAFacebookAgeRange FromDto(WSAFacebookAgeRangeDto dto)
		{
			if (dto == null)
			{
				return null;
			}
			return new WSAFacebookAgeRange
			{
				Max = dto.Max,
				Min = dto.min
			};
		}
	}
}
