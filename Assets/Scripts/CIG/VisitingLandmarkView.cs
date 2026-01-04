using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class VisitingLandmarkView : MonoBehaviour
	{
		private ReadOnlyWrapper _parent;

		private int _boostTiles;

		private IsometricGrid _isometricGrid;

		private HashSet<ReadOnlyWrapper> _buildingsInRange;

		private HashSet<ReadOnlyWrapper> BuildingsInRange => _buildingsInRange ?? (_buildingsInRange = GetBuildingsInRange());

		public void Initialize(StorageDictionary storage, ReadOnlyWrapper parent, IsometricGrid isometricGrid, BuildingParticles landmarkParticlesPrefab)
		{
			_parent = parent;
			_isometricGrid = isometricGrid;
			_boostTiles = storage.Get("CurrentBoostTiles", 0);
			BuildingParticles buildingParticles = UnityEngine.Object.Instantiate(landmarkParticlesPrefab, base.transform);
			buildingParticles.Initialize(parent);
			buildingParticles.PlayedEvent += OnParticlesPlayed;
			buildingParticles.Play(20f, 30f);
		}

		private HashSet<ReadOnlyWrapper> GetBuildingsInRange()
		{
			HashSet<ReadOnlyWrapper> hashSet = _isometricGrid.FindInRange<ReadOnlyWrapper>(_parent, _boostTiles);
			hashSet.RemoveWhere((ReadOnlyWrapper b) => b.IsOfType(ReadOnlyWrapper.BuildingType.Scenery, ReadOnlyWrapper.BuildingType.Landmark, ReadOnlyWrapper.BuildingType.Unknown));
			return hashSet;
		}

		private IEnumerator PlayOtherBuildingsParticlesRoutine()
		{
			yield return new WaitForSeconds(0.5f);
			foreach (ReadOnlyWrapper item in BuildingsInRange)
			{
				item.PulseColor(CIGLandmarkBuilding.ToColor, 2f);
			}
		}

		private void OnParticlesPlayed()
		{
			_parent.PulseColor(CIGLandmarkBuilding.ToColor, 2f);
			StartCoroutine(PlayOtherBuildingsParticlesRoutine());
		}
	}
}
