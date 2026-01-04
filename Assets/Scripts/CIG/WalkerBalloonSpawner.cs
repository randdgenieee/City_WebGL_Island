using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class WalkerBalloonSpawner : MonoBehaviour
	{
		private const float MinSpawnDelay = 5f;

		private const float MaxSpawnDelay = 30f;

		private const int MaxBalloons = 5;

		private WalkerBalloonFactory _balloonFactory;

		private RoadAgentSpawner _roadAgentSpawner;

		private TutorialManager _tutorialManager;

		private Timing _timing;

		private List<WalkerBalloonProperties> _allBalloonProperties;

		private readonly List<WalkerBalloon> _activeBalloons = new List<WalkerBalloon>();

		private double _nextSpawnTime;

		public bool CanSpawnBalloons
		{
			get
			{
				if (_tutorialManager.InitialTutorialFinished && _nextSpawnTime <= _timing.GameTime)
				{
					return _activeBalloons.Count < 5;
				}
				return false;
			}
		}

		public void Initialize(GameState gameState, PopupManager popupManager, RoutineRunner routineRunner, RoadAgentSpawner roadAgentSpawner, TutorialManager tutorialManager, CityAdvisor cityAdvisor, WebService webService, Timing timing, List<WalkerBalloonProperties> allBalloonProperties)
		{
			_roadAgentSpawner = roadAgentSpawner;
			_tutorialManager = tutorialManager;
			_timing = timing;
			_balloonFactory = new WalkerBalloonFactory(gameState, popupManager, routineRunner, cityAdvisor, webService);
			_allBalloonProperties = allBalloonProperties;
		}

		private void OnDestroy()
		{
			_tutorialManager = null;
			_roadAgentSpawner = null;
		}

		private void Update()
		{
			if (CanSpawnBalloons)
			{
				SpawnBalloon();
			}
		}

		private void SpawnBalloon()
		{
			RoadAgent randomRoadAgent = GetRandomRoadAgent();
			if (randomRoadAgent != null)
			{
				List<WalkerBalloonProperties> list = new List<WalkerBalloonProperties>(_allBalloonProperties);
				while (list.Count > 0)
				{
					WalkerBalloonProperties walkerBalloonProperties = list.PickRandom();
					WalkerBalloon walkerBalloon = _balloonFactory.CreateBalloon(walkerBalloonProperties);
					if (walkerBalloon != null && walkerBalloon.IsAvailable)
					{
						SpawnBalloon(randomRoadAgent, walkerBalloon);
						break;
					}
					list.Remove(walkerBalloonProperties);
				}
			}
			_nextSpawnTime = _timing.GameTime + (double)UnityEngine.Random.Range(5f, 30f);
		}

		private void SpawnBalloon(RoadAgent agent, WalkerBalloon balloon)
		{
			agent.ShowBalloon(balloon);
			_activeBalloons.Add(balloon);
			balloon.ExpiredEvent += OnBalloonExpired;
		}

		private RoadAgent GetRandomRoadAgent()
		{
			return _roadAgentSpawner.GetRoadAgents((RoadAgent a) => a.WalkerBalloon == null).PickRandom();
		}

		private void OnBalloonExpired(WalkerBalloon walkerBalloon)
		{
			walkerBalloon.ExpiredEvent -= OnBalloonExpired;
			_activeBalloons.Remove(walkerBalloon);
		}
	}
}
