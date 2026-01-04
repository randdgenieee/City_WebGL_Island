using CIG;
using System;

public class ParticlesAssetCollection : DictionaryAssetCollection<ParticlesAssetCollection.Particles, ParticleType, SpawnedParticles, ParticlesAssetCollection>
{
	[Serializable]
	public class Particles : SerializableDictionary
	{
	}

	public SpawnedParticles GetCurrencyRewardParticles(Currency currency)
	{
		switch (currency.ToCurrencyType())
		{
		case CurrencyType.Cash:
			return GetAsset(ParticleType.UICashReward);
		case CurrencyType.Gold:
			return GetAsset(ParticleType.UIGoldReward);
		case CurrencyType.XP:
			return GetAsset(ParticleType.UIXpReward);
		case CurrencyType.SilverKey:
			return GetAsset(ParticleType.UISilverKeyReward);
		case CurrencyType.GoldKey:
			return GetAsset(ParticleType.UIGoldKeyReward);
		case CurrencyType.Token:
			return GetAsset(ParticleType.UITokenReward);
		default:
			return null;
		}
	}
}
