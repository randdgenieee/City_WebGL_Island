using UnityEngine;

namespace CIG
{
	public interface ILogHandler
	{
		void HandleLog(string logString, string stackTrace, bool isCrash, LogType type);
	}
}
