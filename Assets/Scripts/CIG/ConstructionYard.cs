using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class ConstructionYard : MonoBehaviour
	{
		[Serializable]
		private class FloorSprites
		{
			[SerializeField]
			private Sprite _northSprite;

			[SerializeField]
			private Sprite _northEastSprite;

			[SerializeField]
			private Sprite _eastSprite;

			[SerializeField]
			private Sprite _southEastSprite;

			[SerializeField]
			private Sprite _southSprite;

			[SerializeField]
			private Sprite _southWestSprite;

			[SerializeField]
			private Sprite _westSprite;

			[SerializeField]
			private Sprite _northWestSprite;

			[SerializeField]
			private Sprite[] _centerSprites;

			public Sprite NorthSprite => _northSprite;

			public Sprite NorthEastSprite => _northEastSprite;

			public Sprite EastSprite => _eastSprite;

			public Sprite SouthEastSprite => _southEastSprite;

			public Sprite SouthSprite => _southSprite;

			public Sprite SouthWestSprite => _southWestSprite;

			public Sprite WestSprite => _westSprite;

			public Sprite NorthWestSprite => _northWestSprite;

			public Sprite[] CenterSprites => _centerSprites;
		}

		[Serializable]
		private class ScaffoldingSprites
		{
			[SerializeField]
			private Sprite[] _northEastSprites;

			[SerializeField]
			private Sprite[] _southEastSprites;

			[SerializeField]
			private Sprite[] _southWestSprites;

			[SerializeField]
			private Sprite[] _northWestSprites;

			public Sprite[] NorthEastSprites => _northEastSprites;

			public Sprite[] SouthEastSprites => _southEastSprites;

			public Sprite[] SouthWestSprites => _southWestSprites;

			public Sprite[] NorthWestSprites => _northWestSprites;
		}

		private class ScaffoldingAnimationStep
		{
			public ConstructionScaffolding Scaffolding
			{
				get;
			}

			public bool Active
			{
				get;
			}

			public int Stage
			{
				get;
			}

			public ScaffoldingAnimationStep(ConstructionScaffolding scaffolding, int stage, bool active)
			{
				Scaffolding = scaffolding;
				Stage = stage;
				Active = active;
			}
		}

		private const int MaxScaffoldingLayers = 3;

		private const float ScaffoldingHeight = 100f;

		private const double MaxBuildDurationSeconds = 60.0;

		private const int ScaffoldingFrameCount = 3;

		private const double ShakeDuration = 0.5;

		private const int NorthEastScaffoldingSortingOffset = 1;

		private const int NorthWestScaffoldingSortingOffset = 2;

		private const int BuildingSortingOffset = 3;

		private const int SouthEastScaffoldingSortingOffsetFromBuilding = 1;

		private const int SouthWestScaffoldingSortingOffsetFromBuilding = 2;

		[Header("Building")]
		[SerializeField]
		private GridTileSpriteRenderer _buildingRenderer;

		[SerializeField]
		private PolygonCollider2D _buildingCollider;

		[Header("Floor")]
		[SerializeField]
		private FloorSprites _floorSprites;

		[SerializeField]
		private ConstructionFloorTile _floorPrefab;

		[SerializeField]
		private Transform _floorParent;

		[Header("Scaffolding")]
		[SerializeField]
		private ScaffoldingSprites _scaffoldingSprites;

		[SerializeField]
		private ConstructionScaffolding _scaffoldingPrefab;

		[SerializeField]
		private Transform _scaffoldingParent;

		[Header("Particles")]
		[SerializeField]
		private ParticleSystem _smokeParticles;

		[SerializeField]
		private ParticleSystem _bangParticles;

		private Building _building;

		private Timing _timing;

		private RoutineRunner _routineRunner;

		private int _sortingOrder;

		private int _scaffoldingLayers;

		private ConstructionFloorTile[,] _floorTiles;

		private List<ConstructionScaffolding>[] _scaffoldingTiles;

		private IEnumerator _scaffoldingRoutine;

		public void Initialize(Building building, Timing timing, RoutineRunner routineRunner)
		{
			_building = building;
			_timing = timing;
			_routineRunner = routineRunner;
			_sortingOrder = _building.SpriteRenderer.sortingOrder;
			_scaffoldingLayers = Mathf.Clamp(Mathf.FloorToInt(_building.SpriteRenderer.size.y / 100f), 1, 3);
			_buildingRenderer.Copy(_building.GridTileSpriteRenderer);
			_buildingRenderer.SetSortingOrder(_sortingOrder + 3);
			_buildingRenderer.SetPaused(paused: true);
			_buildingCollider.CopyPathsFrom(_building.Collider);
			CreateFloorTiles(_building.Properties.Size);
			CreateScaffoldings(_building.Properties.Size);
			SetParticles(_smokeParticles, _building);
			SetParticles(_bangParticles, _building);
			_routineRunner.StartCoroutine(_scaffoldingRoutine = ScaffoldingRoutine(_building));
		}

		private void OnDestroy()
		{
			if (_routineRunner != null && _scaffoldingRoutine != null)
			{
				_routineRunner.StopCoroutine(_scaffoldingRoutine);
				_scaffoldingRoutine = null;
			}
		}

		public void ResetAnimation()
		{
			if (_scaffoldingRoutine != null)
			{
				_routineRunner.StopCoroutine(_scaffoldingRoutine);
			}
			_routineRunner.StartCoroutine(_scaffoldingRoutine = ScaffoldingRoutine(_building));
		}

		public void OnHiddenChanged(bool hidden)
		{
			base.gameObject.SetActive(!hidden);
		}

		private void CreateFloorTiles(GridSize size)
		{
			_floorTiles = new ConstructionFloorTile[size.u, size.v];
			if (size.u == 1 && size.v == 1)
			{
				if (_floorSprites.CenterSprites.Length != 0)
				{
					CreateFloorTileAt(_floorSprites.CenterSprites.PickRandom(), 0, 0);
				}
				return;
			}
			CreateFloorTileAt(_floorSprites.NorthSprite, size.u - 1, size.v - 1);
			CreateFloorTileAt(_floorSprites.EastSprite, size.u - 1, 0);
			CreateFloorTileAt(_floorSprites.SouthSprite, 0, 0);
			CreateFloorTileAt(_floorSprites.WestSprite, 0, size.v - 1);
			int i = 1;
			for (int num = size.v - 1; i < num; i++)
			{
				CreateFloorTileAt(_floorSprites.NorthEastSprite, size.u - 1, i);
			}
			int j = 1;
			for (int num2 = size.u - 1; j < num2; j++)
			{
				CreateFloorTileAt(_floorSprites.SouthEastSprite, j, 0);
			}
			int k = 1;
			for (int num3 = size.v - 1; k < num3; k++)
			{
				CreateFloorTileAt(_floorSprites.SouthWestSprite, 0, k);
			}
			int l = 1;
			for (int num4 = size.u - 1; l < num4; l++)
			{
				CreateFloorTileAt(_floorSprites.NorthWestSprite, l, size.v - 1);
			}
			if (_floorSprites.CenterSprites.Length == 0)
			{
				return;
			}
			int m = 1;
			for (int num5 = size.u - 1; m < num5; m++)
			{
				int n = 1;
				for (int num6 = size.v - 1; n < num6; n++)
				{
					CreateFloorTileAt(_floorSprites.CenterSprites.PickRandom(), m, n);
				}
			}
		}

		private void CreateFloorTileAt(Sprite sprite, int u, int v)
		{
			ConstructionFloorTile constructionFloorTile = UnityEngine.Object.Instantiate(_floorPrefab, _floorParent);
			constructionFloorTile.Initialize(sprite, _sortingOrder);
			constructionFloorTile.transform.localPosition = GetLocalPosition(u, v, -(u + v));
			constructionFloorTile.name = sprite.name;
			_floorTiles[u, v] = constructionFloorTile;
		}

		private void CreateScaffoldings(GridSize size)
		{
			_scaffoldingTiles = new List<ConstructionScaffolding>[_scaffoldingLayers];
			int i = 0;
			for (int num = _scaffoldingTiles.Length; i < num; i++)
			{
				List<ConstructionScaffolding> list = new List<ConstructionScaffolding>();
				int num2 = 3 + _buildingRenderer.ChildSpriteRenderers.Count + 1;
				for (int num3 = size.v - 1; num3 >= 0; num3--)
				{
					ConstructionScaffolding item = CreateScaffoldingAt(_scaffoldingSprites.NorthEastSprites, size.u - 1, num3, i, 1);
					list.Add(item);
				}
				for (int num4 = size.u - 1; num4 >= 0; num4--)
				{
					ConstructionScaffolding item2 = CreateScaffoldingAt(_scaffoldingSprites.SouthEastSprites, num4, 0, i, num2 + 1);
					list.Add(item2);
				}
				int j = 0;
				for (int v = size.v; j < v; j++)
				{
					ConstructionScaffolding item3 = CreateScaffoldingAt(_scaffoldingSprites.SouthWestSprites, 0, j, i, num2 + 2);
					list.Add(item3);
				}
				int k = 0;
				for (int u = size.u; k < u; k++)
				{
					ConstructionScaffolding item4 = CreateScaffoldingAt(_scaffoldingSprites.NorthWestSprites, k, size.v - 1, i, 2);
					list.Add(item4);
				}
				_scaffoldingTiles[i] = list;
			}
		}

		private ConstructionScaffolding CreateScaffoldingAt(Sprite[] sprites, int u, int v, int layer, int directionOffset)
		{
			float z = _scaffoldingLayers * (u + v - layer);
			ConstructionScaffolding constructionScaffolding = UnityEngine.Object.Instantiate(_scaffoldingPrefab, _scaffoldingParent);
			constructionScaffolding.Initialize(sprites, _sortingOrder + directionOffset);
			constructionScaffolding.transform.localPosition = GetLocalPosition((float)u + (float)layer * 0.5f, (float)v + (float)layer * 0.5f, z);
			constructionScaffolding.name = sprites[0].name;
			return constructionScaffolding;
		}

		private void SetParticles(ParticleSystem particles, Building building)
		{
			particles.GetComponent<Renderer>().sortingOrder = building.SpriteRenderer.sortingOrder + 1;
			particles.transform.localScale = new Vector3(building.Properties.Size.u, building.Properties.Size.v, 1f);
		}

		private Vector3 GetLocalPosition(float u, float v, float z)
		{
			float x = (u - v) * IsometricGrid.ElementSize.x * 0.5f;
			float y = (u + v) * IsometricGrid.ElementSize.y * 0.5f;
			return new Vector3(x, y, z);
		}

		private ScaffoldingAnimationStep[] BuildUpAnimationSteps(int scaffoldingCount)
		{
			ScaffoldingAnimationStep[] array = new ScaffoldingAnimationStep[scaffoldingCount];
			int num = 0;
			int i = 0;
			for (int num2 = _scaffoldingTiles.Length; i < num2; i++)
			{
				List<ConstructionScaffolding> list = _scaffoldingTiles[i];
				for (int j = 0; j < 3; j++)
				{
					int k = 0;
					for (int count = list.Count; k < count; k++)
					{
						ConstructionScaffolding constructionScaffolding = list[k];
						constructionScaffolding.gameObject.SetActive(value: false);
						array[num] = new ScaffoldingAnimationStep(constructionScaffolding, j, active: true);
						num++;
					}
				}
			}
			return array;
		}

		private ScaffoldingAnimationStep[] BuildDownAnimationSteps(int scaffoldingCount)
		{
			ScaffoldingAnimationStep[] array = new ScaffoldingAnimationStep[scaffoldingCount];
			int num = 0;
			for (int num2 = _scaffoldingTiles.Length - 1; num2 >= 0; num2--)
			{
				List<ConstructionScaffolding> list = _scaffoldingTiles[num2];
				for (int num3 = 1; num3 >= 0; num3--)
				{
					int i = 0;
					for (int count = list.Count; i < count; i++)
					{
						ConstructionScaffolding constructionScaffolding = list[i];
						constructionScaffolding.gameObject.SetActive(value: true);
						array[num] = new ScaffoldingAnimationStep(constructionScaffolding, num3, active: true);
						num++;
					}
				}
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					array[num] = new ScaffoldingAnimationStep(list[j], 0, active: false);
					num++;
				}
			}
			return array;
		}

		private IEnumerator ScaffoldingRoutine(Building building)
		{
			_smokeParticles.Play();
			double buildAnimationDurationSeconds = Math.Min(60.0, (float)building.BuildingProperties.ConstructionDurationSeconds * 0.5f);
			int scaffoldingCount = _scaffoldingTiles[0].Count * 3 * _scaffoldingTiles.Length;
			_buildingRenderer.SetHidden(hidden: true);
			_buildingCollider.enabled = false;
			yield return ScaffoldingAnimationRoutine(BuildUpAnimationSteps(scaffoldingCount), (double)building.BuildingProperties.ConstructionDurationSeconds - building.ConstructionTimeLeft, buildAnimationDurationSeconds - 0.5);
			_buildingRenderer.SetHidden(hidden: false);
			_buildingCollider.enabled = true;
			_bangParticles.Play();
			_floorParent.gameObject.SetActive(value: false);
			yield return ShakeConstructionYard(0.5);
			double timeToWait = building.ConstructionTimeLeft - buildAnimationDurationSeconds;
			yield return new WaitForGameTimeSeconds(_timing, timeToWait);
			yield return ScaffoldingAnimationRoutine(BuildDownAnimationSteps(scaffoldingCount), buildAnimationDurationSeconds - building.ConstructionTimeLeft, buildAnimationDurationSeconds);
			_smokeParticles.Stop();
			_scaffoldingRoutine = null;
		}

		private IEnumerator ShakeConstructionYard(double duration)
		{
			double shakeTimeLeft = duration;
			Vector3 constructionYardPosition = base.transform.position;
			while (shakeTimeLeft > 0.0)
			{
				shakeTimeLeft -= (double)_timing.GetDeltaTime(DeltaTimeType.Game);
				base.transform.position = constructionYardPosition + UnityEngine.Random.insideUnitSphere * 2f;
				yield return null;
			}
			base.transform.position = constructionYardPosition;
		}

		private IEnumerator ScaffoldingAnimationRoutine(ScaffoldingAnimationStep[] animationSteps, double animationTimePassed, double buildAnimationDurationSeconds)
		{
			double scaffoldingDelaySeconds = buildAnimationDurationSeconds / (double)animationSteps.Length;
			double num = animationTimePassed / scaffoldingDelaySeconds;
			int startIndex = Mathf.Clamp((int)Math.Floor(num), 0, animationSteps.Length);
			for (int j = 0; j < startIndex; j++)
			{
				ScaffoldingAnimationStep scaffoldingAnimationStep = animationSteps[j];
				scaffoldingAnimationStep.Scaffolding.gameObject.SetActive(scaffoldingAnimationStep.Active);
				scaffoldingAnimationStep.Scaffolding.SetStage(scaffoldingAnimationStep.Stage);
			}
			double timeToWait = (1.0 - (num - (double)startIndex)) * scaffoldingDelaySeconds;
			yield return new WaitForGameTimeSeconds(_timing, timeToWait);
			int i = startIndex;
			for (int length = animationSteps.Length; i < length; i++)
			{
				ScaffoldingAnimationStep scaffoldingAnimationStep2 = animationSteps[i];
				scaffoldingAnimationStep2.Scaffolding.gameObject.SetActive(scaffoldingAnimationStep2.Active);
				scaffoldingAnimationStep2.Scaffolding.SetStage(scaffoldingAnimationStep2.Stage);
				yield return new WaitForGameTimeSeconds(_timing, scaffoldingDelaySeconds);
			}
		}
	}
}
