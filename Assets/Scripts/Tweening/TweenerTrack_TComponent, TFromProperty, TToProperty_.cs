using UnityEngine;

namespace Tweening
{
	public abstract class TweenerTrack<TComponent, TFromProperty, TToProperty> : TweenerTrackBase where TComponent : Component
	{
		[SerializeField]
		protected TComponent _component;

		[SerializeField]
		protected bool _dynamicFrom;

		[SerializeField]
		protected TFromProperty _from;

		[SerializeField]
		protected TToProperty _to;

		public sealed override void ResetComponentValue()
		{
			if (_dynamicFrom)
			{
				ResetDynamicFromValue();
			}
			else
			{
				UpdateComponentValue(base.Curve.Evaluate(0f));
			}
		}

		public void SetTarget(TToProperty target)
		{
			_to = target;
		}

		protected abstract void ResetDynamicFromValue();
	}
	public abstract class TweenerTrack<TComponent, TProperty> : TweenerTrack<TComponent, TProperty, TProperty> where TComponent : Component
	{
	}
}
