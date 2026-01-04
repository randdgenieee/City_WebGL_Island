using CIG.Translation;
using UnityEngine;

public class InfoLine : MonoBehaviour
{
	[SerializeField]
	private GameObject _titleRoot;

	[SerializeField]
	private GameObject _infoAmountRoot;

	[SerializeField]
	private GameObject _infoBonusAmountRoot;

	[SerializeField]
	private GameObject _infoBoostAmountRoot;

	[SerializeField]
	private GameObject _iconRoot;

	[SerializeField]
	private GameObject _backgroundRoot;

	[SerializeField]
	[Space(8f)]
	private LocalizedText _infoAmount;

	[SerializeField]
	private LocalizedText _infoBonusAmount;

	[SerializeField]
	private LocalizedText _infoBoostAmount;

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
		_titleRoot.SetActive(active);
		_infoAmountRoot.SetActive(active);
		_iconRoot.SetActive(active);
		_backgroundRoot.SetActive(active);
		if (_infoBonusAmountRoot != null)
		{
			_infoBonusAmountRoot.SetActive(active);
		}
		if (_infoBoostAmountRoot != null)
		{
			_infoBoostAmountRoot.SetActive(active);
		}
	}

	public void SetValue(ILocalizedString text)
	{
		_infoAmount.LocalizedString = text;
	}

	public void SetBonus(ILocalizedString text)
	{
		if (_infoBonusAmount != null)
		{
			_infoBonusAmount.LocalizedString = text;
		}
	}

	public void SetBoost(ILocalizedString text)
	{
		if (_infoBoostAmount != null)
		{
			_infoBoostAmount.LocalizedString = text;
		}
	}

	public void SetValueBonusAndBoost(ILocalizedString value, ILocalizedString bonus, ILocalizedString boost)
	{
		SetValue(value);
		SetBonus(bonus);
		SetBoost(boost);
	}
}
