using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class RoadAgentSpawner : MonoBehaviour
	{
		private static readonly Dictionary<RoadType, float> RoadTypeToRoadAgentRatio = new Dictionary<RoadType, float>
		{
			{
				RoadType.Road,
				0.08f
			},
			{
				RoadType.Path,
				0.125f
			},
			{
				RoadType.River,
				0.08f
			}
		};

		private IsometricGrid _isometricGrid;

		private RoadBuilder _roadBuilder;

		private Timing _timing;

		private OverlayManager _overlayManager;

		private readonly List<RoadAgent> _activeRoadAgents = new List<RoadAgent>();

		public void Initialize(IsometricGrid isometricGrid, RoadBuilder roadBuilder, Timing timing, OverlayManager overlayManager)
		{
			_isometricGrid = isometricGrid;
			_roadBuilder = roadBuilder;
			_timing = timing;
			_overlayManager = overlayManager;
			SpawnRoadAgents();
			_roadBuilder.RoadBuiltEvent += OnRoadBuilt;
			_roadBuilder.RoadRemovedEvent += OnRoadRemoved;
		}

		private void OnDestroy()
		{
			if (_roadBuilder != null)
			{
				_roadBuilder.RoadBuiltEvent -= OnRoadBuilt;
				_roadBuilder.RoadRemovedEvent -= OnRoadRemoved;
				_roadBuilder = null;
			}
			_isometricGrid = null;
		}

		public List<RoadAgent> GetRoadAgents(Predicate<RoadAgent> predicate)
		{
			return _activeRoadAgents.FindAll(predicate);
		}

		private void SpawnRoadAgents()
		{
			foreach (KeyValuePair<RoadType, float> kvp in RoadTypeToRoadAgentRatio)
			{
				int num = (int)((float)_roadBuilder.GetRoadCount((Road r) => r.RoadType.Contains(kvp.Key)) * kvp.Value);
				List<RoadAgent> list = _activeRoadAgents.FindAll((RoadAgent x) => x.RoadType.Contains(kvp.Key));
				int num2 = num - list.Count;
				if (num2 > 0)
				{
					if (!_roadBuilder.RoadExists((Road x) => x.RoadType.Contains(kvp.Key) && x.HasNeighbour(kvp.Key)))
					{
						break;
					}
					for (int i = 0; i < num2; i++)
					{
						RoadAgent roadAgent = UnityEngine.Object.Instantiate(SingletonMonobehaviour<RoadAgentsAssetCollection>.Instance.GetAsset(kvp.Key).PickRandom(), _isometricGrid.transform);
						roadAgent.Initialize(_isometricGrid, _timing, _overlayManager, kvp.Key);
						roadAgent.RemovedEvent += OnRoadAgentRemoved;
						_activeRoadAgents.Add(roadAgent);
					}
				}
				else if (num2 < 0)
				{
					for (int j = num2; j < 0; j++)
					{
						list.PickRandom().Remove();
					}
				}
			}
		}

		private void OnRoadAgentRemoved(MovingAgent movingAgent)
		{
			_activeRoadAgents.Remove(movingAgent as RoadAgent);
			SpawnRoadAgents();
			movingAgent.RemovedEvent -= OnRoadAgentRemoved;
		}

		private void OnRoadRemoved(Road road)
		{
			SpawnRoadAgents();
		}

		private void OnRoadBuilt(Road road)
		{
			SpawnRoadAgents();
		}
	}
}
