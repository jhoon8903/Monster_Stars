using UnityEngine;

public class MaxSdkLogger
{
    private const string SdkTag = "AppLovin MAX";
    public const string KeyVerboseLoggingEnabled = "com.applovin.verbose_logging_enabled";

    public static void UserDebug(string message)
    {
        Debug.Log("Debug [" + SdkTag + "] " + message);
    }
    
    public static void D(string message)
    {
        if (MaxSdk.IsVerboseLoggingEnabled())
        {
            Debug.Log("Debug [" + SdkTag + "] " + message);
        }
    }
    
    public static void UserWarning(string message)
    {
        Debug.LogWarning("Warning [" + SdkTag + "] " + message);
    }

    public static void W(string message)
    {
        if (MaxSdk.IsVerboseLoggingEnabled())
        {
            Debug.LogWarning("Warning [" + SdkTag + "] " + message);
        }
    }
    public static void UserError(string message)
    {
        Debug.LogError("Error [" + SdkTag + "] " + message);
    }
    
    public static void E(string message)
    {
        if (MaxSdk.IsVerboseLoggingEnabled())
        {
            Debug.LogError("Error [" + SdkTag + "] " + message);
        }
    }
}
