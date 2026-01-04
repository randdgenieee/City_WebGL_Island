using CIG.Translation;

namespace CIG
{
	public class SSPFriendsListContentView : SSPFriendsContentView
	{
		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.Friends;

		public override ILocalizedString HeaderText => Localization.Key("social_menu_friends");

		protected override void OnDestroy()
		{
			if (_friendsManager != null)
			{
				_friendsManager.FriendListChangedEvent -= base.OnFriendsListChanged;
			}
			base.OnDestroy();
		}

		protected override void Open()
		{
			_friendsManager.FriendListChangedEvent += base.OnFriendsListChanged;
			base.Open();
		}

		protected override void Close()
		{
			_friendsManager.FriendListChangedEvent -= base.OnFriendsListChanged;
			base.Close();
		}

		protected override void GetFriendsList()
		{
			_friendsManager.GetFriendsList();
		}
	}
}
