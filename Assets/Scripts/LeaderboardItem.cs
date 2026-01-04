using CIG;
using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
	[Serializable]
	private struct Style
	{
		[SerializeField]
		private Color _entryNumberBackground;

		[SerializeField]
		private Color _entryNumberOutline;

		[SerializeField]
		private Color _leftBarBackground;

		[SerializeField]
		private Color _rightBarBackground;

		public Color EntryNumberBackground => _entryNumberBackground;

		public Color EntryNumberOutline => _entryNumberOutline;

		public Color LeftBarBackground => _leftBarBackground;

		public Color RightBarBackground => _rightBarBackground;
	}

	[SerializeField]
	private LocalizedText _entryLabel;

	[SerializeField]
	private Outline _entryOutline;

	[SerializeField]
	private Shadow _entryShadow;

	[SerializeField]
	private LocalizedText _islandScoreLabel;

	[SerializeField]
	private LocalizedText _levelLabel;

	[SerializeField]
	private LocalizedText _usernameLabel;

	[SerializeField]
	private LocalizedText _citizenLabel;

	[SerializeField]
	private LocalizedText _likeCountLabel;

	[SerializeField]
	private Button _visitButton;

	[SerializeField]
	private Image _entryNumberBackground;

	[SerializeField]
	private Image _leftBarBackground;

	[SerializeField]
	private Image _rightBarBackground;

	[SerializeField]
	private Style _regularStyle;

	[SerializeField]
	private Style _meStyle;

	private LeaderboardEntry _record;

	private PopupManager _popupManager;

	private IslandsManager _islandsManager;

	public void Initialize(LeaderboardEntry record, PopupManager popupManager, IslandsManager islandsManager, LikeRegistrar likeRegistrar, bool isUser)
	{
		_record = record;
		_popupManager = popupManager;
		_islandsManager = islandsManager;
		_usernameLabel.LocalizedString = Localization.Literal(record.DisplayName);
		_islandScoreLabel.LocalizedString = Localization.Integer(record.Score);
		_levelLabel.LocalizedString = Localization.Integer(record.Level);
		_citizenLabel.LocalizedString = Localization.Integer(record.Population);
		_entryLabel.LocalizedString = Localization.Concat(Localization.Integer(record.Rank + 1), Localization.Literal("."));
		_likeCountLabel.LocalizedString = Localization.Integer(likeRegistrar.GetLikesForUser(record.UserId));
		ApplyStyle(isUser ? _meStyle : _regularStyle);
		_visitButton.gameObject.SetActive(record.CanVisit);
	}

	public void OnVisitButtonClicked()
	{
		_islandsManager.StartVisiting(_record.UserId);
		_popupManager.CloseAllOpenPopups(instant: false);
	}

	private void ApplyStyle(Style style)
	{
		_entryNumberBackground.color = style.EntryNumberBackground;
		_entryOutline.effectColor = style.EntryNumberOutline;
		_entryShadow.effectColor = style.EntryNumberOutline;
		_leftBarBackground.color = style.LeftBarBackground;
		_rightBarBackground.color = style.RightBarBackground;
	}
}
