namespace CIG
{
	public class HUDVisitingCityProgressBars : HUDCityProgressBars
	{
		private int _happiness;

		private int _population;

		private int _housing;

		private int _employees;

		private int _jobs;

		protected override int Happiness => _happiness;

		protected override int Population => _population;

		protected override int Housing => _housing;

		protected override int Employees => _employees;

		protected override int Jobs => _jobs;

		public void UpdateValues(int happiness, int population, int housing, int employees, int jobs)
		{
			_happiness = happiness;
			_population = population;
			_housing = housing;
			_employees = employees;
			_jobs = jobs;
			UpdateHappiness();
			UpdatePopulation();
			UpdateJobs();
		}
	}
}
