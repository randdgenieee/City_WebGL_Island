using GameSparks.Api.Responses;
using GameSparks.Core;
using SUISS.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CIG
{
	public class CloudStorage
	{
		public delegate void ConflictResolutionCompleteEventHandler(ConflictResolver.ConflictSolution result);

		private const string CloudStorageKey = "CloudStorage";

		private const string SaveDataKey = "SaveData";

		private const string SaveGuidKey = "SaveGuid";

		private const string UTCSaveTimeTicksKey = "SaveTimeTicks";

		private const string SaveStateGuidKey = "SaveStateGuid";

		private const string SignedUserIdKey = "UserId";

		private const string PlayerLevelKey = "PlayerLevel";

		private const string GameVersionKey = "GameVersion";

		private const string DeviceLastPushGuidKey = "DeviceLastPushGuid";

		private const string DisplayNameKey = "DisplayName";

		private const string DownloadUrlKey = "DownloadUrl";

		private const string LastSaveGuidSendToFirebaseKey = "LastSaveGuidSendToFirebase";

		private const double PullAfterSecondsInBackground = 60.0;

		private const long TimeoutMillis = 10000L;

		private static readonly GameSparksJsonSchema PullSchema = new GameSparksJsonSchema().Field<GSData>("CloudStorage");

		private readonly CIGGameSparksInstance _gameSparksInstance;

		private readonly CIGGameSparksPlatform _gameSparksPlatform;

		private readonly GameSparksAuthenticator _authenticator;

		private readonly RoutineRunner _routineRunner;

		private string _saveGuid;

		private string _saveStateGuid;

		private string _signedUserId;

		private readonly List<object> _pushBlockingObjects = new List<object>();

		private bool _isAllowedToPush;

		private bool _pullOnAuthenticationChange;

		private GameSparksDownloadProcess _downloadProcess;

		private GameSparksUploadProcess _uploadProcess;

		private string _lastSaveGuidSendToFirebase;

		public CloudGameState LastCloudGameState
		{
			get;
			private set;
		}

		public ConflictResolver.ConflictSolution LastResolutionResult
		{
			get;
			private set;
		}

		public DateTime UTCSaveTime
		{
			get;
			private set;
		}

		public bool IsAllowedToPush
		{
			get
			{
				if (_isAllowedToPush && _pushBlockingObjects.Count == 0 && StorageController.StorageWarning == StorageWarning.None && _authenticator.CurrentAuthentication.IsAuthenticated)
				{
					return _authenticator.CurrentAuthentication.UserId == _signedUserId;
				}
				return false;
			}
		}

		private LocalGameState LocalGameState => new LocalGameState(new GameSparksGameVersion("1.11.8"), _signedUserId, _saveGuid, _saveStateGuid);

		private string DeviceId => _gameSparksPlatform.DeviceId;

		private bool IsAllowedToPull => _authenticator.CurrentAuthentication.IsAuthenticated;

		public event ConflictResolutionCompleteEventHandler ConflictResolutionCompleteEvent;

		private void FireConflictResolutionCompleteEvent(ConflictResolver.ConflictSolution result)
		{
			this.ConflictResolutionCompleteEvent?.Invoke(result);
		}

		public CloudStorage(CIGGameSparksInstance gameSparksInstance, CIGGameSparksPlatform gameSparksPlatform, GameSparksAuthenticator authenticator, GameSparksUser gameSparksUser, RoutineRunner routineRunner)
		{
			_gameSparksInstance = gameSparksInstance;
			_gameSparksPlatform = gameSparksPlatform;
			_authenticator = authenticator;
			_routineRunner = routineRunner;
			gameSparksUser.PlayerUnlinkedEvent += OnPlayerUnlinked;
			_authenticator.AuthenticationChangedEvent += OnAuthenticationChanged;
			LoadFromStorage();
		}

		public void Release()
		{
			_authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
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

		public IEnumerator SyncWithServer()
		{
			bool hasReceivedResponse = false;
			if (IsAllowedToPull)
			{
				Pull(delegate(LogEventResponse successResponse)
				{
					OnPullCompleted(successResponse);
					hasReceivedResponse = true;
				}, delegate(GameSparksException err)
				{
					GameSparksUtils.LogGameSparksError(err);
					hasReceivedResponse = true;
				});
				DateTime timeoutDate = AntiCheatDateTime.UtcNow.AddMilliseconds(10000.0);
				while (!hasReceivedResponse && timeoutDate > AntiCheatDateTime.UtcNow)
				{
					yield return null;
				}
			}
		}

		public void TryPush(long playerLevel)
		{
			if (IsAllowedToPush)
			{
				Dictionary<string, object> metaData = new Dictionary<string, object>
				{
					{
						"CloudStorage",
						new Dictionary<string, object>
						{
							{
								"DeviceId",
								DeviceId
							},
							{
								"Metadata",
								new Dictionary<string, object>
								{
									{
										"SaveGuid",
										_saveGuid
									},
									{
										"SaveStateGuid",
										_saveStateGuid
									},
									{
										"SaveTimeTicks",
										UTCSaveTime.Ticks
									},
									{
										"PlayerLevel",
										playerLevel
									},
									{
										"GameVersion",
										new GameSparksGameVersion("1.11.8").ToGSData()
									}
								}
							}
						}
					}
				};
				string uploadData = StorageController.StorageToString(StorageController.CloudStorageRoot);
				_uploadProcess = new GameSparksUploadProcess(_gameSparksInstance, _routineRunner, metaData, uploadData, "saveData", delegate
				{
					_uploadProcess = null;
				}, delegate
				{
					_uploadProcess = null;
					GameSparksUtils.LogGameSparksError(new GameSparksException("PushCloudStorage"));
				});
			}
		}

		public void Pull()
		{
			Pull(OnPullCompleted, delegate(GameSparksException err)
			{
				GameSparksUtils.LogGameSparksError(err);
			});
		}

		public void ForcePull(Action successCallback, Action<GameSparksException> errorCallback)
		{
			Pull(delegate(LogEventResponse successResponse)
			{
				LastCloudGameState = ConvertPullReponse(successResponse);
				FireConflictResolutionCompleteEvent(ConflictResolver.ConflictSolution.PickCloud);
				EventTools.Fire(successCallback);
			}, delegate(GameSparksException err)
			{
				EventTools.Fire(errorCallback, err);
			});
		}

		public void LoadLastCloudSaveGame(Action onSuccess, Action onError)
		{
			if (LastCloudGameState != null)
			{
				if (string.IsNullOrEmpty(LastCloudGameState.DownloadUrl))
				{
					LoadGameFromString(LastCloudGameState.SaveGame);
					EventTools.Fire(onSuccess);
				}
				else
				{
					_downloadProcess?.Cancel();
					_downloadProcess = new GameSparksDownloadProcess(_routineRunner, LastCloudGameState.DownloadUrl, delegate(byte[] data)
					{
						LoadGameFromString(Encoding.UTF8.GetString(data));
						_downloadProcess = null;
						EventTools.Fire(onSuccess);
					}, delegate
					{
						_downloadProcess = null;
						onError?.Invoke();
					});
				}
			}
			else
			{
				EventTools.Fire(onError);
			}
		}

		public void ConflictResolved()
		{
			LastResolutionResult = ConflictResolver.ConflictSolution.None;
			_isAllowedToPush = true;
		}

		public void PushPushBlockingObject(object obj)
		{
			_pushBlockingObjects.Add(obj);
		}

		public void PopPushBlockingObject(object obj)
		{
			_pushBlockingObjects.Remove(obj);
		}

		public void EnablePullOnAuthenticationChange()
		{
			_pullOnAuthenticationChange = true;
		}

		public void DisablePullOnAuthenticationChange()
		{
			_pullOnAuthenticationChange = false;
		}

		public void ReturnedFromBackground(double secondsInBackground)
		{
			if (secondsInBackground > 60.0)
			{
				_isAllowedToPush = false;
				Pull();
			}
		}

		private void Pull(Action<LogEventResponse> onSuccess, Action<GameSparksException> onError)
		{
			new RequestCloudStoragePullMetadata(_gameSparksInstance, DeviceId).Send(onSuccess, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("Cloud Pull", errorResponse));
			});
		}

		private void OnPlayerUnlinked()
		{
			_isAllowedToPush = false;
		}

		private void OnAuthenticationChanged(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
		{
			if (IsAllowedToPull && _pullOnAuthenticationChange)
			{
				_isAllowedToPush = false;
				Pull();
			}
		}

		private void OnPullCompleted(LogEventResponse response)
		{
			LastCloudGameState = ConvertPullReponse(response);
			if (LastCloudGameState != null)
			{
				LastResolutionResult = ConflictResolver.Resolve(LocalGameState, LastCloudGameState);
				_isAllowedToPush = (LastResolutionResult == ConflictResolver.ConflictSolution.None);
				FireConflictResolutionCompleteEvent(LastResolutionResult);
			}
		}

		private CloudGameState ConvertPullReponse(LogEventResponse response)
		{
			string text = PullSchema.Validate(response.ScriptData);
			if (!string.IsNullOrEmpty(text))
			{
				UnityEngine.Debug.LogErrorFormat("Validation error in `ConvertPullReponse`: {0}", text);
				return null;
			}
			GSData gSData = response.ScriptData.GetGSData("CloudStorage");
			string @string = gSData.GetString("SaveData");
			string string2 = gSData.GetString("UserId");
			string string3 = gSData.GetString("SaveGuid");
			string string4 = gSData.GetString("SaveStateGuid");
			string string5 = gSData.GetString("DeviceLastPushGuid");
			int playerLevel = gSData.GetInt("PlayerLevel") ?? 1;
			string string6 = gSData.GetString("DisplayName");
			string string7 = gSData.GetString("DownloadUrl");
			GameSparksGameVersion gameVersion = new GameSparksGameVersion(gSData.GetGSData("GameVersion"));
			long? @long = gSData.GetLong("SaveTimeTicks");
			DateTime? utcSaveTime = @long.HasValue ? new DateTime?(new DateTime(@long.Value)) : null;
			return new CloudGameState(@string, string2, string3, string5, string4, utcSaveTime, playerLevel, string6, gameVersion, string7);
		}

		private void SendSaveGuidToFirebase(string lastSaveGuid, string newSaveGuid)
		{
			if (lastSaveGuid != newSaveGuid)
			{
				_lastSaveGuidSendToFirebase = (Analytics.TrySetSaveGuid(newSaveGuid) ? newSaveGuid : lastSaveGuid);
			}
		}

		public void Serialize()
		{
			_saveStateGuid = Guid.NewGuid().ToString();
			UTCSaveTime = AntiCheatDateTime.UtcNow;
			StorageController.CloudStorageRoot.Set("SaveGuid", _saveGuid);
			StorageController.CloudStorageRoot.Set("SaveStateGuid", _saveStateGuid);
			StorageController.CloudStorageRoot.Set("SaveTimeTicks", UTCSaveTime);
			StorageController.CloudStorageRoot.Set("LastSaveGuidSendToFirebase", _lastSaveGuidSendToFirebase);
			SignSaveDataWithUserId();
		}

		private void SignSaveDataWithUserId()
		{
			if (LastResolutionResult == ConflictResolver.ConflictSolution.None)
			{
				string userId = _authenticator.CurrentAuthentication.UserId;
				if (string.IsNullOrEmpty(userId))
				{
					StorageController.CloudStorageRoot.Remove("UserId");
					return;
				}
				_signedUserId = userId;
				StorageController.CloudStorageRoot.Set("UserId", userId);
			}
		}

		private void LoadFromStorage()
		{
			_lastSaveGuidSendToFirebase = StorageController.CloudStorageRoot.Get("LastSaveGuidSendToFirebase", string.Empty);
			_saveGuid = StorageController.CloudStorageRoot.Get("SaveGuid", Guid.NewGuid().ToString());
			SendSaveGuidToFirebase(_lastSaveGuidSendToFirebase, _saveGuid);
			_saveStateGuid = StorageController.CloudStorageRoot.Get<string>("SaveStateGuid", null);
			UTCSaveTime = StorageController.CloudStorageRoot.GetDateTime("SaveTimeTicks", AntiCheatDateTime.UtcNow);
			_signedUserId = StorageController.CloudStorageRoot.Get<string>("UserId", null);
		}

		private void LoadGameFromString(string input)
		{
			string saveGuid = _saveGuid;
			StorageController.ReplaceGameRootFromString(input);
			SendSaveGuidToFirebase(saveGuid, _saveGuid);
			LoadFromStorage();
		}
	}
}
