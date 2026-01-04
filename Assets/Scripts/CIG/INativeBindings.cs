namespace CIG
{
    public interface INativeBindings
    {
        string CountryCode
        {
            get;
        }

        string DeviceUUID
        {
            get;
        }

        string UniqueID
        {
            get;
        }

        string Referrer
        {
            get;
        }

        string DeviceOSVersion
        {
            get;
        }

        void ShowSpinner();

        void HideSpinner();

        void RateThisApp(string appId, string title, string body);

        void OpenAppInStore(string appId, string appUrl, string referrer);
    }
}
