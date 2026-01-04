namespace CIG
{
	public enum GSError
	{
		None = 0,
		Other = 1,
		InvalidUserDetails = 2,
		NoConnection = 4,
		SchemaValidationError = 8,
		NotAuthenticated = 0x10,
		ExternalAuthenticationFailed = 0x20,
		AlreadyLinkedToSameUser = 0x40,
		InvalidCode = 0x80,
		FriendCodeGenerationFailed = 264,
		UserOrEnvironmentError = 230
	}
}
