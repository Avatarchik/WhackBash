using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GoogleMobileAds.Api;

namespace Scripts.Logic
{
    public static class GamePlatformController
    {
        static string kRateStatus = "ppk_rateus";
        public static BannerView bannerView;
		private static DateTime PrevChange;

		private static AdRequest.Builder builder;

#if UNITY_ANDROID
        static string kLeaderboardID = "CgkIz8H-rPweEAIQAA";

        static private string rateText = "If you enjoy playing Whack & Bash, please take a moment to rate it. Thanks for your support!";

        static private string rateUrl = "market://details?id=com.shortcut2success.whackbash";

#elif UNITY_IOS
		static string kLeaderboardID = "whackbash.leaderboard";
		static string kLeaderboardID_hard = "whackbash.hard.leaderboard";
#endif

        public static void Init()
        {
            var sz = Screen.width;
            AdSize adsz = new AdSize(sz, 70);
			//ChartboostSDK.Chartboost.setAutoCacheAds(true);
			ChartboostSDK.Chartboost.cacheRewardedVideo(ChartboostSDK.CBLocation.Default);

			bannerView = new BannerView("ca-app-pub-8967299162876384/6137596356", AdSize.SmartBanner, AdPosition.Bottom);
			bannerView.Show();

			builder = new AdRequest.Builder ();
			builder.AddTestDevice (AdRequest.TestDeviceSimulator);
			builder.AddTestDevice("2d0125c887fc781cad324fe391378222");
			//request.testDevices = @[ @"2d0125c887fc781cad324fe391378222" ];

            //AdRequest.Builder;
            //AdRequest request = new AdRequest.Builder().Build();

			// Load the banner with the request. .AddTestDevice("db3ebac41d20c6219ec5f8e2f69259f430c6a785")
            //bannerView.LoadAd(request);
#if UNITY_ANDROID
            GoogleCloudMessageService.Instance.Init();
            GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
            GooglePlayConnection.Instance.Connect();
#elif UNITY_IOS
            GameCenterManager.Init();  
#endif
        }

        public static void UpdateAd()
        {                        
			if (PrevChange == null || (DateTime.Now - PrevChange).TotalSeconds > 60)
			{
				PrevChange = DateTime.Now;

				AdRequest request = builder.Build();
				//AdRequest request = new AdRequest.Builder ().Build();

                // Load the banner with the request.
                bannerView.LoadAd(request);
            }
        }

        public static void ShowUI()
        {
#if UNITY_ANDROID
            if (!GooglePlayConnection.Instance.IsConnected)
            {
                GooglePlayConnection.Instance.Connect();
            }
            else
            {
                GooglePlayManager.Instance.ShowLeaderBoardsUI();
            }
#elif UNITY_IOS
            GameCenterManager.ShowLeaderboards();
#endif
        }

        static bool AlreadyRated()
        {
            IOSDialogResult status = (IOSDialogResult)PlayerPrefs.GetInt(kRateStatus, 0);

            return status == IOSDialogResult.RATED || status == IOSDialogResult.DECLINED;
        }

        public static void RateIt()
        {
            if (!AlreadyRated())
            { 
                Debug.Log("RateAttempt::success");
#if UNITY_ANDROID
                AndroidRateUsPopUp rate = AndroidRateUsPopUp.Create("Rate Us", rateText, rateUrl);
                rate.ActionComplete += onRatePopUpClose;
#else
                IOSRateUsPopUp rate = IOSRateUsPopUp.Create("Like this game?", "Please rate to support future updates!");
				rate.OnComplete += onRatePopUpClose;
#endif
            }
        }

        private static void onRatePopUpClose(IOSDialogResult result)
        {
            PlayerPrefs.SetInt(kRateStatus, (int)result);
            switch (result)
            {
                case IOSDialogResult.RATED:
                    Debug.Log("Rate button pressed");
                    break;
                case IOSDialogResult.REMIND:
                    Debug.Log("Remind button pressed");
                    break;
                case IOSDialogResult.DECLINED:
                    Debug.Log("Decline button pressed");
                    break;
            }

            //IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");
        }

        private static void onRatePopUpClose(AndroidDialogResult result)
        {
            PlayerPrefs.SetInt(kRateStatus, (int)result);
            switch (result)
            {
                case AndroidDialogResult.RATED:
                    Debug.Log("Rate button pressed");
                    break;
                case AndroidDialogResult.REMIND:
                    Debug.Log("Remind button pressed");
                    break;
                case AndroidDialogResult.DECLINED:
                    Debug.Log("Decline button pressed");
                    break;

            }

            //AndroidMessage.Create("Result", result.ToString() + " button pressed");
        }

        private static void ActionConnectionResultReceived(GooglePlayConnectionResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("Connected!");
            }
            else
            {
                Debug.Log("Connection failed with code::: " + result.code.ToString());
            }
        }

        public static void ReportScore(int score)
        {
#if UNITY_ANDROID
            if (GooglePlayConnection.Instance.IsConnected)
            {
                GooglePlayManager.Instance.SubmitScoreById(kLeaderboardID, score);
            }
#else
			if(GameController.IsHardMode)
			{
				GameCenterManager.ReportScore(score, kLeaderboardID_hard); 
			}
			else
			{
				GameCenterManager.ReportScore(score, kLeaderboardID); 
			}
#endif
        } 
    }
}
