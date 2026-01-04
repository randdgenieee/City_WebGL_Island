using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
    public class BuildingImage : MonoBehaviour
    {
        public enum EffectType
        {
            Gold,
            Boost,
            None
        }

        private const float DefaultMaxImageWidth = 200f;

        [SerializeField]
        private Image _mainImage;

        [SerializeField]
        private AnimatedSpriteImage _animation;

        [SerializeField]
        private bool _allowUpscale;

        [Header("Optional")]
        [SerializeField]
        private Image _goldIconObject;

        [SerializeField]
        private Image _boostIconObject;

        [SerializeField]
        [Tooltip("The object to scale based on the given max width. If empty, `_mainImage` will be scaled instead.")]
        private RectTransform _buildingRoot;

        [SerializeField]
        private Image _surfaceTypeFrame;

        [SerializeField]
        private Image _surfaceTypeBackground;

        [SerializeField]
        private Image _surfaceTypeIcon;

        [SerializeField]
        private Image _surfaceTypeFloor;

        [SerializeField]
        private Image _backgroundOutline;

        [SerializeField]
        private Image _frameOutline;

        [SerializeField]
        private GameObject _rays;

        public Image MainImage => _mainImage;

        public void Initialize(BuildingProperties buildingProperties, MaterialType materialType = MaterialType.UIClip, float maxImageWidth = 200f)
        {
            GridTile asset = SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(buildingProperties.BaseKey);
            EffectType effectType = (buildingProperties is LandmarkBuildingProperties) ? EffectType.Boost : ((!buildingProperties.IsGoldBuilding) ? EffectType.None : EffectType.Gold);
            _mainImage.sprite = asset.SpriteRenderer.sprite;
            bool flag = asset.ChildSpriteRenderers.Count > 0;
            _animation.gameObject.SetActive(flag);
            if (flag)
            {
                ChildSpriteRenderer childSpriteRenderer = asset.ChildSpriteRenderers[0];
                AnimatedSprite animatedSprite = childSpriteRenderer.AnimatedSprite;
                if (animatedSprite != null)
                {
                    _animation.AnimationMode = animatedSprite.AnimationMode;
                    _animation.FPS = animatedSprite.FPS;
                    _animation.WaitAtEndSeconds = animatedSprite.WaitAtEndSeconds;
                    _animation.ReplaceSprites(animatedSprite.Sprites);
                    if (!_animation.IsPlaying)
                    {
                        _animation.Play();
                    }
                }
                else
                {
                    _animation.ReplaceSprites(new Sprite[1]
                    {
                        childSpriteRenderer.SpriteRenderer.sprite
                    });
                    _animation.Reset();
                }
            }
            SurfaceSpriteAssetCollection.SurfaceSprites asset2 = SingletonMonobehaviour<SurfaceSpriteAssetCollection>.Instance.GetAsset(buildingProperties.SurfaceType);
            if (_surfaceTypeFrame != null)
            {
                _surfaceTypeFrame.sprite = ((effectType == EffectType.Boost) ? SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.LandmarkBuildingFrame) : asset2.Frame);
            }
            if (_surfaceTypeBackground != null)
            {
                _surfaceTypeBackground.sprite = ((effectType == EffectType.Boost) ? SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.LandmarkBuildingBackground) : asset2.Background);
            }
            if (_surfaceTypeIcon != null)
            {
                _surfaceTypeIcon.sprite = asset2.Icon;
                _surfaceTypeIcon.gameObject.SetActive(asset2.Icon != null);
            }
            if (_surfaceTypeFloor != null)
            {
                _surfaceTypeFloor.sprite = asset2.Floor;
                _surfaceTypeFloor.gameObject.SetActive(value: true);
            }
            Initialize(materialType, maxImageWidth, effectType);
        }

        public void Initialize(Sprite sprite, Sprite backgroundSprite, Sprite frameSprite, MaterialType materialType = MaterialType.UIClip, float maxImageWidth = 200f)
        {
            _mainImage.sprite = sprite;
            _animation.gameObject.SetActive(value: false);
            if (backgroundSprite != null && _surfaceTypeBackground != null)
            {
                _surfaceTypeBackground.sprite = backgroundSprite;
            }
            if (frameSprite != null && _surfaceTypeFrame != null)
            {
                _surfaceTypeFrame.sprite = frameSprite;
            }
            if (_surfaceTypeIcon != null)
            {
                _surfaceTypeIcon.gameObject.SetActive(value: false);
            }
            if (_surfaceTypeFloor != null)
            {
                _surfaceTypeFloor.gameObject.SetActive(value: false);
            }
            Initialize(materialType, maxImageWidth);
        }

        public void Initialize(MaterialType materialType, float maxImageWidth = 200f, EffectType effectType = EffectType.None)
        {
            float num = _allowUpscale ? maxImageWidth : Mathf.Min(_mainImage.sprite.rect.width, maxImageWidth);
            if (_buildingRoot == null)
            {
                _mainImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
            }
            else
            {
                _buildingRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
            }
            if (_backgroundOutline != null)
            {
                switch (effectType)
                {
                    case EffectType.Gold:
                        _backgroundOutline.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.GoldBackgroundOutline);
                        break;
                    case EffectType.Boost:
                        _backgroundOutline.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.LandmarkBackgroundOutline);
                        break;
                }
                _backgroundOutline.gameObject.SetActive(effectType != EffectType.None);
            }
            if (_frameOutline != null)
            {
                switch (effectType)
                {
                    case EffectType.Gold:
                        _frameOutline.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.GoldFrameOutline);
                        break;
                    case EffectType.Boost:
                        _frameOutline.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.LandmarkFrameOutline);
                        break;
                }
                _frameOutline.gameObject.SetActive(effectType != EffectType.None);
            }
            if (_rays != null)
            {
                _rays.SetActive(effectType == EffectType.Boost);
            }
            Material asset = SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(materialType);
            _mainImage.material = asset;
            _animation.Image.material = asset;
            if (_goldIconObject != null)
            {
                _goldIconObject.material = asset;
                _goldIconObject.gameObject.SetActive(effectType == EffectType.Gold);
            }
            if (_boostIconObject != null)
            {
                _boostIconObject.material = asset;
                _boostIconObject.gameObject.SetActive(effectType == EffectType.Boost);
            }
            if (_surfaceTypeFrame != null)
            {
                _surfaceTypeFrame.material = asset;
            }
            if (_surfaceTypeBackground != null)
            {
                _surfaceTypeBackground.material = asset;
            }
            if (_surfaceTypeIcon != null)
            {
                _surfaceTypeIcon.material = asset;
            }
            if (_surfaceTypeFloor != null)
            {
                _surfaceTypeFloor.material = asset;
                _surfaceTypeFloor.rectTransform.anchoredPosition3D = -Vector3.up * (num / 20f);
            }
            if (_frameOutline != null)
            {
                _frameOutline.material = asset;
            }
        }
    }
}
