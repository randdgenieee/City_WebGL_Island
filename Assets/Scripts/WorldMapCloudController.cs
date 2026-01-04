using CIG;
using System.Collections;
using System.Collections.Generic;
using Tweening.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapCloudController : MonoBehaviour
{
	private class CloudData
	{
		public Image Cloud
		{
			get;
			private set;
		}

		public RectTransform CloudTransform
		{
			get;
			private set;
		}

		public Image Shadow
		{
			get;
			private set;
		}

		public RectTransform ShadowTransform
		{
			get;
			private set;
		}

		public float AnimationOffset
		{
			get;
			private set;
		}

		public CloudData(Image cloud, Image shadow, float animationOffset)
		{
			Cloud = cloud;
			CloudTransform = cloud.rectTransform;
			Shadow = shadow;
			ShadowTransform = shadow.rectTransform;
			AnimationOffset = animationOffset;
		}
	}

	private const float AnimationDuration = 20f;

	private const int CloudCountX = 21;

	private const int CloudCountY = 15;

	private const float CloudSizeX = 400f;

	private const float CloudSizeY = 200f;

	private const float CloudSpacingX = 300f;

	private const float MinimumCloudScale = 1f;

	private const float MaximumCloudScale = 1.3f;

	private const float MinZ = 70f;

	public const float MaxZ = 150f;

	private static readonly Color ShadowColor = new Color(0f, 0f, 0f, 0.15f);

	[SerializeField]
	private Image _cloudPrefab;

	[SerializeField]
	private RectTransform _cloudShadowParent;

	[SerializeField]
	private RectTransform _cloudParent;

	private Timing _timing;

	private WorldMapView _worldMapView;

	private CloudData[,] _clouds;

	private float _animationProgress;

	public void Initialize(Timing timing, WorldMapView worldMapView, List<WorldMapIsland> visibleIslands)
	{
		_timing = timing;
		_worldMapView = worldMapView;
		CreateClouds(visibleIslands);
	}

	private void Update()
	{
		_animationProgress += _timing.GetDeltaTime(DeltaTimeType.Animation);
		_animationProgress %= 20f;
		float num = _animationProgress * 0.05f;
		int i = 0;
		for (int length = _clouds.GetLength(0); i < length; i++)
		{
			int j = 0;
			for (int length2 = _clouds.GetLength(1); j < length2; j++)
			{
				CloudData cloudData = _clouds[i, j];
				if (cloudData != null)
				{
					float num2 = num + cloudData.AnimationOffset;
					float num3 = num2 - Mathf.Floor(num2);
					float t = 1f - Mathf.Abs(num3 * 2f - 1f);
					Vector3 localScale = new Vector3(Mathf.SmoothStep(1f, 1.3f, t), Mathf.SmoothStep(1.3f, 1f, t), 1f);
					cloudData.CloudTransform.localScale = localScale;
					cloudData.ShadowTransform.localScale = localScale;
				}
			}
		}
	}

	public IEnumerator FadeCloudsAround(WorldMapIsland worldMapIsland)
	{
		List<Vector2Int> indices = GetCloudIndicesAround(worldMapIsland);
		if (indices.Count > 0)
		{
			_worldMapView.CameraOperator.ScrollAndZoom(worldMapIsland.transform.position, 1400f);
			yield return new WaitForSeconds(0.6f);
			int i = 0;
			for (int count = indices.Count; i < count; i++)
			{
				Vector2Int vector2Int = indices[i];
				CloudData cloud = _clouds[vector2Int.x, vector2Int.y];
				StartCoroutine(FadeCloudRoutine(cloud));
				_clouds[vector2Int.x, vector2Int.y] = null;
			}
			yield return new WaitForSeconds(2f);
		}
	}

	private void CreateClouds(List<WorldMapIsland> visibleIslands)
	{
		HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
		int i = 0;
		for (int count = visibleIslands.Count; i < count; i++)
		{
			WorldMapIsland worldMapIsland = visibleIslands[i];
			hashSet.UnionWith(GetCloudIndicesAround(worldMapIsland));
		}
		List<Transform> list = new List<Transform>();
		_clouds = new CloudData[21, 15];
		int j = 0;
		for (int length = _clouds.GetLength(0); j < length; j++)
		{
			int k = 0;
			for (int length2 = _clouds.GetLength(1); k < length2; k++)
			{
				if (!hashSet.Contains(new Vector2Int(j, k)))
				{
					float value = UnityEngine.Random.value;
					float num = UnityEngine.Random.Range(0.9f, 1.1f);
					Vector3 vector = new Vector3(((float)j - 10f) * 300f, ((float)k - 7f) * 200f + (float)(j % 2) * 200f * 0.5f, 0f);
					Image image = UnityEngine.Object.Instantiate(_cloudPrefab, _cloudParent);
					image.transform.localPosition = vector + Vector3.back * UnityEngine.Random.Range(70f, 150f);
					image.rectTransform.sizeDelta *= num;
					Image image2 = UnityEngine.Object.Instantiate(_cloudPrefab, _cloudShadowParent);
					image2.transform.localPosition = vector;
					image2.rectTransform.sizeDelta *= num;
					image2.color = ShadowColor;
					_clouds[j, k] = new CloudData(image, image2, value);
					list.Add(image.transform);
				}
			}
		}
		list.Sort(CloudSort);
		int l = 0;
		for (int count2 = list.Count; l < count2; l++)
		{
			list[l].SetSiblingIndex(l);
		}
	}

	private static int CloudSort(Transform lhs, Transform rhs)
	{
		if (lhs.localPosition.z < rhs.localPosition.z)
		{
			return 1;
		}
		if (lhs.localPosition.z > rhs.localPosition.z)
		{
			return -1;
		}
		return 0;
	}

	private List<Vector2Int> GetCloudIndicesAround(WorldMapIsland worldMapIsland)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		int num = Mathf.RoundToInt(worldMapIsland.transform.localPosition.x / 300f + 10f);
		int num2 = Mathf.RoundToInt((worldMapIsland.transform.localPosition.y - (float)(num % 2) * 200f * 0.5f) / 200f + 7f);
		list.Add(new Vector2Int(num, num2));
		list.Add(new Vector2Int(num, num2 + 1));
		list.Add(new Vector2Int(num, num2 - 1));
		list.Add(new Vector2Int(num + 1, num2));
		list.Add(new Vector2Int(num - 1, num2));
		int num3 = (num % 2 != 0) ? 1 : (-1);
		list.Add(new Vector2Int(num + 1, num2 + num3));
		list.Add(new Vector2Int(num - 1, num2 + num3));
		for (int num4 = list.Count - 1; num4 >= 0; num4--)
		{
			Vector2Int vector2Int = list[num4];
			if (vector2Int.x < 0 || vector2Int.x >= 21 || vector2Int.y < 0 || vector2Int.y >= 15 || (_clouds != null && _clouds[vector2Int.x, vector2Int.y] == null))
			{
				list.RemoveAt(num4);
			}
		}
		return list;
	}

	private static IEnumerator FadeCloudRoutine(CloudData cloud)
	{
		float t = 0f;
		Vector3 initialScale = cloud.CloudTransform.localScale;
		float initialCloudAlpha = cloud.Cloud.color.a;
		float initialShadowAlpha = cloud.Shadow.color.a;
		while (t < 1f)
		{
			t += Time.deltaTime;
			Vector3 localScale = initialScale * (1f + t * 3f);
			cloud.CloudTransform.localScale = localScale;
			cloud.ShadowTransform.localScale = localScale;
			float num = 1f - t;
			cloud.Cloud.UpdateColorAlpha(initialCloudAlpha * num);
			cloud.Shadow.UpdateColorAlpha(initialShadowAlpha * num);
			yield return null;
		}
		UnityEngine.Object.Destroy(cloud.CloudTransform.gameObject);
		UnityEngine.Object.Destroy(cloud.ShadowTransform.gameObject);
	}
}
