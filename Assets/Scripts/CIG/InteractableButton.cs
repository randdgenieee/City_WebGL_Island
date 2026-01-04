using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Interactable Button", 30)]
	public class InteractableButton : Button
	{
		public delegate void InteractableChangedEventHandler(bool interactable);

		private bool _wasInteractable;

		public event InteractableChangedEventHandler InteractableChangedEvent;

		private void FireInteractableChangedEvent(bool interactable)
		{
			if (this.InteractableChangedEvent != null)
			{
				this.InteractableChangedEvent(interactable);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_wasInteractable = base.interactable;
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			if (_wasInteractable != base.interactable)
			{
				FireInteractableChangedEvent(base.interactable);
				_wasInteractable = base.interactable;
			}
		}
	}
}
