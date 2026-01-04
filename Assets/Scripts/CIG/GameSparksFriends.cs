using GameSparks.Api.Responses;
using GameSparks.Core;
using SparkLinq;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class GameSparksFriends
	{
		public delegate void FriendRequestReceivedEventHandler(FriendData friendData);

		public delegate void FriendRequestAcceptedEventHandler(FriendData friendData);

		public delegate void FriendRequestDeclinedEventHandler(string friendCode);

		public delegate void FriendGiftReceivedEventHandler(FriendData friendData);

		private const string FriendsListKey = "FriendsList";

		private const string FriendSuggestionsKey = "FriendSuggestions";

		private const string FriendDataKey = "FriendData";

		private const string CurrenciesKey = "Currencies";

		private static readonly GameSparksJsonSchema ReceiveGiftSchema = new GameSparksJsonSchema().Field<GSData>("Currencies");

		private const string CooldownKey = "Cooldown";

		private static readonly GameSparksJsonSchema GiveGiftSchema = new GameSparksJsonSchema().Field<double>("Cooldown");

		private readonly CIGGameSparksInstance _gameSparksInstance;

		public event FriendRequestReceivedEventHandler FriendRequestReceivedEvent;

		public event FriendRequestAcceptedEventHandler FriendRequestAcceptedEvent;

		public event FriendRequestDeclinedEventHandler FriendRequestDeclinedEvent;

		public event FriendGiftReceivedEventHandler FriendGiftReceivedEvent;

		private void FireFriendRequestReceivedEvent(FriendData friend)
		{
			this.FriendRequestReceivedEvent?.Invoke(friend);
		}

		private void FireFriendRequestAcceptedEvent(FriendData friend)
		{
			this.FriendRequestAcceptedEvent?.Invoke(friend);
		}

		private void FireFriendRequestDeclinedEvent(string friendCode)
		{
			this.FriendRequestDeclinedEvent?.Invoke(friendCode);
		}

		private void FireFriendGiftReceivedEvent(FriendData friendData)
		{
			this.FriendGiftReceivedEvent?.Invoke(friendData);
		}

		public GameSparksFriends(CIGGameSparksInstance gameSparksInstance)
		{
			_gameSparksInstance = gameSparksInstance;
			MessageFriendRequestReceived.Listener = (Action<MessageFriendRequestReceived>)Delegate.Combine(MessageFriendRequestReceived.Listener, new Action<MessageFriendRequestReceived>(OnFriendRequestReceivedMessageReceived));
			MessageFriendRequestAccepted.Listener = (Action<MessageFriendRequestAccepted>)Delegate.Combine(MessageFriendRequestAccepted.Listener, new Action<MessageFriendRequestAccepted>(OnFriendRequestAcceptedMessageReceived));
			MessageFriendRequestDeclined.Listener = (Action<MessageFriendRequestDeclined>)Delegate.Combine(MessageFriendRequestDeclined.Listener, new Action<MessageFriendRequestDeclined>(OnFriendRequestDeclinedMessageReceived));
			MessageFriendGiftReceived.Listener = (Action<MessageFriendGiftReceived>)Delegate.Combine(MessageFriendGiftReceived.Listener, new Action<MessageFriendGiftReceived>(OnFriendGiftReceived));
		}

		public void Release()
		{
			MessageFriendRequestReceived.Listener = (Action<MessageFriendRequestReceived>)Delegate.Remove(MessageFriendRequestReceived.Listener, new Action<MessageFriendRequestReceived>(OnFriendRequestReceivedMessageReceived));
			MessageFriendRequestAccepted.Listener = (Action<MessageFriendRequestAccepted>)Delegate.Remove(MessageFriendRequestAccepted.Listener, new Action<MessageFriendRequestAccepted>(OnFriendRequestAcceptedMessageReceived));
			MessageFriendRequestDeclined.Listener = (Action<MessageFriendRequestDeclined>)Delegate.Remove(MessageFriendRequestDeclined.Listener, new Action<MessageFriendRequestDeclined>(OnFriendRequestDeclinedMessageReceived));
			MessageFriendGiftReceived.Listener = (Action<MessageFriendGiftReceived>)Delegate.Remove(MessageFriendGiftReceived.Listener, new Action<MessageFriendGiftReceived>(OnFriendGiftReceived));
		}

		public void GetFriendsList(Action<FriendList> onSuccess, Action onError)
		{
			new RequestGetFriendsList(_gameSparksInstance).Send(delegate(LogEventResponse successResponse)
			{
				List<GSData> gSDataList = successResponse.ScriptData.GetGSDataList("FriendsList");
				try
				{
					List<FriendData> friendDatas = from data in gSDataList
						select new FriendData(data);
					EventTools.Fire(onSuccess, new FriendList(friendDatas));
				}
				catch (GameSparksException exception)
				{
					GameSparksUtils.LogGameSparksError(exception);
					EventTools.Fire(onError);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				GameSparksUtils.LogGameSparksError(new GameSparksException("GetFriendsList", errorResponse));
				EventTools.Fire(onError);
			});
		}

		public void GetFriendSuggestions(int amount, int level, Action<FriendList> onSuccess, Action onError)
		{
			new RequestGetFriendSuggestions(_gameSparksInstance, amount, level).Send(delegate(LogEventResponse successResponse)
			{
				List<GSData> gSDataList = successResponse.ScriptData.GetGSDataList("FriendSuggestions");
				try
				{
					List<FriendData> friendDatas = from data in gSDataList
						select new FriendData(data);
					EventTools.Fire(onSuccess, new FriendList(friendDatas));
				}
				catch (GameSparksException exception)
				{
					GameSparksUtils.LogGameSparksError(exception);
					EventTools.Fire(onError);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				GameSparksUtils.LogGameSparksError(new GameSparksException("GetFriendSuggestions", errorResponse));
				EventTools.Fire(onError);
			});
		}

		public void SendFriendRequest(string friendCode, Action<FriendData> onSuccess, Action onError)
		{
			new RequestSendFriendRequest(_gameSparksInstance, friendCode).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					FriendData value = new FriendData(successResponse.ScriptData.GetGSData("FriendData"));
					EventTools.Fire(onSuccess, value);
				}
				catch (GameSparksException exception)
				{
					GameSparksUtils.LogGameSparksError(exception);
					EventTools.Fire(onError);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				GameSparksUtils.LogGameSparksError(new GameSparksException("SendFriendRequest", errorResponse));
				EventTools.Fire(onError);
			});
		}

		public void DeclineFriendRequest(string friendCode, Action onSuccess, Action onError)
		{
			new RequestDeclineFriendRequest(_gameSparksInstance, friendCode).Send(delegate
			{
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				GameSparksUtils.LogGameSparksError(new GameSparksException("DeclineFriendRequest", errorResponse));
				EventTools.Fire(onError);
			});
		}

		public void SendGift(string userId, Currencies currencies, Action<double> onSuccess, Action onError)
		{
			new RequestFriendsGiveGift(_gameSparksInstance, userId, currencies).Send(delegate(LogEventResponse successResponse)
			{
				string text = GiveGiftSchema.Validate(successResponse.ScriptData);
				if (string.IsNullOrEmpty(text))
				{
					EventTools.Fire(onSuccess, successResponse.ScriptData.GetDouble("Cooldown") ?? 0.0);
				}
				else
				{
					GameSparksUtils.LogGameSparksError(new GameSparksException("SendGift validation", text, GSError.SchemaValidationError));
					EventTools.Fire(onError);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				GameSparksUtils.LogGameSparksError(new GameSparksException("SendGift", errorResponse));
				EventTools.Fire(onError);
			});
		}

		public void ReceiveGift(string userId, Action<Currencies> onSuccess, Action onError)
		{
			new RequestFriendsReceiveGift(_gameSparksInstance, userId).Send(delegate(LogEventResponse successResponse)
			{
				string text = ReceiveGiftSchema.Validate(successResponse.ScriptData);
				if (string.IsNullOrEmpty(text))
				{
					try
					{
						Currencies value = Currencies.Parse(successResponse.ScriptData.GetGSData("Currencies"));
						EventTools.Fire(onSuccess, value);
					}
					catch (GameSparksException exception)
					{
						GameSparksUtils.LogGameSparksError(exception);
						EventTools.Fire(onError);
					}
				}
				else
				{
					GameSparksUtils.LogGameSparksError(new GameSparksException("ReceiveGift validation", text, GSError.SchemaValidationError));
					EventTools.Fire(onError);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				GameSparksUtils.LogGameSparksError(new GameSparksException("ReceiveGift", errorResponse));
				EventTools.Fire(onError);
			});
		}

		private void OnFriendRequestReceivedMessageReceived(MessageFriendRequestReceived message)
		{
			try
			{
				FriendData friend = new FriendData(message.Data);
				FireFriendRequestReceivedEvent(friend);
			}
			catch (GameSparksException exception)
			{
				GameSparksUtils.LogGameSparksError(exception);
			}
		}

		private void OnFriendRequestAcceptedMessageReceived(MessageFriendRequestAccepted message)
		{
			try
			{
				FriendData friend = new FriendData(message.Data);
				FireFriendRequestAcceptedEvent(friend);
			}
			catch (GameSparksException exception)
			{
				GameSparksUtils.LogGameSparksError(exception);
			}
		}

		private void OnFriendRequestDeclinedMessageReceived(MessageFriendRequestDeclined message)
		{
			try
			{
				FriendData friendData = new FriendData(message.Data);
				FireFriendRequestDeclinedEvent(friendData.FriendCode);
			}
			catch (GameSparksException exception)
			{
				GameSparksUtils.LogGameSparksError(exception);
			}
		}

		private void OnFriendGiftReceived(MessageFriendGiftReceived message)
		{
			try
			{
				FriendData friendData = new FriendData(message.Data);
				FireFriendGiftReceivedEvent(friendData);
			}
			catch (GameSparksException exception)
			{
				GameSparksUtils.LogGameSparksError(exception);
			}
		}
	}
}
