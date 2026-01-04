using UnityEngine;

namespace CIG
{
	public abstract class InteractableButtonComponent : MonoBehaviour
	{
		[SerializeField]
		private InteractableButton _interactableButton;

		private void Awake()
		{
			_interactableButton.InteractableChangedEvent += OnInteractableChanged;
		}

		private void OnEnable()
		{
			OnInteractableChanged(_interactableButton.interactable);
		}

		private void OnDestroy()
		{
			if (_interactableButton != null)
			{
				_interactableButton.InteractableChangedEvent -= OnInteractableChanged;
			}
		}

		protected abstract void OnInteractable();

		protected abstract void OnNonInteractable();

		private void OnInteractableChanged(bool interactable)
		{
			if (interactable)
			{
				OnInteractable();
			}
			else
			{
				OnNonInteractable();
			}
		}
	}
}
