using System.Collections;
using TMPro;
using UnityEngine;

namespace CIG
{
	public class UpgradeSign : MonoBehaviour
	{
		private const float ParticleMinWaitTime = 8f;

		private const float ParticleMaxWaitTime = 16f;

		[SerializeField]
		private SpriteRenderer _iconLevel0;

		[SerializeField]
		private SpriteRenderer _iconUpgraded;

		[SerializeField]
		private TextMeshPro _currentLevelText;

		[SerializeField]
		private Color _lowLevelColor;

		[SerializeField]
		private Color _midLevelColor;

		[SerializeField]
		private Color _highLevelColor;

		[SerializeField]
		private BuildingParticles _maxLevelParticles;

		private IEnumerator _particleCoroutine;

		private bool _playParticles;

		private bool _mirrored;

		private bool _visiting;

		public void Initialize(GridTile building, bool visiting)
		{
			_visiting = visiting;
			_maxLevelParticles.Initialize(building);
		}

		private void OnEnable()
		{
			if (_playParticles)
			{
				_maxLevelParticles.Play(8f, 16f);
			}
		}

		private void OnDisable()
		{
			_maxLevelParticles.Stop();
		}

		public void UpdateSign(bool mirrored, int buildingLevel, int buildingMaxLevel)
		{
			CheckBuildingMirrored(mirrored);
			if (buildingLevel >= 1)
			{
				CheckBuildingLevel(buildingLevel, buildingMaxLevel);
			}
			else if (_visiting)
			{
				base.gameObject.SetActive(value: true);
				SetSignColorAndText(buildingLevel, _lowLevelColor);
			}
		}

		public void UpdateSortingOrder(int newSortingOrder)
		{
			_iconLevel0.sortingOrder = newSortingOrder + 1;
			_iconUpgraded.sortingOrder = newSortingOrder + 1;
			_currentLevelText.sortingOrder = newSortingOrder + 1;
			_maxLevelParticles.SetSortingOrder(newSortingOrder);
		}

		public void SetHidden(bool hidden)
		{
			if (hidden)
			{
				_maxLevelParticles.Stop();
			}
			else if (_playParticles)
			{
				_maxLevelParticles.Play(8f, 16f);
			}
		}

		private void CheckBuildingLevel(int level, int maxLevel)
		{
			base.gameObject.SetActive(value: true);
			float num = (float)level / (float)maxLevel;
			if (num < 0.33f)
			{
				SetSignColorAndText(level, _lowLevelColor);
			}
			else if (num < 0.66f)
			{
				SetSignColorAndText(level, _midLevelColor);
			}
			else if (num < 1f)
			{
				SetSignColorAndText(level, _highLevelColor);
			}
			else if (base.gameObject.activeInHierarchy)
			{
				_playParticles = true;
				_maxLevelParticles.Play(8f, 16f);
				_iconLevel0.gameObject.SetActive(value: false);
				_iconUpgraded.gameObject.SetActive(value: false);
			}
		}

		private void CheckBuildingMirrored(bool mirrored)
		{
			Vector3 localScale = base.transform.localScale;
			localScale.x = (mirrored ? (0f - Mathf.Abs(localScale.x)) : Mathf.Abs(localScale.x));
			base.transform.localScale = localScale;
		}

		private void SetSignColorAndText(int level, Color color)
		{
			_iconLevel0.gameObject.SetActive(value: false);
			_iconUpgraded.gameObject.SetActive(value: true);
			_currentLevelText.text = level.ToString();
			_currentLevelText.faceColor = color;
		}
	}
}
