using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Placement;

public class AdMob : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*List<string> deviceIds = new List<string>();
        deviceIds.Add("2077ef9a63d2b398840261c8221a0c9b");
        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(deviceIds)
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.SetiOSAppPauseOnBackground(true);*/
        MobileAds.Initialize(initStatus => { });
        BannerAdGameObject banner = MobileAds.Instance.GetAd<BannerAdGameObject>("Banner Ad");
        banner.LoadAd();
    }
}
