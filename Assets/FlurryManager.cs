using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlurrySDK;

public class FlurryManager : MonoBehaviour
{
    #if UNITY_ANDROID
    private readonly string FLURRY_API_KEY = "RZFVJ7GDVV42YP4K5R9N";
    #elif UNITY_IPHONE
    private readonly string FLURRY_API_KEY = "RZFVJ7GDVV42YP4K5R9N";
    #else 
    private readonly string FLURRY_API_KEY = null;
    #endif

    private void Start()
    {
        new Flurry.Builder()
            .WithCrashReporting(true).WithLogEnabled(true).WithLogLevel(Flurry.LogLevel.DEBUG).WithMessaging(true).Build(FLURRY_API_KEY);
    }
}
