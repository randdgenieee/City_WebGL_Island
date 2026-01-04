
namespace CIG
{
    public static class AdMobManager
    {
        public const string PlaystoreAppId = "ca-app-pub-2218023806663419~2139954438";

        public const string iOSAppId = "ca-app-pub-2218023806663419~4542756494";

        private static bool _initialized;

        private static string AppId => "ca-app-pub-2218023806663419~2139954438";

        public static void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
            }
        }
    }
}
