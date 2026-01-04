using UnityEngine;

namespace CIG
{
	public class CurrencyAnimationSource : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Raw Position uses transform.position and directly uses it to set the flying currency's position.\r\nUse this when the CurrencyAnimationSource is seen by the same camera as the CurrencyAnimator\r\n\r\nScreen Position uses transform.position, converts it through the UI Camera to a World Position which is used to set the flying currency's position.\r\nUse this when the CurrencyAnimationSource's position is originating from a click/drag/pointer event's screen position.\r\n\r\nWorld Position uses transform.position, converts it through the World Camera to a Screen Position to then convert it through the UI Camera to a World Position which is used to set the flying currency's position.\r\nUse this when the CurrencyAnimationSource is seen by the World Camera (e.g. a building).\r\n")]
		private CurrencyAnimPositionType _positionType;

		public CurrencyAnimPositionType PositionType => _positionType;

		public Vector3 Position => base.transform.position;

		public object AnimationSource
		{
			get;
			private set;
		}

		public void Initialize(object animationSource)
		{
			AnimationSource = animationSource;
			SingletonMonobehaviour<CurrencyAnimator>.Instance.RegisterCurrencySource(this);
		}

		public void Deinitialize()
		{
			AnimationSource = null;
			if (SingletonMonobehaviour<CurrencyAnimator>.IsAvailable)
			{
				SingletonMonobehaviour<CurrencyAnimator>.Instance.UnregisterCurrencySource(this);
			}
		}

		private void OnDestroy()
		{
			Deinitialize();
		}
	}
}
