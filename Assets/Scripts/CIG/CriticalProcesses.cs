using System.Collections.Generic;

namespace CIG
{
	public class CriticalProcesses
	{
		public delegate void NoMoreCriticalProcessesEventHandler();

		private readonly List<object> _criticalProcesses = new List<object>();

		public bool HasCriticalProcess => _criticalProcesses.Count > 0;

		public event NoMoreCriticalProcessesEventHandler NoMoreCriticalProcessesEvent;

		private void FireNoMoreCriticalProcessesEvent()
		{
			if (this.NoMoreCriticalProcessesEvent != null)
			{
				this.NoMoreCriticalProcessesEvent();
			}
		}

		public void RegisterCriticalProcess(object process)
		{
			_criticalProcesses.Add(process);
		}

		public void UnregisterCriticalProcess(object process)
		{
			_criticalProcesses.Remove(process);
			if (_criticalProcesses.Count == 0)
			{
				FireNoMoreCriticalProcessesEvent();
			}
		}
	}
}
