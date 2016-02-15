using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Logic;
using System;
using Scripts.Logic;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public class UnavailableCircles
    {
        public List<Circle> circles = new List<Circle>();
        private int maxCount;


        public UnavailableCircles(int maxCapacity)
        {
            maxCount = (int)maxCapacity;
        }

        public void Push(Circle newOne)
        {
            circles.Add(newOne);
            if (circles.Count > maxCount)
            {
                circles.RemoveAt(0);
            }
        }

        public bool Contains(Circle circle) { return circles.Contains(circle); }
    }

    public BackgroundTouchListener Background;
    
    private const float kDoubleTapTreshold = 0.125f;
    private static int tapsLeft = 2; //initial value  
	private static List<double> clickTimeVec = new List<double>();

    public static UnavailableCircles temporaryUnavailable;
    private static List<Circle> circles;
    private static float timeToNextCircleActivation = 0.3f;

    private static int spawnedOnThisLevelCnt = 0;
    
    private static System.Random randomizer; //static to prevent allocations on Update();

	public static DoubleTapBonus Bonus
	{
		get
		{
			if (IsHardMode) 
			{
				return bonus_hard ?? (bonus_hard = new DoubleTapBonus());
			} 
			else 
			{
				return bonus ?? (bonus = new DoubleTapBonus());
			}            
		}
	}

	private static DoubleTapBonus bonus;
	private static DoubleTapBonus bonus_hard;

    public static StageManager Stages
    {
        get
        {
			if (IsHardMode) 
			{
				StageManagerInitArgs args = new StageManagerInitArgs ();
				args.InititalTaps = 5;
				return stages_hard ?? (stages_hard = new StageManager(args));
			} 
			else 
			{
				return stages ?? (stages = new StageManager(new StageManagerInitArgs()));
			}            
        }
    }

    private static StageManager stages;
	private static StageManager stages_hard;

    public static int LevelIndex
    {
        get
        {
            return Stages.CurrentStage.CurrentLevelIndexFriendly;
        }
    }
    public static int GainedScore { get; set; }

    public static float LastTapTime { get; set; }

    public static bool IsLevelSuccesful
    {
        get; set;
    }

    public static int TapsLeft
    {
        get
        {
            return tapsLeft;
        }
    }

    private static bool gameOver;
    private static int lastGained = 0;

	public static bool IsHardMode
	{
		get; set;
	}

	public static Vector3 llCorner;
	public static Vector3 trCorner;
	public static float boardWidth;
	public static float boardHeight;

    private static GameController self;

    public Scripts.CirclesBarScript CirclesBar;

    void Awake()
    {		
		timeToNextCircleActivation = IsHardMode ? 0.27f : 0.3f;
        Bonus.CheckForTimeout();

        var pairLitCap = Bonus.GetLitCapPair();
        CirclesBar.UpdateBar(pairLitCap.Key, pairLitCap.Value);
        Bonus.OnBonusGiven += OnBonus;

        Debug.Log("==========================================================");
        this.Background.OnBackgroundTouch += OnCircleDeactivatedHandler; 

        gameOver = false;

        circles = new List<Circle>();
		temporaryUnavailable = new UnavailableCircles(IsHardMode ? 9 : 2);

        tapsLeft = Stages.CurrentStage.CurrentLevel.TapsToWin;

        LastTapTime = -300;
        lastGained = 0;
        spawnedOnThisLevelCnt = 0;

        clickTimeVec.Clear();

        self = this;

		Vector3[] gameboardCoords_world = new Vector3[4];
		Image gameboard = transform.GetChild (0).GetChild (1).GetComponent<Image> ();

		gameboard.rectTransform.GetWorldCorners(gameboardCoords_world);
		llCorner = new Vector3 (gameboardCoords_world[0].x, gameboardCoords_world[0].y);
		trCorner = new Vector3 (gameboardCoords_world[2].x, gameboardCoords_world[2].y);

		boardWidth = trCorner.x - llCorner.x;
		boardHeight = trCorner.y - llCorner.y;

        if (randomizer == null)
            randomizer = new System.Random();	
    }

	void Start()
	{
		if (Stages.CurrentStage.IsLastLevel())
		{
			self.StartCoroutine(self.ActivateAllTheCirlesDelayed(0.25f));
			tapsLeft = Stages.StageIndexFriendly * 9;
			#if !UNITY_EDITOR
			Circle.circleColorChangeTimedelay = 0.047f; 
			#else
			Circle.circleColorChangeTimedelay = 0.095f;
			#endif
		} //fix it later
		else if (Stages.StageIndexFriendly > 2)
		{
			Circle.circleColorChangeTimedelay = 0.018f;
		}
		else if (Stages.StageIndexFriendly > 1)
		{
			Circle.circleColorChangeTimedelay = 0.0165f;
		}
	}

    private void OnBonus(bool bonusEarned, int points)
    {
        Debug.Log("GameController::OnBonus was just called");
        //todo: spawn effect, give score
        if (bonusEarned)
        {
            int lit = Bonus.GetLitCapPair().Value - 1;
            int score = lit * 5 * Stages.StageIndexFriendly;
            points = score;
            CirclesBar.StartCoroutine(CirclesBar.ResetCor(Bonus.GetLitCapPair().Value));
        }
        else
        {
            var pairLitCap = Bonus.GetLitCapPair();
            CirclesBar.UpdateBar(pairLitCap.Key, pairLitCap.Value);
        }

        ShowDoubleBonus(points, bonusEarned);
    }
    
    // :(
    static void ShowDoubleBonus(int pts, bool bonus)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>(bonus ? "DT2" : "DT"));

        if (bonus)
        {
            var ac = Resources.Load("doubletap_sfx_bonus") as AudioClip;
            AudioSource.PlayClipAtPoint(ac, Vector3.zero);
            int sum = pts;

            var txt = go.transform.FindChild("Text2").GetComponent<UnityEngine.UI.Text>();

            txt.text = "+" + sum;
            GainedScore += sum;
        }
        else
        { 
            var ac = Resources.Load("doubletap_sfx") as AudioClip;
            AudioSource.PlayClipAtPoint(ac, Vector3.zero);
            int sum = pts + lastGained;

            var txt = go.transform.FindChild("Text2").GetComponent<UnityEngine.UI.Text>();

            txt.text = "+" + sum;
            GainedScore += sum;
        }


        GameObject.Destroy(go, 1.3f);
    }

    IEnumerator ActivateAllTheCirlesDelayed(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        foreach (var cl in circles)
        {
            cl.ActivateCircle();
			temporaryUnavailable.Push(cl);
        }
    }

    internal static void OnNextLevelBegin(bool isLevelSuccesful)
    {
        if (isLevelSuccesful)
        {
            Stages.OnLevelEnd();
        } 
    }

    void OnDestroy()
    {
        // del it for the gods sake.
        this.Background.OnBackgroundTouch -= OnCircleDeactivatedHandler;
        Bonus.OnBonusGiven -= OnBonus;
    } 

    public static void AddCircle(Circle circle)
    {
        if (circles == null)
        {
            circles = new List<Circle>();
        }

        circles.Add(circle);
        circle.OnCircleDeactivated += OnCircleDeactivatedHandler;
    }

    private static void OnCircleDeactivatedHandler(Circle sender, CircleDeactivatedEventArgs args)
	{
		GainedScore += args.GainedPoints;

		if (Stages.CurrentStage.IsLastLevel()){
			temporaryUnavailable.circles.Remove (sender);
		} else {			
			temporaryUnavailable.Push (sender);
		}

        if (args.GainedPoints < 1 || tapsLeft <= 1)
        {
            gameOver = true;
            foreach (var circle in circles)
            {
                circle.OnCircleDeactivated -= OnCircleDeactivatedHandler;
                circle.SetToWhiteAndDisable(); 
            } 

            if (tapsLeft <= 1)
            {
                IsLevelSuccesful = true;
            }

            if (args.GainedPoints <= 0)
            {
                IsLevelSuccesful = false;
            }

            if (args.TappedOutside)
            {
				DateTime time = DateTime.Now;
                var ac = Resources.Load("BuzzerSFX") as AudioClip;
                AudioSource.PlayClipAtPoint(ac, Vector3.zero, 0.7f);

                self.Background.MakeItRed();

                self.StartCoroutine(self.GameOverAfter(1.25f));
                self.Background.box.enabled = false;


                foreach(var it in circles)
                {
                    it.CircleCol.enabled = false;
				} 
				Debug.Log ("before end of level time_:::" + (DateTime.Now - time).TotalMilliseconds);
            }
            else
			{
				DateTime time = DateTime.Now;
                tapsLeft--;
				self.StartCoroutine(self.GameOverAfter(0.2f));
				Debug.Log ("before end of level time_:::" + (DateTime.Now - time).TotalMilliseconds);
            }
        }
        else
        {
            if ( (Time.time - LastTapTime) < kDoubleTapTreshold)
            {
                // showing the bonus  
                LastTapTime = -10;
                Bonus.OnDoubletapped(args.GainedPoints);
            }
            else
            { 
                LastTapTime = Time.time;
                clickTimeVec.Add(sender.GetReactionTimeMs());
            }
            
            tapsLeft--;
        }

        bool clear = circles.Where(circle => circle.IsActivated).Count() == 0;
        if (clear && !gameOver && Stages.CurrentStage.IsLastLevel())
        {
            self.StartCoroutine(self.ActivateAllTheCirlesDelayed(0.2f));
        }

        lastGained = args.GainedPoints;
    }

    public static int GetAverageReactionTime()
    {
        if (0 == clickTimeVec.Count)
        {
            return 0;
        }

        double summ = 0;
        foreach (var it in clickTimeVec)
        {
            summ += it;
        }
        summ = summ / clickTimeVec.Count;
        return (int)summ;
    }

    IEnumerator GameOverAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Application.LoadLevel("EndOfLevel");
    } 

    void Update()
    {
        // could be replaced for performance sake.
        if (!Stages.CurrentStage.IsLastLevel())
        {
            ActivateCircleIfPossible();
        } 
        GamePlatformController.UpdateAd(); 
    }

    private /*static*/ /*???*/ void ActivateCircleIfPossible()
    { 
        if (gameOver || spawnedOnThisLevelCnt >= Stages.CurrentStage.CurrentLevel.TapsToWin)
        {
            return;
        }

        if (timeToNextCircleActivation <= 0 && tapsLeft > 0)
        {
            var activatedCirclesCount = circles.Where(circle => circle.IsActivated
                                                      || temporaryUnavailable.Contains(circle))
                                                      .Count();

            if (activatedCirclesCount > 8)
            {
                return;
            }
            else
            {
                int left = Math.Min(9 - activatedCirclesCount, Stages.CurrentStage.CurrentLevel.TapsToWin - spawnedOnThisLevelCnt);
				float timeMod = Spawn(left);
				if (IsHardMode) 
				{
					timeToNextCircleActivation = UnityEngine.Random.Range(0.252f * timeMod, 0.72f * timeMod);
				} 
				else 
				{
					timeToNextCircleActivation = UnityEngine.Random.Range(0.28f * timeMod, 0.8f * timeMod);
				}
            }      
        }

        timeToNextCircleActivation -= Time.deltaTime;
    }

    private float Spawn(int circlesLeft)
    {
        if ((circlesLeft >= 3) && Stages.CurrentStage.RollForTriplespawn())
        { 
            Circle c1 = FindRandomInactiveCricle();
            c1.ActivateCircle();
            temporaryUnavailable.Push(c1);

            Circle c2 = FindRandomInactiveCricle();
            c2.ActivateCircle();
            temporaryUnavailable.Push(c2);

            Circle c3 = FindRandomInactiveCricle(); 
            c3.ActivateCircle();
            temporaryUnavailable.Push(c3);

            spawnedOnThisLevelCnt += 3;
            Debug.Log("[TRIPLE 333 SPAWN] " + c1.name + "-" + c2.name + "-" + c3.name);
            
            return 1.7f;
        }
        else if((circlesLeft >= 2) && Stages.CurrentStage.RollForDoublespawn())
        { 
            Circle c1 = FindRandomInactiveCricle();
            c1.ActivateCircle();
            temporaryUnavailable.Push(c1);

            Circle c2 = FindRandomInactiveCricle();  
            c2.ActivateCircle();
            temporaryUnavailable.Push(c2);

            Debug.Log("[Double 22 SPAWN] " + c1.name + "-" + c2.name);
            spawnedOnThisLevelCnt += 2;
            
            return 1.3f;
        }
        else
        {
            Circle c1 = FindRandomInactiveCricle(); 
            c1.ActivateCircle();
            Debug.Log("[SPAWN] " + c1.gameObject.name);
            spawnedOnThisLevelCnt++;

            temporaryUnavailable.Push(c1);

            return 1;
        }
    }

    private Circle FindRandomInactiveCricle()
    {
        Circle circleToLit;

        const int mx = 100;
        int dbgGuard = 0;
        do
        {
            circleToLit = circles[randomizer.Next(0, circles.Count)];
            ++dbgGuard;
            Debug.Assert(dbgGuard < mx, "ALARM !!! Infinite loop");
        } 
        while (temporaryUnavailable.Contains(circleToLit) || circleToLit.IsActivated);

        return circleToLit;
    }
}
