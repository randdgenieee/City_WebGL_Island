using CI.WSANative.Facebook.Models;

namespace CI.WSANative.Facebook
{
	public class WSAFacebookUser
	{
		public string Id
		{
			get;
			set;
		}

		public WSAFacebookAgeRange AgeRange
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public string Link
		{
			get;
			set;
		}

		public string Gender
		{
			get;
			set;
		}

		public string Locale
		{
			get;
			set;
		}

		public WSAFacebookPicture Picture
		{
			get;
			set;
		}

		public int TimeZone
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public string Birthday
		{
			get;
			set;
		}

		public static WSAFacebookUser FromDto(WSAFacebookUserDto dto)
		{
			return new WSAFacebookUser
			{
				AgeRange = WSAFacebookAgeRange.FromDto(dto.age_range),
				Birthday = dto.birthday,
				Email = dto.email,
				FirstName = dto.first_name,
				Gender = dto.gender,
				Id = dto.id,
				LastName = dto.last_name,
				Link = dto.link,
				Locale = dto.locale,
				Name = dto.name,
				Picture = WSAFacebookPicture.FromDto(dto.picture),
				TimeZone = dto.timezone
			};
		}
	}
}
