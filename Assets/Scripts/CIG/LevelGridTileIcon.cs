using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class LevelGridTileIcon : GridTileIcon
	{
		[SerializeField]
		private LocalizedText _levelLabel;

		public override bool ShowWhileTilesHidden => true;

		public void SetLevel(int level)
		{
			_levelLabel.LocalizedString = Localization.Integer(level);
		}
	}
}
