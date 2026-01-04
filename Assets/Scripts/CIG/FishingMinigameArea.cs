using UnityEngine;

namespace CIG
{
	public class FishingMinigameArea
	{
		public delegate void AreaMovedEventHandler();

		private readonly float _positionPadding;

		private readonly float _scaleMultiplier;

		public float GreenAreaSizePercentage
		{
			get;
			private set;
		}

		public float GreenAreaStartPercentage
		{
			get;
			private set;
		}

		public float GreenAreaEndPercentage
		{
			get;
			private set;
		}

		public float YellowAreaSizePercentage
		{
			get;
			private set;
		}

		public float YellowAreaStartPercentage
		{
			get;
			private set;
		}

		public float YellowAreaEndPercentage
		{
			get;
			private set;
		}

		public event AreaMovedEventHandler AreaMovedEvent;

		private void FireAreaMovedEvent()
		{
			this.AreaMovedEvent?.Invoke();
		}

		public FishingMinigameArea(float greenAreaStartSizePercentage, float yellowAreaStartSizePercentage, float positionPadding, float scaleMultiplier)
		{
			GreenAreaSizePercentage = greenAreaStartSizePercentage;
			YellowAreaSizePercentage = yellowAreaStartSizePercentage;
			_positionPadding = positionPadding;
			_scaleMultiplier = scaleMultiplier;
			MoveArea();
		}

		public int CatchFish(float markerPosition)
		{
			int result = 0;
			if (markerPosition >= GreenAreaStartPercentage && markerPosition <= GreenAreaEndPercentage)
			{
				result = ((!(markerPosition >= YellowAreaStartPercentage) || !(markerPosition <= YellowAreaEndPercentage)) ? 1 : 2);
			}
			return result;
		}

		public void MoveArea(int fishCaught)
		{
			if (fishCaught > 0)
			{
				GreenAreaSizePercentage *= _scaleMultiplier;
				YellowAreaSizePercentage *= _scaleMultiplier;
			}
			MoveArea();
		}

		private void MoveArea()
		{
			GreenAreaStartPercentage = Random.Range(_positionPadding, 100f - GreenAreaSizePercentage - _positionPadding);
			GreenAreaEndPercentage = GreenAreaStartPercentage + GreenAreaSizePercentage;
			YellowAreaStartPercentage = GreenAreaStartPercentage + GreenAreaSizePercentage / 2f - YellowAreaSizePercentage / 2f;
			YellowAreaEndPercentage = YellowAreaStartPercentage + YellowAreaSizePercentage;
			FireAreaMovedEvent();
		}
	}
}
