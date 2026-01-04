using System;
using System.Collections.Generic;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class TreasureChestRenderer : MonoBehaviour
	{
		[Serializable]
		private class ChestPrefab
		{
			[SerializeField]
			private TreasureChestType _treasureChestType;

			[SerializeField]
			private TreasureChestView _chestPrefab;

			public TreasureChestType TreasureChestType => _treasureChestType;

			public TreasureChestView Prefab => _chestPrefab;
		}

		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private Tweener _cameraTweener;

		[SerializeField]
		private List<ChestPrefab> _chestPrefabs;

		private RenderTexture _renderTexture;

		private TreasureChestView _chestInstance;

		public RenderTexture RenderTexture
		{
			get
			{
				if (_renderTexture == null)
				{
					_renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
					_renderTexture.Create();
					_camera.targetTexture = _renderTexture;
				}
				return _renderTexture;
			}
		}

		public TreasureChestView Initialize(TreasureChestType treasureChestType, List<RewardItemData> rewardItemData)
		{
			List<RewardItemData> list = new List<RewardItemData>();
			for (int num = rewardItemData.Count - 1; num >= 0; num--)
			{
				RewardItemData rewardItemData2 = rewardItemData[num];
				if (rewardItemData2.BuildingProperties is LandmarkBuildingProperties)
				{
					list.Add(rewardItemData2);
				}
				else
				{
					list.Insert(0, rewardItemData2);
				}
			}
			ChestPrefab chestPrefab = _chestPrefabs.Find((ChestPrefab p) => p.TreasureChestType == treasureChestType);
			_chestInstance = UnityEngine.Object.Instantiate(chestPrefab.Prefab, base.transform);
			_chestInstance.Initialize(treasureChestType, list, _cameraTweener);
			base.gameObject.SetActive(value: true);
			return _chestInstance;
		}

		public void Deinitialize()
		{
			if (_chestInstance != null)
			{
				_chestInstance.Deinitialize();
				UnityEngine.Object.Destroy(_chestInstance.gameObject);
				_chestInstance = null;
			}
			base.gameObject.SetActive(value: false);
		}

		private void OnDestroy()
		{
			if (_renderTexture != null)
			{
				_renderTexture.Release();
				_renderTexture = null;
			}
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}
	}
}
