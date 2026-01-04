using SparkLinq;
using System.Collections.Generic;

namespace CIG
{
	public class Leaderboard : IStorable
	{
		private const string EntriesKey = "Entries";

		public List<LeaderboardEntry> Entries
		{
			get;
			private set;
		}

		public bool IsEmpty => Entries.Count == 0;

		public Leaderboard()
		{
			Entries = new List<LeaderboardEntry>();
		}

		public Leaderboard(List<LeaderboardEntry> entries)
		{
			Entries = entries;
			Entries.Sort((LeaderboardEntry lhs, LeaderboardEntry rhs) => lhs.Rank - rhs.Rank);
		}

		public Leaderboard(StorageDictionary storage, GameSparksAuthenticator authenticator)
		{
			Entries = storage.GetModels("Entries", (StorageDictionary sd) => new LeaderboardEntry(sd, authenticator));
		}

		public bool ContainsUser(string userId)
		{
			return Entries.Any((LeaderboardEntry e) => e.UserId == userId);
		}

		public void AddLocalEntry(GameSparksAuthenticator authenticator, int score, int level, int population)
		{
			if (ContainsUser(authenticator.CurrentAuthentication.UserId))
			{
				return;
			}
			int num = Entries.FindIndex((LeaderboardEntry e) => e.Score < score);
			int rank;
			if (num < 0)
			{
				LeaderboardEntry leaderboardEntry = Entries.Last();
				rank = ((leaderboardEntry != null) ? (leaderboardEntry.Rank + 1) : 0);
			}
			else
			{
				rank = Entries[num].Rank;
			}
			LeaderboardEntry item = new LeaderboardEntry(authenticator, score, level, population, rank, canVisit: false);
			if (num >= 0)
			{
				int i = num;
				for (int count = Entries.Count; i < count; i++)
				{
					Entries[i].Rank++;
				}
			}
			if (num < 0)
			{
				Entries.Add(item);
			}
			else
			{
				Entries.Insert(num, item);
			}
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Entries", Entries);
			return storageDictionary;
		}
	}
}
