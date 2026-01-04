using System;
using UnityEngine;

namespace CIG
{
	public class FlyingQuestObject : FlyingObject
	{
		[SerializeField]
		private CompositeSpriteImage _compositeSpriteImage;

		public void Initialize(QuestSpriteType questSpriteType, Vector3 start, Vector3 target, Action<FlyingQuestObject> onComplete)
		{
			_compositeSpriteImage.Initialize(SingletonMonobehaviour<QuestSpriteAssetCollection>.Instance.GetAsset(questSpriteType).LargeSpriteData);
			PlayAnimation(start, target, delegate
			{
				EventTools.Fire(onComplete, this);
			});
		}
	}
}
