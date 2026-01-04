using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CIG
{
	public class GameSparksIslandVisiting
	{
		private const string IslandsDataKey = "IslandsData";

		private const string DowloadUrlKey = "DownloadUrl";

		private const string DisplayNameKey = "DisplayName";

		private const string LikesKey = "Likes";

		private static readonly GameSparksJsonSchema NewIslandsDataSchema = new GameSparksJsonSchema().Field<string>("DownloadUrl").Field<string>("DisplayName").Field<int>("Likes");

		private readonly CIGGameSparksInstance _gameSparksInstance;

		private readonly RoutineRunner _routineRunner;

		private GameSparksDownloadProcess _downloadProcess;

		private GameSparksUploadProcess _uploadProcess;

		public GameSparksIslandVisiting(CIGGameSparksInstance gameSparksInstance, RoutineRunner routineRunner)
		{
			_gameSparksInstance = gameSparksInstance;
			_routineRunner = routineRunner;
		}

		public void Release()
		{
			if (_downloadProcess != null)
			{
				_downloadProcess.Release();
				_downloadProcess = null;
			}
			if (_uploadProcess != null)
			{
				_uploadProcess.Release();
				_uploadProcess = null;
			}
		}

		public void PushIslandData(Dictionary<string, object> islandsData, Action onSuccess, Action<GameSparksException> onError)
		{
			Dictionary<string, object> metaData = new Dictionary<string, object>
			{
				{
					"Type",
					"IslandsData"
				}
			};
			string uploadData = StorageController.StorageToString(islandsData);
			_uploadProcess = new GameSparksUploadProcess(_gameSparksInstance, _routineRunner, metaData, uploadData, "islandsData", delegate
			{
				_uploadProcess = null;
				EventTools.Fire(onSuccess);
			}, delegate
			{
				_uploadProcess = null;
				EventTools.Fire(onError, new GameSparksException("PushIslandData"));
			});
		}

		public void PullIslandData(LikeRegistrar likeRegistrar, string userId, Action<IslandsVisitingData> onSuccess, Action<GameSparksException> onError)
		{
			new RequestIslandVisitingPullMetadata(_gameSparksInstance, userId).Send(delegate(LogEventResponse successResponse)
			{
				GSData gSData = successResponse.ScriptData.GetGSData("IslandsData");
				if (gSData == null)
				{
					EventTools.Fire(onError, new GameSparksException("IslandVisitingPullMetadata", "", GSError.SchemaValidationError));
				}
				else if (gSData.ContainsKey("DownloadUrl"))
				{
					ParseNewPullResponse(likeRegistrar, userId, gSData, onSuccess, onError);
				}
				else
				{
					ParseOldPullResponse(likeRegistrar, userId, gSData, onSuccess, onError);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("IslandVisitingPullMetadata", errorResponse));
			});
		}

		private void ParseNewPullResponse(LikeRegistrar likeRegistrar, string userId, GSData islandsData, Action<IslandsVisitingData> onSuccess, Action<GameSparksException> onError)
		{
			string text = NewIslandsDataSchema.Validate(islandsData);
			if (!string.IsNullOrEmpty(text))
			{
				EventTools.Fire(onError, new GameSparksException("PullIslandData", text, GSError.SchemaValidationError));
				return;
			}
			string @string = islandsData.GetString("DownloadUrl");
			_downloadProcess = new GameSparksDownloadProcess(_routineRunner, @string, delegate(byte[] data)
			{
				StorageDictionary storageDictionary = StorageController.StringToStorage(Encoding.UTF8.GetString(data));
				storageDictionary.Set("Likes", islandsData.GetInt("Likes") ?? 0);
				storageDictionary.Set("DisplayName", islandsData.GetString("DisplayName"));
				try
				{
					EventTools.Fire(onSuccess, IslandsVisitingData.CreateFromResponseData(likeRegistrar, userId, new GSData(storageDictionary.InternalDictionary)));
				}
				catch (GameSparksException value)
				{
					EventTools.Fire(onError, value);
				}
			}, delegate
			{
				EventTools.Fire(onError, new GameSparksException("IslandVisitingPullMetadata"));
			});
		}

		private void ParseOldPullResponse(LikeRegistrar likeRegistrar, string userId, GSData islandsData, Action<IslandsVisitingData> onSuccess, Action<GameSparksException> onError)
		{
			try
			{
				EventTools.Fire(onSuccess, IslandsVisitingData.CreateFromResponseData(likeRegistrar, userId, islandsData));
			}
			catch (GameSparksException value)
			{
				EventTools.Fire(onError, value);
			}
		}
	}
}
