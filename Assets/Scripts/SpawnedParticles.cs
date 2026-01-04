using UnityEngine;

public class SpawnedParticles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] _particleSystems;

	public virtual void Play()
	{
		int i = 0;
		for (int num = _particleSystems.Length; i < num; i++)
		{
			_particleSystems[i].Play();
		}
		UnityEngine.Object.Destroy(base.gameObject, GetLongestDuration());
	}

	private float GetLongestDuration()
	{
		float num = float.MinValue;
		int i = 0;
		for (int num2 = _particleSystems.Length; i < num2; i++)
		{
			ParticleSystem.MainModule main = _particleSystems[i].main;
			num = Mathf.Max(num, main.duration + main.startLifetime.constantMax);
		}
		return num;
	}
}
