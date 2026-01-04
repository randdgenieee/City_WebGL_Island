using UnityEngine;

namespace CIG
{
	public class CIGIslandState : MonoBehaviour
	{
		public delegate void ValueChangedHandler(string key, object oldValue, object newValue);

		public const string HappinessKey = "Happiness";

		public const string PopulationKey = "Population";

		public const string HousingKey = "Housing";

		public const string EmployeesKey = "Employees";

		public const string JobsKey = "Jobs";

		private StorageDictionary _storage;

		private GameState _gameState;

		private int _baseHappiness;

		private int _happiness;

		private int _population;

		private int _housing;

		private int _employees;

		private int _jobs;

		private bool _propagate;

		public int Happiness
		{
			get
			{
				return _happiness;
			}
			private set
			{
				int happiness = _happiness;
				_happiness = value;
				_storage.Set("Happiness", _happiness);
				OnValueChanged("Happiness", happiness, _happiness);
			}
		}

		public int AvailableHappiness => _gameState.GlobalHappiness;

		public int Population
		{
			get
			{
				return _population;
			}
			private set
			{
				int population = _population;
				_population = value;
				_storage.Set("Population", _population);
				OnValueChanged("Population", population, _population);
			}
		}

		public int AvailablePopulation => _gameState.GlobalPopulation;

		public int Housing
		{
			get
			{
				return _housing;
			}
			private set
			{
				int housing = _housing;
				_housing = value;
				_storage.Set("Housing", _housing);
				OnValueChanged("Housing", housing, _housing);
			}
		}

		public int Employees
		{
			get
			{
				return _employees;
			}
			private set
			{
				int employees = _employees;
				_employees = value;
				_storage.Set("Employees", _employees);
				OnValueChanged("Employees", employees, _employees);
			}
		}

		public int Jobs
		{
			get
			{
				return _jobs;
			}
			private set
			{
				int jobs = _jobs;
				_jobs = value;
				_storage.Set("Jobs", _jobs);
				OnValueChanged("Jobs", jobs, _jobs);
			}
		}

		public event ValueChangedHandler ValueChanged;

		private void FireValueChanged(string key, object oldValue, object newValue)
		{
			this.ValueChanged?.Invoke(key, oldValue, newValue);
		}

		public void Initialize(StorageDictionary storage, GameState gameState, int baseHappiness, bool readOnly)
		{
			_storage = storage;
			_gameState = gameState;
			_baseHappiness = baseHappiness;
			_propagate = !readOnly;
			if (_storage.Contains("Happiness"))
			{
				_happiness = _storage.Get("Happiness", 0);
			}
			else
			{
				Happiness = 0;
				AddHappiness(_baseHappiness);
			}
			_population = _storage.Get("Population", 0);
			_housing = _storage.Get("Housing", 0);
			_employees = _storage.Get("Employees", 0);
			_jobs = _storage.Get("Jobs", 0);
		}

		public void AddPopulation(int pop)
		{
			Population += pop;
			if (_propagate)
			{
				_gameState.AddGlobalPopulation(pop);
			}
		}

		public void AddHousing(int room)
		{
			Housing += room;
			if (_propagate)
			{
				_gameState.AddGlobalHousing(room);
			}
		}

		public void AddHappiness(int smiles)
		{
			Happiness += smiles;
			if (_propagate)
			{
				_gameState.AddGlobalHappiness(smiles);
			}
		}

		public void AddEmployees(int emp)
		{
			Employees += emp;
			if (_propagate)
			{
				_gameState.AddGlobalEmployees(emp);
			}
		}

		public void AddJobs(int jobs)
		{
			Jobs += jobs;
			if (_propagate)
			{
				_gameState.AddGlobalJobs(jobs);
			}
		}

		public void LoadValuesFromBuildings()
		{
			int num = 0;
			int num2 = 0;
			CIGCommercialBuilding[] array = UnityEngine.Object.FindObjectsOfType<CIGCommercialBuilding>();
			int i = 0;
			for (int num3 = array.Length; i < num3; i++)
			{
				CIGCommercialBuilding cIGCommercialBuilding = array[i];
				num += cIGCommercialBuilding.Employees;
				num2 += cIGCommercialBuilding.MaxEmployees;
			}
			if (num != Employees || num2 != Jobs)
			{
				UnityEngine.Debug.LogWarningFormat("Employees gamestate values changed from {2}/{3} to {0}/{1}.", num, num2, Employees, Jobs);
				if (_propagate)
				{
					_gameState.AddGlobalJobs(num2 - Jobs);
				}
				Jobs = num2;
				if (_propagate)
				{
					_gameState.AddGlobalEmployees(num - Employees);
				}
				Employees = num;
			}
			int num4 = 0;
			int num5 = 0;
			CIGResidentialBuilding[] array2 = UnityEngine.Object.FindObjectsOfType<CIGResidentialBuilding>();
			int j = 0;
			for (int num6 = array2.Length; j < num6; j++)
			{
				CIGResidentialBuilding cIGResidentialBuilding = array2[j];
				num4 += cIGResidentialBuilding.People;
				num5 += cIGResidentialBuilding.MaxPeople;
			}
			if (num4 != Population || num5 != Housing)
			{
				UnityEngine.Debug.LogWarningFormat("Population gamestate values changed from {2}/{3} to {0}/{1}.", num4, num5, Population, Housing);
				if (_propagate)
				{
					_gameState.AddGlobalHousing(num5 - Housing);
				}
				Housing = num5;
				if (_propagate)
				{
					_gameState.AddGlobalPopulation(num4 - Population);
				}
				Population = num4;
			}
			int num7 = _baseHappiness;
			CIGBuilding[] array3 = UnityEngine.Object.FindObjectsOfType<CIGBuilding>();
			int k = 0;
			for (int num8 = array3.Length; k < num8; k++)
			{
				num7 += array3[k].Happiness;
			}
			if (num7 != Happiness)
			{
				UnityEngine.Debug.LogWarningFormat("Happiness gamestate value changed from {1} to {0}.", num7, Happiness);
				if (_propagate)
				{
					_gameState.AddGlobalHappiness(num7 - Happiness);
				}
				Happiness = num7;
			}
		}

		private void OnValueChanged(string key, object oldValue, object newValue)
		{
			if (oldValue != newValue)
			{
				FireValueChanged(key, oldValue, newValue);
			}
		}
	}
}
