using UnityEngine;

namespace CIG
{
	public class GridOverlay : MonoBehaviour
	{
		public static readonly Color TransparentColor = new Color(0f, 0f, 0f, 0f);

		[SerializeField]
		private MeshFilter _meshFilter;

		[SerializeField]
		private MeshRenderer _meshRenderer;

		private bool _readOnly;

		private Texture2D _overlayColorsTexture;

		private bool _overlayColorsChanged;

		public void Initialize(GridSize size, Vector2 offset, bool readOnly)
		{
			_readOnly = readOnly;
			_overlayColorsTexture = CreateColorsTexture(size);
			_meshFilter.sharedMesh = CreateMesh(size);
			SetMaterialParameters(_meshRenderer.material, size, _overlayColorsTexture);
			_meshRenderer.sortingLayerName = "Background";
			base.transform.localPosition = new Vector3(offset.x, offset.y, -1f);
		}

		private void LateUpdate()
		{
			if (_overlayColorsChanged)
			{
				_overlayColorsTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
				_overlayColorsChanged = false;
			}
		}

		public Color GetColor(GridIndex index)
		{
			if (_overlayColorsTexture == null)
			{
				UnityEngine.Debug.LogError("Trying to get the color of the overlay texture before it is initialized.");
				return TransparentColor;
			}
			return _overlayColorsTexture.GetPixel(index.u, index.v);
		}

		public void SetColor(GridIndex index, Color color)
		{
			if (!_readOnly)
			{
				if (_overlayColorsTexture == null)
				{
					UnityEngine.Debug.LogError("Trying to set the color of the overlay texture before it is initialized.");
					return;
				}
				_overlayColorsTexture.SetPixel(index.u, index.v, color);
				_overlayColorsChanged = true;
			}
		}

		private Texture2D CreateColorsTexture(GridSize size)
		{
			Texture2D texture2D = new Texture2D(size.u, size.v, TextureFormat.ARGB32, mipChain: false);
			texture2D.filterMode = FilterMode.Point;
			for (int i = 0; i < size.v; i++)
			{
				for (int j = 0; j < size.u; j++)
				{
					texture2D.SetPixel(j, i, TransparentColor);
				}
			}
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			return texture2D;
		}

		private Mesh CreateMesh(GridSize size)
		{
			Mesh mesh = new Mesh();
			Vector3[] vertices = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3((float)size.u * (IsometricGrid.ElementSize.x * 0.5f), (float)(-size.u) * (IsometricGrid.ElementSize.y * 0.5f), 0f),
				new Vector3((float)(size.u - size.v) * (IsometricGrid.ElementSize.x * 0.5f), (float)(-(size.u + size.v)) * (IsometricGrid.ElementSize.y * 0.5f), 0f),
				new Vector3((float)(-size.v) * (IsometricGrid.ElementSize.x * 0.5f), (float)(-size.v) * (IsometricGrid.ElementSize.y * 0.5f), 0f)
			};
			Vector2[] uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};
			int[] triangles = new int[6]
			{
				0,
				1,
				2,
				2,
				3,
				0
			};
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		private void SetMaterialParameters(Material material, GridSize size, Texture2D colorsTexture)
		{
			material.SetTexture("_ColorsTex", colorsTexture);
			material.SetFloat("_GridU", size.u);
			material.SetFloat("_GridV", size.v);
		}
	}
}
