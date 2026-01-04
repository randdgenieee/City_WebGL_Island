using CIG;
using CIG.Translation;
using System.Collections;
using Tweening;
using UnityEngine;

public class WelcomeView : MonoBehaviour
{
	private const float AnimationDuration = 5.5f;

	private const float CrossSellingHUDDelay = 1f;

	[SerializeField]
	private Animator[] _introAnimators;

	[SerializeField]
	private GameObject _hudRoot;

	[SerializeField]
	private Tweener _playButtonTweener;

	[SerializeField]
	private Tweener _infoBoxTweener;

	[SerializeField]
	private LocalizedText _playerIdLabel;

	[SerializeField]
	private LocalizedText _versionLabel;

	[SerializeField]
	private CrossSellingController _crossSellingController;

	private Model _model;

	private SceneLoader _sceneLoader;

	private bool _hasClickedPlay;

	private bool _skipAnimation;

	public void Initialize(Model model, SceneLoader sceneLoader)
	{
		_model = model;
		_sceneLoader = sceneLoader;
		ScreenView.PushScreenView("welcome_scene");
		FPSCounter.Create();
		InitInfoBox();
		StartCoroutine(IntroAnimationRoutine());
	}

	private void OnDestroy()
	{
		ScreenView.PopScreenView("welcome_scene");
	}

	public void OnPlayClick()
	{
		if (!_hasClickedPlay)
		{
			_hasClickedPlay = true;
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
			_sceneLoader.LoadScene(new GameSceneRequest(_model));
		}
	}

	public void OnTermsOfServiceClicked()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
		Application.OpenURL("https://www.sparklingsociety.net/terms-of-service/");
	}

	public void OnPrivacyPolicyClicked()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
		Application.OpenURL("http://www.sparklingsociety.net/privacy-policy/");
	}

	public void OnSkipAnimationClicked()
	{
		_skipAnimation = true;
		int i = 0;
		for (int num = _introAnimators.Length; i < num; i++)
		{
			_introAnimators[i].SetTrigger("Skip");
		}
	}

	private void InitInfoBox()
	{
		_playerIdLabel.LocalizedString = (string.IsNullOrEmpty(_model.Device.User.UserKey) ? Localization.EmptyLocalizedString : Localization.Format(Localization.Key("loading_screen.social_your_player_id"), Localization.Literal(_model.Device.User.UserKey)));
		_versionLabel.LocalizedString = Localization.Format(Localization.Key("your_game_version"), Localization.Literal(NativeGameVersion.Version));
	}

	private IEnumerator IntroAnimationRoutine()
	{
		_crossSellingController.gameObject.SetActive(value: false);
		_hudRoot.SetActive(value: false);
		float animationEndTime = Time.time + 5.5f;
		while (Time.time < animationEndTime && !_skipAnimation)
		{
			yield return null;
		}
		_hudRoot.SetActive(value: true);
		_playButtonTweener.Play();
		_infoBoxTweener.Play();
		yield return new WaitForSeconds(1f);
		_crossSellingController.gameObject.SetActive(value: true);
		_crossSellingController.Initialize(_model.GameServer.CrossPromo);
	}
}
