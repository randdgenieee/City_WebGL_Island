using System.Collections;

namespace CIG
{
	public abstract class SceneRequest
	{
		public abstract string SceneName
		{
			get;
		}

		public virtual string LoaderSceneName => "Loading";

		public abstract float LoadingWeight
		{
			get;
		}

		public float Progress
		{
			get;
			protected set;
		}

		public bool HasCompleted
		{
			get;
			protected set;
		}

		protected SceneRequest()
		{
			Progress = 0f;
			HasCompleted = false;
		}

		public abstract IEnumerator LoadDuringSceneSwitch();
	}
}
