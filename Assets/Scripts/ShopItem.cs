using CIG.Translation;
using System;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
	[SerializeField]
	protected LocalizedText _priceLabel;

	private Action _onClick;

	protected void Initialize(Action onClick, ILocalizedString price)
	{
		_onClick = onClick;
		_priceLabel.LocalizedString = price;
	}

	public void OnItemClicked()
	{
		_onClick?.Invoke();
	}

	public virtual void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}
}
