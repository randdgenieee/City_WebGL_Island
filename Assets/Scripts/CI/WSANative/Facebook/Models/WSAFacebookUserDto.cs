using System;

namespace CI.WSANative.Facebook.Models
{
	[Serializable]
	public class WSAFacebookUserDto
	{
		public string id;

		public WSAFacebookAgeRangeDto age_range;

		public string name;

		public string first_name;

		public string last_name;

		public string link;

		public string gender;

		public string locale;

		public WSAFacebookPictureDto picture;

		public int timezone;

		public string email;

		public string birthday;
	}
}
