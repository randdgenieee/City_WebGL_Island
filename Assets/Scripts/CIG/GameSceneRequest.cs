using System.Collections;

namespace CIG
{
	public class GameSceneRequest : SceneRequest
	{
		public override string SceneName => "Game";

		public override float LoadingWeight => 0.4f;

		public Model Model
		{
			get;
		}

		public GameSceneRequest(Model model)
		{
			Model = model;
		}

		public override IEnumerator LoadDuringSceneSwitch()
		{
			yield return null;
			base.Progress = 1f;
			base.HasCompleted = true;
		}
	}
}
