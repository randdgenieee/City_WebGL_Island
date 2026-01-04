using UnityEngine;

namespace Tweening
{
    public sealed class ParticleSystemEmmisionRateOverTimeTweenerTrack : TweenerTrack<ParticleSystem, float>
    {
        public override void UpdateComponentValue(float evaluatedTime)
        {
            SetRateOverTime(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
        }

        protected override void ResetDynamicFromValue()
        {
            _from = GetRateOverTime();
        }

        private void SetRateOverTime(float rate)
        {
            ParticleSystem.EmissionModule emission = _component.emission;
            emission.rateOverTimeMultiplier = rate;
        }

        private float GetRateOverTime()
        {
            return _component.emission.rateOverTimeMultiplier;
        }
    }
}
