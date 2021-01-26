using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Placement;

public class AdMobInter : MonoBehaviour
{
    // Start is called before the first frame update
    /*void Start()
    {
        MobileAds.Initialize(initStatus => { });
        InterstitialAdGameObject interAd = MobileAds.Instance.GetAd<InterstitialAdGameObject>("Interstitial Ad");
    }*/
    void Start()
    {
        InterstitialAdGameObject interstitialAd = MobileAds.Instance
                    .GetAd<InterstitialAdGameObject>("Interstitial Ad");
        MobileAds.Initialize((initStatus) =>
        {
            Debug.Log("Initialized MobileAds");
            interstitialAd.LoadAd();
        });
        interstitialAd.ShowIfLoaded();



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
