using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public abstract class HUDCityProgressBars : MonoBehaviour
	{
		[SerializeField]
		private HUDProgressBar _happinessProgressBar;

		[SerializeField]
		private HUDProgressBar _populationProgressBar;

		[SerializeField]
		private HUDProgressBar _jobsProgressBar;

		[SerializeField]
		private Image _happinessImage;

		[SerializeField]
		private Sprite _happinessLowSprite;

		[SerializeField]
		private Sprite _happinessMediumSprite;

		[SerializeField]
		private Sprite _happinessHighSprite;

		protected abstract int Happiness
		{
			get;
		}

		protected abstract int Population
		{
			get;
		}

		protected abstract int Housing
		{
			get;
		}

		protected abstract int Employees
		{
			get;
		}

		protected abstract int Jobs
		{
			get;
		}

		protected void UpdateHappiness()
		{
			int maxValue = GetMaxValue();
			float progress = Mathf.Min(1f, (maxValue == 0) ? 0f : ((float)Happiness / (float)maxValue));
			float num = (Housing == 0) ? 1f : Mathf.Max(0f, (float)Happiness / (float)Housing);
			_happinessProgressBar.UpdateProgress(progress, Localization.Percentage(num, 0));
			if (num < 0.75f)
			{
				_happinessImage.sprite = _happinessLowSprite;
			}
			else if (num > 1f)
			{
				_happinessImage.sprite = _happinessHighSprite;
			}
			else
			{
				_happinessImage.sprite = _happinessMediumSprite;
			}
		}

		protected void UpdatePopulation()
		{
			int maxValue = GetMaxValue();
			float progress = (maxValue == 0) ? 0f : ((float)Population / (float)maxValue);
			_populationProgressBar.UpdateProgress(progress, Localization.Concat(Localization.Integer(Population), Localization.Literal("/"), Localization.Integer(Housing)));
		}

		protected void UpdateJobs()
		{
			int maxValue = GetMaxValue();
			float progress = (maxValue == 0) ? 0f : ((float)Employees / (float)maxValue);
			_jobsProgressBar.UpdateProgress(progress, Localization.Concat(Localization.Integer(Employees), Localization.Literal("/"), Localization.Integer(Jobs)));
		}

		private int GetMaxValue()
		{
			return Mathf.Max(Housing, Jobs, Happiness, Population);
		}
	}
}
