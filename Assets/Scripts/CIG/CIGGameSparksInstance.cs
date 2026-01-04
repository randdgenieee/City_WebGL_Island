using GameSparks.Core;
using System;

namespace CIG
{
	public class CIGGameSparksInstance : GSInstance
	{
		public CIGGameSparksInstance()
			: base("CIG GameSparks Instance")
		{
		}

		public void Release(Action callback)
		{
			Disconnect();
			ShutDown(callback);
		}
	}
}
