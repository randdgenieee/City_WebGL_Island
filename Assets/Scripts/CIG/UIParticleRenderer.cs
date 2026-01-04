using System;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace CIG
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasRenderer))]
	[RequireComponent(typeof(ParticleSystem))]
	[AddComponentMenu("UI/Effects/UI Particle System")]
	public class UIParticleRenderer : MaskableGraphic
	{
		[SerializeField]
		private ParticleSystem _particleSystem;

		[SerializeField]
		private Sprite _particleSprite;

		private ParticleSystem.MainModule _mainModule;

		private ParticleSystem.Particle[] _particles;

		private UIVertex[] _quad = new UIVertex[4];

		private Material _currentMaterial;

		private Texture _currentTexture;

		private Sprite _currentSprite;

		private Vector4 _imageUV = Vector4.zero;

		public override Texture mainTexture => _currentTexture;

		protected override void Awake()
		{
			base.Awake();
			if (!Init())
			{
				base.enabled = false;
			}
		}

		private void LateUpdate()
		{
			_particleSystem.Simulate(Time.unscaledDeltaTime, withChildren: false, restart: false, fixedTimeStep: true);
			SetAllDirty();
			if (material != _currentMaterial || _particleSprite != _currentSprite)
			{
				Init();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			Vector2 zero3 = Vector2.zero;
			int particles = _particleSystem.GetParticles(_particles);
			for (int i = 0; i < particles; i++)
			{
				ParticleSystem.Particle particle = _particles[i];
				Vector2 vector = (_mainModule.simulationSpace == ParticleSystemSimulationSpace.Local) ? particle.position : base.transform.InverseTransformPoint(particle.position);
				float num = (0f - particle.rotation) * ((float)Math.PI / 180f);
				float f = num + (float)Math.PI / 2f;
				Color32 currentColor = particle.GetCurrentColor(_particleSystem);
				float num2 = particle.GetCurrentSize(_particleSystem) * 0.5f;
				if (_mainModule.scalingMode == ParticleSystemScalingMode.Shape)
				{
					vector /= base.canvas.scaleFactor;
				}
				Vector4 imageUV = _imageUV;
				zero.x = imageUV.x;
				zero.y = imageUV.y;
				_quad[0] = UIVertex.simpleVert;
				_quad[0].color = currentColor;
				_quad[0].uv0 = zero;
				zero.x = imageUV.x;
				zero.y = imageUV.w;
				_quad[1] = UIVertex.simpleVert;
				_quad[1].color = currentColor;
				_quad[1].uv0 = zero;
				zero.x = imageUV.z;
				zero.y = imageUV.w;
				_quad[2] = UIVertex.simpleVert;
				_quad[2].color = currentColor;
				_quad[2].uv0 = zero;
				zero.x = imageUV.z;
				zero.y = imageUV.y;
				_quad[3] = UIVertex.simpleVert;
				_quad[3].color = currentColor;
				_quad[3].uv0 = zero;
				if (num == 0f)
				{
					zero2.x = vector.x - num2;
					zero2.y = vector.y - num2;
					zero3.x = vector.x + num2;
					zero3.y = vector.y + num2;
					zero.x = zero2.x;
					zero.y = zero2.y;
					_quad[0].position = zero;
					zero.x = zero2.x;
					zero.y = zero3.y;
					_quad[1].position = zero;
					zero.x = zero3.x;
					zero.y = zero3.y;
					_quad[2].position = zero;
					zero.x = zero3.x;
					zero.y = zero2.y;
					_quad[3].position = zero;
				}
				else
				{
					Vector2 b = new Vector2(Mathf.Cos(num), Mathf.Sin(num)) * num2;
					Vector2 b2 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num2;
					_quad[0].position = vector - b - b2;
					_quad[1].position = vector - b + b2;
					_quad[2].position = vector + b + b2;
					_quad[3].position = vector + b - b2;
				}
				vh.AddUIVertexQuad(_quad);
			}
		}

		private bool Init()
		{
			if (_particleSystem == null)
			{
				return false;
			}
			_mainModule = _particleSystem.main;
			if (_mainModule.maxParticles > 14000)
			{
				_mainModule.maxParticles = 14000;
			}
			_mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = _particleSystem.textureSheetAnimation;
			if (textureSheetAnimation.enabled)
			{
				textureSheetAnimation.enabled = false;
				UnityEngine.Debug.LogWarningFormat("Texture Sheet Animation is not (yet) supported by the UIParticleRenderer.");
			}
			ParticleSystemRenderer component = _particleSystem.GetComponent<ParticleSystemRenderer>();
			if (component != null)
			{
				component.enabled = false;
				component.material = null;
				component.materials = new Material[0];
				component.sharedMaterial = null;
				component.sharedMaterials = new Material[0];
			}
			if (material == null)
			{
				Shader shader = Shader.Find("Mobile/Particles/Additive");
				if (shader != null)
				{
					material = new Material(shader);
				}
			}
			_currentMaterial = material;
			_currentSprite = _particleSprite;
			if (_currentSprite != null)
			{
				_currentTexture = _currentSprite.texture;
				_imageUV = DataUtility.GetOuterUV(_currentSprite);
			}
			else
			{
				_imageUV = new Vector4(0f, 0f, 1f, 1f);
				if (_currentMaterial != null && _currentMaterial.HasProperty("_MainTex"))
				{
					_currentTexture = _currentMaterial.mainTexture;
				}
			}
			if (_currentTexture == null)
			{
				_currentTexture = Texture2D.whiteTexture;
			}
			if (_particles == null || _particles.Length != _mainModule.maxParticles)
			{
				_particles = new ParticleSystem.Particle[_mainModule.maxParticles];
			}
			return true;
		}
	}
}
