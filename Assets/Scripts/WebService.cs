using CIG;
using CIG.Translation.Native;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public sealed class WebService
{
    public delegate void PullRequestCompletedEventHandler(Dictionary<string, string> properties);

    public struct WWWResponse
    {
        public bool TimedOut
        {
            get;
        }

        public string Error
        {
            get;
        }

        public string Text
        {
            get;
        }

        public WWWResponse(bool timedout, string error, string text)
        {
            TimedOut = timedout;
            Error = error;
            Text = text;
        }
    }

    public const string AndroidApiDomain = "https://gamesapi.sparklingsociety.net/v2/cig5/";

    public const string IOSApiDomain = "https://gamesapi.sparklingsociety.net/v2/cig5-ios/";

    public const string MetroApiDomain = "https://gamesapi.sparklingsociety.net/v2/cig5-windows/";

    public const string OSXApiDomain = "https://gamesapi.sparklingsociety.net/v2/cig5-osx/";

    public const string AmazonApiDomain = "https://gamesapi.sparklingsociety.net/v2/cig5-amazon/";

    public const int DefaultTimeoutSeconds = 5;

    private const string DefaultABGroupValue = "notset";

    private const int DefaultCheaterFlagValue = -1;

    private const string OverriddenPropertiesPrefix = "property.";

    private const string UseBadgeServerKey = "use_badge";

    public const string AdjustTokensServerKey = "adjust_tokens";

    private const string UserIdServerKey = "userId";

    private const string UserKeyServerKey = "userKey";

    private const string ABGroupKey = "abgroup";

    private const string CheaterFlagKey = "cheater_value";

    private const string IAPSignValidationEnabledKey = "iap_sign_validation_enabled";

    private const string WheelOfFortuneTokenRewardEnabledKey = "wheel_of_fortune_token_reward_enabled";

    public const string SaleServerKey = "saletype";

    public const string SaleSecondsServerKey = "saleseconds";

    public const string UpdateAvailableServerKey = "update_available";

    public const string ExpansionCashCostMultiplierServerKey = "expansionCashCostMultiplier";

    public const string CommerialCostCashMultiplierServerKey = "commercialBuildingCostCashMultiplier";

    public const string CommercialCostGoldMultiplierServerKey = "commercialBuildingCostGoldMultiplier";

    public const string CommunityCostCashMultiplierServerKey = "communityBuildingCostCashMultiplier";

    public const string CommunityCostGoldMultiplierServerKey = "communityBuildingCostGoldMultiplier";

    public const string ResidentialCostCashMultiplierServerKey = "residentialBuildingCostCashMultiplier";

    public const string ResidentialCostGoldMultiplierServerKey = "residentialBuildingCostGoldMultiplier";

    public const string DecorationCashCostMultiplierServerKey = "decorationCashCostMultiplier";

    public const string DecorationGoldCostMultiplierServerKey = "decorationGoldCostMultiplier";

    public const string ResidentialInstantGoldCostBuildingMultiplierServerKey = "residentialInstantGoldCostBuildingMultiplier";

    public const string CommercialInstantGoldCostBuildingMultiplierServerKey = "commercialInstantGoldCostBuildingMultiplier";

    public const string CommunityInstantGoldCostBuildingMultiplierServerKey = "communityInstantGoldCostBuildingMultiplier";

    public const string ResidentialInstantGoldCostUpgradeMultiplierServerKey = "residentialInstantGoldCostUpgradeMultiplier";

    public const string CommercialInstantGoldCostUpgradeMultiplierServerKey = "commercialInstantGoldCostUpgradeMultiplier";

    public const string CommunityInstantGoldCostUpgradeMultiplierServerKey = "communityInstantGoldCostUpgradeMultiplier";

    public const string ResidentialCashCostUpgradeMultiplierServerKey = "residentialCashCostUpgradeMultiplier";

    public const string CommercialCashCostUpgradeMultiplierServerKey = "commercialCashCostUpgradeMultiplier";

    public const string CommunityCashCostUpgradeMultiplierServerKey = "communityCashCostUpgradeMultiplier";

    public const string KeyDealGoldKeyCostMultiplierServerKey = "keyDealGoldKeyCostMultiplier";

    public const string KeyDealSilverKeyCostMultiplierServerKey = "keyDealSilverKeyCostMultiplier";

    public const string LandmarksGoldKeyCostMultiplierServerKey = "landmarksGoldKeyCostMultiplier";

    public const string LandmarksSilverKeyCostMultiplierServerKey = "landmarksSilverKeyCostMultiplier";

    public const string UpgradeGoldKeyRewardMultiplierServerKey = "upgradeGoldKeyRewardMultiplier";

    public const string UpgradeSilverKeyRewardMultiplierServerKey = "upgradeSilverKeyRewardMultiplier";

    public const string LevelUpCashRewardMultiplierServerKey = "levelUpCashRewardMultiplier";

    public const string UpspeedGoldCostMultiplierServerKey = "upspeedGoldCostMultiplier";

    public const string CommercialBuildingProfitMultiplierServerKey = "commercialBuildingProfitMultiplier";

    public const string XPMultiplierServerKey = "xpMultiplier";

    private static string ApiDomain;

    private readonly StorageDictionary _storage;

    private readonly Device _device;

    private readonly GameSparksServer _gameSparksServer;

    private Dictionary<string, string> _properties = new Dictionary<string, string>();

    private string _lastPullResponseError = "";

    private string _lastResponse = string.Empty;

    private const string UserIdKey = "UserId";

    private const string UseBadgeKey = "UseBadge";

    private const string LastResponseKey = "LastResponse";

    private const string MultipliersKey = "Multipliers";

    private const string OverriddenGamePropertiesKey = "OverriddenGameProperties";

    private const string IAPSignValidationEnabledStorageKey = "IAPSignValidationEnabled";

    private const string WheelOfFortuneTokenRewardEnabledStorageKey = "WheelOfFortuneTokenRewardEnabled";

    public static string Domain => ApiDomain;

    public static string APIURL => ApiDomain + "api.php";

    public static string UserPropsURL => ApiDomain + "userprops.php";


    public Dictionary<string, string> Properties => _properties;

    public string LastPullResponseError => _lastPullResponseError;

    public bool LastSyncSuccess
    {
        get;
        private set;
    }

    public long UserId
    {
        get;
        private set;
    }

    public Multipliers Multipliers
    {
        get;
    }

    public bool UseBadge
    {
        get;
        private set;
    }

    public string ABGroup
    {
        get;
        private set;
    }

    public int CheaterFlag
    {
        get;
        private set;
    }

    public Dictionary<string, string> OverriddenGameProperties
    {
        get;
        private set;
    }

    public bool IAPSignValidationEnabled
    {
        get;
        private set;
    }

    public bool WheelOfFortuneTokenRewardEnabled
    {
        get;
        private set;
    }

    public event PullRequestCompletedEventHandler PullRequestCompletedEvent;

    private void FirePullRequestCompletedEvent(Dictionary<string, string> properties)
    {
        this.PullRequestCompletedEvent?.Invoke(properties);
    }

    static WebService()
    {
        SetupDomains();
    }

    public WebService(StorageDictionary storage, Device device, GameSparksServer gameSparksServer)
    {
        _storage = storage;
        _device = device;
        _gameSparksServer = gameSparksServer;
        UserId = _storage.Get("UserId", -1L);
        UseBadge = _storage.Get("UseBadge", defaultValue: true);
        Multipliers = _storage.GetModel("Multipliers", (StorageDictionary sd) => new Multipliers(sd), new Multipliers());
        ABGroup = _storage.Get("abgroup", "notset");
        CheaterFlag = _storage.Get("cheater_value", -1);
        OverriddenGameProperties = _storage.GetDictionary<string>("OverriddenGameProperties");
        if (_storage.Contains("LastResponse"))
        {
            _lastResponse = _storage.Get("LastResponse", string.Empty);
            _properties = ParsePropertyFileFormat(_lastResponse);
        }
    }

    public IEnumerator SyncWithServer(int timeoutSeconds = 5)
    {
        WWWForm parameters = GenerateRequestParameters();
        yield return PullRequestRoutine(parameters, timeoutSeconds);
    }

    public IEnumerator SyncWithServerExtended(Game game, int timeoutSeconds = 5)
    {
        WWWForm parameters = GenerateExtendedRequestParameters(game.GameState, game.GameStats, game.IslandsManager);
        yield return PullRequestRoutine(parameters, timeoutSeconds);
    }

    public UnityWebRequest RedeemCode(string code)
    {
        WWWForm wWWForm = GenerateRequestParameters();
        wWWForm.AddField("action", "redeemcode_extended");
        wWWForm.AddField("code", code);
        return UnityWebRequest.Post(APIURL, wWWForm);
    }

    public UnityWebRequest PurchaseIntent(string item)
    {
        WWWForm wWWForm = GenerateRequestParameters();
        wWWForm.AddField("action", "purchase_intent");
        wWWForm.AddField("item", item);
        return UnityWebRequest.Post(APIURL, wWWForm);
    }

    public UnityWebRequest PricePoints()
    {
        WWWForm wWWForm = GenerateRequestParameters();
        wWWForm.AddField("action", "getpricepoints");
        return UnityWebRequest.Post(APIURL, wWWForm);
    }

    public UnityWebRequest ValidatePurchase(string productID, string receipt)
    {
        WWWForm wWWForm = GenerateRequestParameters();
        wWWForm.AddField("action", "purchase_receipt");
        wWWForm.AddField("sku", productID);
        wWWForm.AddField("receipt", receipt);
        return UnityWebRequest.Post(APIURL, wWWForm);
    }

    public UnityWebRequest RegisterNewsLetter(string email)
    {
        WWWForm wWWForm = GenerateRequestParameters();
        wWWForm.AddField("action", "register");
        wWWForm.AddField("email", email);
        return UnityWebRequest.Post(APIURL, wWWForm);
    }

    private static void SetupDomains()
    {
        ApiDomain = "https://gamesapi.sparklingsociety.net/v2/cig5/";
    }

    private IEnumerator PullRequestRoutine(WWWForm parameters, int timeoutSeconds)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(UserPropsURL, parameters))
        {
            request.timeout = timeoutSeconds;
            yield return request.SendWebRequest();
            if (string.IsNullOrEmpty(request.error))
            {
                HandlePullResponse(request.downloadHandler.text);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Server error: " + request.error);
                HandlePullResponseError(request.error);
            }
        }
    }

    private WWWForm GenerateRequestParameters()
    {
        WWWForm wWWForm = new WWWForm();
        return wWWForm;
    }

    private WWWForm GenerateExtendedRequestParameters(GameState gameState, GameStats gameStats, IslandsManager islandsManager)
    {
        WWWForm wWWForm = GenerateRequestParameters();
        wWWForm.AddField("brand", "Unity");
        wWWForm.AddField("device", SystemInfo.deviceName);
        wWWForm.AddField("manufacturer", SystemInfo.graphicsDeviceVendor);
        wWWForm.AddField("model", SystemInfo.deviceModel);
        wWWForm.AddField("product", SystemInfo.deviceType.ToString());
        wWWForm.AddField("xp", gameState.Balance.Round(RoundingMethod.Nearest).GetValue("XP").ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("level", gameState.Level);
        wWWForm.AddField("cash", gameState.Balance.Round(RoundingMethod.Nearest).GetValue("Cash").ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("gold", gameState.Balance.Round(RoundingMethod.Nearest).GetValue("Gold").ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("tokens", gameState.Balance.Round(RoundingMethod.Nearest).GetValue("Token").ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("cashspent", gameState.CurrenciesSpent.Round(RoundingMethod.Nearest).GetValue("Cash").ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("goldspent", gameState.CurrenciesSpent.Round(RoundingMethod.Nearest).GetValue("Gold").ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("minutesplayed", gameState.TotalMinutesPlayed.ToString());
        wWWForm.AddField("sessions", gameStats.NumberOfTimesPlayed.ToString());
        wWWForm.AddField("gold_CashBuildings", gameStats.GoldSpentCashBuildings.ToString());
        wWWForm.AddField("gold_CashExchange", gameStats.GoldSpentCashExchange.ToString());
        wWWForm.AddField("gold_CraneHire", gameStats.GoldSpentCraneHire.ToString());
        wWWForm.AddField("gold_CranePurchase", gameStats.GoldSpentCranePurchase.ToString());
        wWWForm.AddField("gold_Expansions", gameStats.GoldSpentExpansions.ToString());
        wWWForm.AddField("gold_GoldBuildings", gameStats.GoldSpentGoldBuildings.ToString());
        wWWForm.AddField("gold_Speedups", gameStats.GoldSpentSpeedups.ToString());
        wWWForm.AddField("gold_SpeedupsUpgrade", gameStats.GoldSpentUpgradeSpeedups.ToString());
        wWWForm.AddField("gold_SpeedupsBuild", gameStats.GoldSpentBuildSpeedups.ToString());
        wWWForm.AddField("gold_SpeedupsAirship", gameStats.GoldSpentAirshipSpeedups.ToString());
        wWWForm.AddField("gold_SpeedupsDemolish", gameStats.GoldSpentDemolishSpeedups.ToString());
        wWWForm.AddField("gold_ImmediateBuild", gameStats.GoldSpentImmediateBuild.ToString());
        wWWForm.AddField("gold_ImmediateUpgrades", gameStats.GoldSpentOnImmediateUpgrades.ToString());
        wWWForm.AddField("expansions_BoughtWithGold", gameStats.NumberOfExpansionsPurchasedWithGold);
        wWWForm.AddField("expansions_BoughtWithCash", gameStats.NumberOfExpansionsPurchasedWithCash);
        wWWForm.AddField("goldKeysSpent", gameStats.GoldKeysSpent.ToString());
        wWWForm.AddField("goldKeysReceived", gameStats.GoldKeysEarned.ToString());
        wWWForm.AddField("goldKeysReceived_Upgrades", gameStats.GoldKeysEarnedFromUpgrades.ToString());
        wWWForm.AddField("silverKeysSpent", gameStats.SilverKeysSpent.ToString());
        wWWForm.AddField("silverKeysReceived", gameStats.SilverKeysEarned.ToString());
        wWWForm.AddField("silverKeysReceived_Upgrades", gameStats.SilverKeysEarnedFromUpgrades.ToString());
        wWWForm.AddField("cashSpentCurrencyConversion", gameStats.CashSpentOnCurrencyConversion.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("silverKeysSpentCurrencyConversion", gameStats.SilverKeysSpentOnCurrencyConversion.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("goldKeysSpentCurrencyConversion", gameStats.GoldKeysSpentOnCurrencyConversion.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("silverKeysEarnedCurrencyConversion", gameStats.SilverKeysEarnedFromCurrencyConversion.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("goldKeysEarnedCurrencyConversion", gameStats.GoldKeysEarnedFromCurrencyConversion.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("goldEarnedCurrencyConversion", gameStats.GoldEarnedFromCurrencyConversion.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("keyDealsPurchased_NoLandmarks", gameStats.NumberOfKeyDealsWithoutLandmarksPurchased.ToString());
        wWWForm.AddField("videos_watched", gameStats.NumberOfVideosWatched.ToString());
        wWWForm.AddField("wooden_chests_opened", gameStats.NumberOfWoodenChestsOpened);
        wWWForm.AddField("silver_chests_opened", gameStats.NumberOfSilverChestsOpened);
        wWWForm.AddField("gold_chests_opened", gameStats.NumberOfGoldChestsOpened);
        wWWForm.AddField("platinum_chests_opened", gameStats.NumberOfPlatinumChestsOpened);
        wWWForm.AddField("silver_keys_spent_on_silver_chests", gameStats.NumberOfSilverKeysSpentOnSilverChests.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("gold_keys_spent_on_gold_chests", gameStats.NumberOfGoldKeysSpentOnGoldChests.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("gold_keys_spent_on_platinum_chests", gameStats.NumberOfGoldKeysSpentOnPlatinumChests.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("tokens_spent_on_wheel_of_fortune", gameStats.TokensSpentWheelOfFortune.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("gold_spent_on_wheel_of_fortune", gameStats.GoldSpentWheelOfFortune.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("gold_earned_from_wheel_of_fortune", gameStats.GoldEarnedWheelOfFortune.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("goldkeys_earned_from_wheel_of_fortune", gameStats.GoldKeysEarnedWheelOfFortune.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("currencies_earned_friend_island_visiting", gameStats.CurrenciesEarnedFriendIslandVisiting.ToString(CultureInfo.InvariantCulture));
        wWWForm.AddField("islands_unlocked", islandsManager.IslandsUnlocked);
        wWWForm.AddField("number_of_low_memory_warnings", _device.NumberOfLowMemoryWarnings);
        GameSparksAuthentication currentAuthentication = _gameSparksServer.Authenticator.CurrentAuthentication;
        if (currentAuthentication.IsAuthenticated)
        {
            wWWForm.AddField("gamesparks_displayname", currentAuthentication.DisplayName);
            wWWForm.AddField("gamesparks_userid", currentAuthentication.UserId);
        }
        return wWWForm;
    }

    private void HandlePullResponse(string response)
    {
        _properties = ParsePropertyFileFormat(response);
        UserId = _properties.Get("userId", -1L);
        if (_properties.TryGetValue("userKey", out string value))
        {
            _device.User.SetUserKey(value);
        }
        ExceptionReporter.SetEdwinUserId(UserId.ToString(), value);
        UseBadge = _properties.Get("use_badge", defaultValue: true);
        string text = _properties.Get("abgroup", "notset");
        if (ABGroup != text)
        {
            ABGroup = text;
            Analytics.SetABGroup(ABGroup);
        }
        int num = _properties.Get("cheater_value", -1);
        if (CheaterFlag != num)
        {
            CheaterFlag = num;
            Analytics.SetCheaterFlag(CheaterFlag);
        }
        IAPSignValidationEnabled = _properties.Get("iap_sign_validation_enabled", defaultValue: true);
        WheelOfFortuneTokenRewardEnabled = _properties.Get("wheel_of_fortune_token_reward_enabled", defaultValue: true);
        Multipliers.HandlePullResponse(_properties);
        OverriddenGameProperties = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> property in Properties)
        {
            if (property.Key.StartsWith("property."))
            {
                OverriddenGameProperties.Add(property.Key.Replace("property.", string.Empty), property.Value);
            }
        }
        _lastResponse = response;
        LastSyncSuccess = true;
        FirePullRequestCompletedEvent(_properties);
    }

    private void HandlePullResponseError(string error)
    {
        _lastPullResponseError = error;
        LastSyncSuccess = false;
    }

    private Dictionary<string, string> ParsePropertyFileFormat(string text)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] array = text.Split('\n');
        foreach (string text2 in array)
        {
            int num = text2.IndexOf('=');
            if (num >= 0)
            {
                string text3 = text2.Substring(0, num).Trim();
                string value = text2.Substring(num + 1).Trim();
                if (dictionary.ContainsKey(text3))
                {
                    UnityEngine.Debug.LogWarning("Result dictionary already contains key '" + text3 + "'");
                }
                dictionary[text3] = value;
            }
        }
        return dictionary;
    }

    public void Serialize()
    {
        _storage.Set("UserId", UserId);
        _storage.Set("LastResponse", _lastResponse);
        _storage.Set("UseBadge", UseBadge);
        _storage.Set("Multipliers", Multipliers);
        _storage.Set("abgroup", ABGroup);
        _storage.Set("cheater_value", CheaterFlag);
        _storage.Set("OverriddenGameProperties", OverriddenGameProperties);
        _storage.Set("IAPSignValidationEnabled", IAPSignValidationEnabled);
        _storage.Set("WheelOfFortuneTokenRewardEnabled", WheelOfFortuneTokenRewardEnabled);
    }
}
