using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Scripts.Logic;

public class StatusText : MonoBehaviour
{
    public Text buttonText;
    public Text ScoreText;
    public Text HighScoreText;

    public Image ShadowImg;
    public Image BubbleImg;

	public Text NextLevelText;
	public Text StageText;
	public GameObject progressBar;

    private static string[] loserPhrases = new string[] { "Game over", "You suck", "Are you blind?",
        "Getting old?", "Spasms?", "Not for you", "My mom beats you", "Too fast for u?", "Retire!", "Give up!", "Loser!" };
    // Use this for initialization
    void Start()
    {
		int temp_highscore = GameController.IsHardMode ? PlayerPrefs.GetInt ("HighScore_Hard", 0) : PlayerPrefs.GetInt ("HighScore");

        var statusText = GetComponent<Text>();


        if (EndOfLevelController.IsGameOver)
        {			
            ScoreText.text = string.Format("Score: {0} ({1})",
                                            GameController.GainedScore,
											temp_highscore);
        }
        else
        {
            ScoreText.text = string.Format("Score: {0}",
                                            GameController.GainedScore);
        }

        if (EndOfLevelController.IsGameOver)
        {
			StageText.gameObject.SetActive (false);
			NextLevelText.gameObject.SetActive (false);
			progressBar.gameObject.SetActive (false);

			if (GameController.GainedScore > temp_highscore)
            {
                statusText.text = loserPhrases[0]; 
                statusText.color = new Color(2.4f, 0.4f, 0.2f);

                var ac = Resources.Load("yay") as AudioClip;

                AudioSource.PlayClipAtPoint(ac, Vector3.zero);
                HighScoreText.text = "New highscore!";
                HighScoreText.color = new Color(0.39f, 1.74f, 0.94f, 1);
                GamePlatformController.ReportScore(GameController.GainedScore);
                TryToShowRatePopup();
            }
            else
            {
                statusText.text = loserPhrases[new System.Random().Next(0, loserPhrases.Length)];
                var ac = Resources.Load("failsound") as AudioClip;
                AudioSource.PlayClipAtPoint(ac, Vector3.zero);
                statusText.color = new Color(2.4f, 0.4f, 0.2f);
            } 
            buttonText.text = "Restart";
            /////////////////
			var highScore = temp_highscore;
            Debug.Log(string.Format("Highscore is {0}", highScore));

            if (GameController.GainedScore > highScore)
            {
				if (GameController.IsHardMode) 
				{
					PlayerPrefs.SetInt("HighScore_Hard", GameController.GainedScore);
				} 
				else 
				{
					PlayerPrefs.SetInt("HighScore", GameController.GainedScore);
				}
            }
            /////////////////
        }
        else
        {
            BubbleImg.gameObject.SetActive(false);
            ShadowImg.gameObject.SetActive(false);

			progressBar.gameObject.SetActive (true);

			StageText.gameObject.SetActive (true);
			StageText.text = string.Format("Stage {0}",
				GameController.Stages.StageIndexFriendly);

			/*NextLevelText.gameObject.SetActive (true);
			NextLevelText.text = string.Format("Level {0} ({1})",
				GameController.Stages.CurrentStage.CurrentLevelIndexLocal + 1,
				GameController.Stages.CurrentStage.LevelCount);*/

            buttonText.text = "Continue";
            statusText.text = "";
         } 
    }

    void TryToShowRatePopup()
    {
        if (GameController.Stages.StageIndexFriendly >= 2)
        {
            Debug.Log("RateAttempt");
            GamePlatformController.RateIt();
        }
    }

}
