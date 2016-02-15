//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Analytics;
using Scripts.Logic;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class MenuController : MonoBehaviour
{  
    private static bool Inited = false;
    
    public Text CircleSpeed;

    public Text SpawnsCount;

    public Text HintTxt;

    public Image GamePlatform;

    public Sprite AndroidVersion;
    public Sprite IOSVersion;

    public Text StageTxt;

    private static string[] hints = new string[] { "Hint: Use 2 fingers for an increasing double tap bonus!!!",
        "Hint: When the stage final is beat, all following games you play will start on the next stage!",
        "Hint: The double tap bonus gets higher and higher and is reset first after 24 hours of inactivity! So keep playing for higher bonuses!", };

    void Start()
    {
        Debug.Log("=================>>> Init ==" + Inited);
        {
#if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
#endif
        }
        // For Flurry Android only: 
#if UNITY_IOS
        GamePlatform.sprite = IOSVersion;
#elif UNITY_ANDROID
        GamePlatform.sprite = AndroidVersion;
#endif

        if (!string.IsNullOrEmpty(this.CircleSpeed.text))
        {
            Circle.circleColorChangeTimedelay = float.Parse(this.CircleSpeed.text);
        }
        else
        {
            Circle.circleColorChangeTimedelay = 0.02f;
        }

        if (!string.IsNullOrEmpty(this.SpawnsCount.text))
        {
            NextLevelController.SpawnsPerLevel = Convert.ToInt32(this.SpawnsCount.text);
        }
        else
        {
            NextLevelController.SpawnsPerLevel = 5;
        }

        int phId = UnityEngine.Random.Range(0, hints.Length - 1);
        HintTxt.text = hints[phId];

        if (!Inited)
        {
            //FlurryAndroid.SetLogEnabled(true);
            // For Flurry iOS only:
            //FlurryIOS.SetDebugLogEnabled(true);
            Flurry.Instance.StartSession("5F6SVFHCMCJ89Z4CQTR8", "Z6J52369XQWZN83Y3WMD");
            CircleSpeed.text = Circle.circleColorChangeTimedelay.ToString();
            SpawnsCount.text = NextLevelController.SpawnsPerLevel.ToString();

			OneSignal.Init("30c61093-91ed-4adf-bd9d-a9402b35532c", "1064172495055");
			OneSignal.RegisterForPushNotifications();
			OneSignal.EnableVibrate(true);
			OneSignal.EnableInAppAlertNotification(true);

            Debug.Log("=================>>> Init social");
            GamePlatformController.Init();
            Debug.Log("=================>>> Send game start");
            Flurry.Instance.LogEvent("OnGameStart");
        }
        Inited = true;
    }
	
	protected void OnDestroy()
	{ 
	}

    void Update()
    {
        GamePlatformController.UpdateAd();
    }
	
	protected void OnDisable()
	{
	}
	
	protected void OnEnable()
	{
	}

    public void OnGameCenterTap()
    {
        GamePlatformController.ShowUI();
    }

    public void OnSettingsTap()
    {
        Debug.Log("Tapped on settings button.");
    } 

	public void OnGameBegins(bool isHard)
    {
        //		string number = StageTxt.text;
        //		
        //		Debug.Log(number);
        //		int stageN = -1;
        //		bool result = int.TryParse(number, out stageN);
        //		if (result && stageN > -1)
        //		{
        //			Debug.Log("__:::Parsed");
        //			PlayerPrefs.DeleteAll();
        //			GameController.Stages.UnlockDBG(stageN > 0 ? stageN - 1 : 0);
        //		}
		GameController.IsHardMode = isHard;
        Application.LoadLevel("NextLevel");
    }

    public void GoOnStage(string txt)
    {
    }
}
