using GameSparks.Api.Responses;
using GameSparks.Core;
using System;

namespace CIG
{
	public class GameSparksUser
	{
		public delegate void PlayerUnlinkedEventHandler();

		public struct LinkCode
		{
			public static readonly GameSparksJsonSchema Schema = new GameSparksJsonSchema().Field<string>("Code").Field<long>("ExpirationDateMillis");

			private const string CodeKey = "Code";

			private const string ExpirationDateKey = "ExpirationDateMillis";

			public string Code
			{
				get;
				private set;
			}

			public DateTime ExpirationDate
			{
				get;
				private set;
			}

			public static LinkCode CreateFromResponseData(GSData scriptData)
			{
				string text = Schema.Validate(scriptData);
				if (!string.IsNullOrEmpty(text))
				{
					throw new GameSparksException("LinkCodeFromScriptData", text, GSError.SchemaValidationError);
				}
				string @string = scriptData.GetString("Code");
				DateTime dateTime = GameSparksUtils.GetDateTime("ExpirationDateMillis", scriptData);
				LinkCode result = default(LinkCode);
				result.Code = @string;
				result.ExpirationDate = dateTime;
				return result;
			}
		}

		public struct UserMetaData
		{
			public static readonly GameSparksJsonSchema Schema = new GameSparksJsonSchema().Field<string>("DisplayName").Field<int>("PlayerLevel");

			private const string DisplayNameKey = "DisplayName";

			private const string PlayerLevelKey = "PlayerLevel";

			public string DisplayName
			{
				get;
				private set;
			}

			public int PlayerLevel
			{
				get;
				private set;
			}

			public static UserMetaData CreateFromResponseData(GSData scriptData)
			{
				string text = Schema.Validate(scriptData);
				if (!string.IsNullOrEmpty(text))
				{
					throw new GameSparksException("MetadataFromScriptData", text, GSError.SchemaValidationError);
				}
				string @string = scriptData.GetString("DisplayName");
				int playerLevel = scriptData.GetInt("PlayerLevel") ?? 0;
				UserMetaData result = default(UserMetaData);
				result.DisplayName = @string;
				result.PlayerLevel = playerLevel;
				return result;
			}
		}

		private const string LinkCodeKey = "LinkCode";

		private const string MetaDataKey = "MetaData";

		private const string UserDataKey = "UserData";

		private static readonly GameSparksJsonSchema RequestLinkCodeSchema = new GameSparksJsonSchema().Field("LinkCode", LinkCode.Schema);

		private static readonly GameSparksJsonSchema GetMetaDataSchema = new GameSparksJsonSchema().Field("MetaData", UserMetaData.Schema);

		private readonly GameSparksAuthenticator _authenticator;

		public CIGGameSparksInstance Instance
		{
			get;
			private set;
		}

		public event PlayerUnlinkedEventHandler PlayerUnlinkedEvent;

		private void FirePlayerUnlinkedEvent()
		{
			if (this.PlayerUnlinkedEvent != null)
			{
				this.PlayerUnlinkedEvent();
			}
		}

		public GameSparksUser(CIGGameSparksInstance instance, GameSparksAuthenticator authenticator)
		{
			Instance = instance;
			_authenticator = authenticator;
		}

		public void RequestLinkCode(Action<LinkCode> onSuccess, Action<GameSparksException> onError)
		{
			new RequestAcountLinkCode(Instance).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					string text = RequestLinkCodeSchema.Validate(successResponse.ScriptData);
					if (!string.IsNullOrEmpty(text))
					{
						throw new GameSparksException("RequestLinkCode", text, GSError.SchemaValidationError);
					}
					LinkCode value = LinkCode.CreateFromResponseData(successResponse.ScriptData.GetGSData("LinkCode"));
					EventTools.Fire(onSuccess, value);
				}
				catch (GameSparksException value2)
				{
					EventTools.Fire(onError, value2);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("RequestLinkCode", errorResponse));
			});
		}

		public void LinkPlayer(string linkCode, Action onSuccess, Action<GameSparksException> onError)
		{
			new RequestLinkAccounts(Instance, linkCode).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					_authenticator.CurrentAuthentication.UpdateUserData(successResponse.ScriptData.GetGSData("UserData"));
				}
				catch (GameSparksException value)
				{
					EventTools.Fire(onError, value);
					return;
				}
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("LinkPlayer", errorResponse));
			});
		}

		public void UnlinkPlayer(Action onSuccess, Action<GameSparksException> onError)
		{
			new RequestUnlinkAccount(Instance).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					_authenticator.CurrentAuthentication.UpdateUserData(successResponse.ScriptData.GetGSData("UserData"));
				}
				catch (GameSparksException value)
				{
					EventTools.Fire(onError, value);
					return;
				}
				FirePlayerUnlinkedEvent();
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("UnlinkPlayer", errorResponse));
			});
		}

		public void GetMetaData(string linkCode, Action<string, UserMetaData> onSuccess, Action<GameSparksException> onError)
		{
			new RequestUserGetMetaData(Instance, linkCode).Send(delegate(LogEventResponse succesResponse)
			{
				try
				{
					string text = GetMetaDataSchema.Validate(succesResponse.ScriptData);
					if (!string.IsNullOrEmpty(text))
					{
						throw new GameSparksException("GetMetaData", text, GSError.SchemaValidationError);
					}
					UserMetaData value = UserMetaData.CreateFromResponseData(succesResponse.ScriptData.GetGSData("MetaData"));
					EventTools.Fire(onSuccess, linkCode, value);
				}
				catch (GameSparksException value2)
				{
					EventTools.Fire(onError, value2);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("GetMetaData", errorResponse));
			});
		}
	}
}
