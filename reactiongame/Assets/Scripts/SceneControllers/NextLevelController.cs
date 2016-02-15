using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Scripts.Logic;
using Analytics;

public class NextLevelController : MonoBehaviour
{
    public Canvas Ordinary;
    public Canvas StageEnd;

    public Text ContinueText;

    public static int SpawnsPerLevel = 5;
    public Text NextLevelText;
    public Text StageText;
    // Use this for initialization
    void Start()
    { 
        if (GameController.Stages.CurrentStage.IsLastLevel())
        {
            Ordinary.gameObject.SetActive(false);
            StageEnd.gameObject.SetActive(true);

            ContinueText.text = "Tap away!";

            Flurry.Instance.LogEvent("StageFinalStart");
        }
        else
        {
            Flurry.Instance.LogEvent("LevelStart");
            Ordinary.gameObject.SetActive(true);
            StageEnd.gameObject.SetActive(false);

            StageText.text = string.Format("Stage {0}",
                GameController.Stages.StageIndexFriendly);

            NextLevelText.text = string.Format("Level {0} ({1})",
                GameController.Stages.CurrentStage.CurrentLevelIndexLocal + 1,
                GameController.Stages.CurrentStage.LevelCount);

            ContinueText.text = EndOfLevelController.SecondChanceUsed == ESecondChance.Activated ?
                "Second Chance" : "Go!";

            if (EndOfLevelController.SecondChanceUsed == ESecondChance.Activated)
            { 
                EndOfLevelController.SecondChanceUsed = ESecondChance.Used;
            }
        }
    }

	public void OnNextLevel()
	{		
		Debug.Log ("Starting new level.");
		Application.LoadLevel ("Game");			
	}

    void Update()
    {
        GamePlatformController.UpdateAd();
    }
}
