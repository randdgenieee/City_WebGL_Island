using System;
using UnityEngine;

public static class NativeGameVersion
{
    public static string Version
    {
        get
        {
            return "1.11.8";
        }
    }

    public static int Build
    {
        get
        {
            return 27920;
        }
    }

    public static string GameTitle => "City Island 5";

    private static AndroidJavaObject GetPackageInfo()
    {
        return new AndroidJavaClass("com.innovattic.PackageInfoHelper")?.CallStatic<AndroidJavaObject>("getPackageInfo", Array.Empty<object>());
    }
}
