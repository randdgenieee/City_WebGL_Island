using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GridTileIconManager : MonoBehaviour
	{
		private OverlayManager _overlayManager;

		private readonly Dictionary<GridTileIconType, GridTileIcon> _icons = new Dictionary<GridTileIconType, GridTileIcon>();

		private GridTileIcon _currentIcon;

		public void Initialize(OverlayManager overlayManager)
		{
			_overlayManager = overlayManager;
			_overlayManager.InteractionEnabledChangedEvent += OnInteractionEnabledChanged;
			Builder.TilesHiddenChangedEvent += OnTilesHiddenChanged;
		}

		private void OnDestroy()
		{
			if (_overlayManager != null)
			{
				_overlayManager.InteractionEnabledChangedEvent -= OnInteractionEnabledChanged;
				_overlayManager = null;
			}
		}

		public T SetIcon<T>(GridTileIconType iconType) where T : GridTileIcon
		{
			if (_icons.ContainsKey(iconType))
			{
				return (T)_icons[iconType];
			}
			GridTileIcon asset = SingletonMonobehaviour<GridTileIconAssetCollection>.Instance.GetAsset(iconType);
			GridTileIcon gridTileIcon = _overlayManager.CreateOverlay<GridTileIcon>(base.gameObject, asset);
			gridTileIcon.HideInstant();
			_icons.Add(iconType, gridTileIcon);
			RefreshCurrentIcon();
			return (T)gridTileIcon;
		}

		public void RemoveIcon(GridTileIconType iconType)
		{
			if (_icons.TryGetValue(iconType, out GridTileIcon value))
			{
				_icons.Remove(iconType);
				value.Remove();
				RefreshCurrentIcon();
			}
		}

		public void HideIcon()
		{
			if (_currentIcon != null)
			{
				_currentIcon.Hide();
			}
		}

		public void ShowIcon()
		{
			if (CanShowIcon(_currentIcon))
			{
				_currentIcon.Show();
			}
		}

		public bool IsShowingIcon(GridTileIconType iconType)
		{
			return _icons.ContainsKey(iconType);
		}

		private void RefreshCurrentIcon()
		{
			GridTileIcon gridTileIcon = null;
			foreach (KeyValuePair<GridTileIconType, GridTileIcon> icon in _icons)
			{
				if (gridTileIcon == null || icon.Value.Priority > gridTileIcon.Priority)
				{
					if (gridTileIcon != null)
					{
						if (!CanShowIcon(gridTileIcon))
						{
							gridTileIcon.HideInstant();
						}
						else
						{
							gridTileIcon.Hide();
						}
					}
					gridTileIcon = icon.Value;
				}
				else if (!CanShowIcon(icon.Value))
				{
					icon.Value.HideInstant();
				}
				else
				{
					icon.Value.Hide();
				}
			}
			_currentIcon = gridTileIcon;
			ShowIcon();
		}

		private void OnInteractionEnabledChanged(bool interactionEnabled)
		{
			if (interactionEnabled)
			{
				ShowIcon();
			}
			else
			{
				HideIcon();
			}
		}

		private void OnTilesHiddenChanged(bool tilesHidden)
		{
			if (tilesHidden && _currentIcon != null && !_currentIcon.ShowWhileTilesHidden)
			{
				HideIcon();
			}
			else
			{
				ShowIcon();
			}
		}

		private bool CanShowIcon(GridTileIcon icon)
		{
			if (icon != null && !_overlayManager.InteractionDisabled)
			{
				if (!icon.ShowWhileTilesHidden)
				{
					return !Builder.TilesHidden;
				}
				return true;
			}
			return false;
		}
	}
}
