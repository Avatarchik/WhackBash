using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.Logic;
using ChartboostSDK;
using System;

public enum ESecondChance
{
    NotActive,
    Activated,
    Used
}

public class EndOfLevelController : MonoBehaviour
{
    public static ESecondChance SecondChanceUsed = ESecondChance.NotActive;
    public GameObject StageLayer;
    public UnityEngine.UI.Text StageTxt;
    public UnityEngine.UI.Text TimeText; 

	private static List<double> averageForGame = new List<double>();

    public static bool IsGameOver { get; set; }
    // Use this for initialization
    void Awake()
    {  
		DateTime time = DateTime.Now;
        Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
        Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
        Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
        Chartboost.didClickRewardedVideo += didClickRewardedVideo;
        Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
        Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
        Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
        Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;

		//ChartboostSDK.CBExternal.

        IsGameOver = !GameController.IsLevelSuccesful;

        GameController.OnNextLevelBegin(GameController.IsLevelSuccesful);

        if (!GameController.Stages.Unlocked())
        {
            Analytics.Flurry.Instance.LogEvent("OnStageUnlocked",  new Dictionary<string, string>
                    {
                        { "Stage", GameController.Stages.StageIndexFriendly.ToString() },
                    });
            
            Debug.Log("STAGE UNLOCKED >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            StageLayer.SetActive(true);

            var ac = Resources.Load("cheer_hooray") as AudioClip;
            AudioSource.PlayClipAtPoint(ac, Vector3.zero);

            StageTxt.text = string.Format(StageTxt.text,
                            GameController.Stages.StageIndexFriendly);
        }
        else if (!IsGameOver)
        {
            var ac = Resources.Load("cheer") as AudioClip;
            AudioSource.PlayClipAtPoint(ac, Vector3.zero);
        }

        var time2 = GameController.GetAverageReactionTime();

		if (time2 > 0)
			averageForGame.Add(time2);

        GameController.Stages.SaveProgress(); 
		
		Debug.Log ("before end of level time_1:::" + (DateTime.Now - time).TotalMilliseconds);
        if (IsGameOver)
        {
            Analytics.Flurry.Instance.LogEvent("OnGameOver", new Dictionary<string, string>
                    {
                        { "Stage", GameController.Stages.StageIndexFriendly.ToString() },
                        { "Level", GameController.Stages.CurrentStage.CurrentLevelIndex.ToString() },
                    }
            );

            bool hasRewardedVideo = Chartboost.hasRewardedVideo(CBLocation.Default);
            int indx = GameController.Stages.CurrentStage.CurrentLevelIndexFriendly;
			Debug.Log("REWARDED VIDEO STAUTS_:::" + hasRewardedVideo
                + "\n SC_STATUS_:::" + SecondChanceUsed.ToString());
            if (hasRewardedVideo && (indx >= 6) && (indx <= 9) && 
                (ESecondChance.NotActive == SecondChanceUsed))
           // if (!SecondChanceUsed)
            {  
#if UNITY_ANDROID
                AndroidDialog dialog = AndroidDialog.Create("Second Chance", "Watch a short video to continue this game?");
                dialog.ActionComplete += OnDialogClose;
#else
                IOSDialog dialogIos = IOSDialog.Create("Second Chance", "Watch a short video to continue this game?");
				dialogIos.OnComplete += OnDialogClose;
#endif
            }
            else
            {
				Chartboost.cacheRewardedVideo(CBLocation.Default);
                StartCoroutine(NullifyProgress());
            }

			var avForGame = getAverageForGame();
			TimeText.text = string.Format(TimeText.text, avForGame > 0 ? avForGame.ToString() : " --");
			averageForGame.Clear();

			GameController.Bonus.ResetLit();
        }
		else
        {
            TimeText.text = string.Format(TimeText.text, time2 > 0 ? time2.ToString() : " --");
		}
		
		Debug.Log ("before end of level time 2 _:::" + (DateTime.Now - time).TotalMilliseconds);
    } 

	private int getAverageForGame()
	{
		if (0 == averageForGame.Count)
		{
			return 0;
		}
		
		double summ = 0;
		foreach (var it in averageForGame)
		{
			summ += it;
		}
		summ = summ / averageForGame.Count;
		return (int)summ;
	}
	
	private void OnDialogClose(IOSDialogResult result)
	{ 
		//parsing result
		switch (result)
		{
		case IOSDialogResult.YES:
			Debug.Log("NA_popup:::Yes button pressed");
			OnSecondChance();
			break;
			
		case IOSDialogResult.NO:
			Debug.Log("NA_POPUP:::No button pressed");
			StartCoroutine(NullifyProgress()); 
			break; 
		} 
	}

    private void OnDialogClose(AndroidDialogResult result)
    { 
        //parsing result
        switch (result)
        {
            case AndroidDialogResult.YES:
                Debug.Log("NA_popup:::Yes button pressed");
                OnSecondChance();
                break;

            case AndroidDialogResult.NO:
                Debug.Log("NA_POPUP:::No button pressed");
                StartCoroutine(NullifyProgress()); 
                break; 
        } 
    }

    IEnumerator NullifyProgress()
    {
        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0.01f);

        GameController.Stages.ResetProgress();
    }

    public void OnContinueTap()
    {
        if (!EndOfLevelController.IsGameOver)
        {
			if (GameController.Stages.CurrentStage.IsLastLevel())
			{
				Application.LoadLevel("NextLevel");
			}
			else
			{
				Application.LoadLevel("Game");
			}
        }
        else
        {
            if (SecondChanceUsed != ESecondChance.Activated)
            {
                GameController.Stages.ResetProgress();
            }
            if (SecondChanceUsed == ESecondChance.Used)
            {
                SecondChanceUsed = ESecondChance.NotActive;
            }
            Application.LoadLevel("Menu");
        }
    }

    void Update()
    {
        GamePlatformController.UpdateAd();
    }

    void OnSecondChance()
    {  
        {
            Debug.Log("hasRewardedVideo:::showed");
            Chartboost.showRewardedVideo(CBLocation.Default);
        }
    }

    void GiveSecondChance()
    {
        SecondChanceUsed = ESecondChance.Activated;
        Application.LoadLevel("NextLevel");
    }

    void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
    {
        Debug.Log(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
    }

    void didDismissRewardedVideo(CBLocation location)
    {
		Debug.Log("didDismissRewardedVideo: " + location);
    }

    void didCloseRewardedVideo(CBLocation location)
    {
        Debug.Log("didCloseRewardedVideo: " + location); 
    }

    void didClickRewardedVideo(CBLocation location)
    {
        Debug.Log("didClickRewardedVideo: " + location);
        GiveSecondChance();
    }

    void didCacheRewardedVideo(CBLocation location)
    {
        Debug.Log("didCacheRewardedVideo: " + location);
    }

    bool shouldDisplayRewardedVideo(CBLocation location)
    {
        Debug.Log("shouldDisplayRewardedVideo @" + location + " : " + true);
        return true;
    }

    void didCompleteRewardedVideo(CBLocation location, int reward)
    {
        Debug.Log(string.Format("didCompleteRewardedVideo: reward {0} at location {1}",
            reward, location));

        GiveSecondChance();
    }

    void didDisplayRewardedVideo(CBLocation location)
    {
        Debug.Log("didDisplayRewardedVideo: " + location);
    }

    void OnDestroy()
    {
        // Remove event handlers 
        Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
        Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
        Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
        Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
        Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
        Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
        Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
        Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo; 
    }
}
