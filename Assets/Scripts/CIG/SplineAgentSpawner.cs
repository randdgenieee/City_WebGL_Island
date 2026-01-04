using Splines;
using UnityEngine;

namespace CIG
{
	public sealed class SplineAgentSpawner : SingletonMonobehaviour<SplineAgentSpawner>
	{
		public SplineAgent SpawnSplineAgent(SplineAgentType type, BezierSpline spline, float moveDuration, IsometricGrid grid, Timing timing)
		{
			SplineAgent splineAgent = Object.Instantiate(SingletonMonobehaviour<SplineAgentsAssetCollection>.Instance.GetAsset(type), grid.transform);
			splineAgent.Initialize(grid, timing, spline, moveDuration);
			return splineAgent;
		}
	}
}
