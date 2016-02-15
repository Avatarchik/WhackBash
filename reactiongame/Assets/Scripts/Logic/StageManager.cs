using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic
{
    public class StageManagerInitArgs
    {		
#if UNITY_EDITOR
        public int InititalTaps = 2;
#else
        public int InititalTaps = 2;
#endif
        public int TapStep = 1;
#if UNITY_EDITOR
        public int LevelPerStage = 10;
        public int StartDoublesFromStage = 0;
        public int StartTriplesFromStage = 0;
#else
        public int LevelPerStage = 10;
        public int StartDoublesFromStage = 1;
        public int StartTriplesFromStage = 2;
#endif
        public float ChanceGrowPerStage = 0.2f;
        public int StagesCont = 120; 
    }

    public class StageManager
    {
        const string kFirstStageAchID = "CgkIz8H-rPweEAIQAg";
        const string kSecondStageAchID = "CgkIz8H-rPweEAIQAw";
        const string kThirdStageAchID = "CgkIz8H-rPweEAIQBA";

        const string kForthtStageAchID = "CgkIz8H-rPweEAIQBQ";
        const string kFifthStageAchID = "CgkIz8H-rPweEAIQBg";
        const string kSixthStageAchID = "CgkIz8H-rPweEAIQBw";

        const string kStoreKey = "stage_unlocked";
		const string kStoreKey_hard = "hard_stage_unlocked";
        const string kStoreKeyForScore = "stage_unlocked_last_score";
		const string kStoreKeyForScore_hard = "hard_stage_unlocked_last_score";

        public const float kMaxDoubleChance = 0.24f;
        public const float kMaxTripleChance = 0.24f;

        private int stage = 0;

        public int StageIndexFriendly
        {
            get { return stage + 1;  }
        }

        public Stage CurrentStage
        {
            get
            {
                int i = stage < stages.Count ? stage : stages.Count - 1;
                return stages[i];
            }
        } 

        private List<Stage> stages = new List<Stage>();

        public StageManager(StageManagerInitArgs args)
        {
            float initialD = 0.1f;
            float initialT = 0.1f;

            float d = 0;
            float t = 0;
            for (int i = 0; i < args.StagesCont; ++i)
            {
                int startIndx = i * args.LevelPerStage;
                
                if (i >= args.StartDoublesFromStage)
                {
                    d = initialD + (args.ChanceGrowPerStage * (i - args.StartDoublesFromStage));
                    d = d > 0.24f ? 0.24f : d;
                }
                if (i >= args.StartTriplesFromStage)
                {
                    t = initialT + (args.ChanceGrowPerStage * (i - args.StartTriplesFromStage));
                    t = t > 0.24f ? 0.24f : t;
                }
                
                Stage stg = new Stage(startIndx, args.LevelPerStage, args.InititalTaps, args.TapStep, d, t);

                if (2 == i)
                {
                    stg.DifficultyModifier = 0.95f;
                }

                stages.Add(stg);

                args.InititalTaps = stg.MaxTapsPerLevel + args.TapStep;
            }

            RestoreProgress();
        } 

        public void OnLevelEnd()
        {
            CurrentStage.ToNextLevel();
            bool stageOver = CurrentStage.IsOver();

            if (stageOver)
            {
				if (GameController.IsHardMode) 
				{
					PlayerPrefs.SetInt (kStoreKeyForScore_hard, GameController.GainedScore); 
				} 
				else 
				{
					PlayerPrefs.SetInt (kStoreKeyForScore, GameController.GainedScore); 
				}

                this.Next();
            }
        }

        public void SaveProgress()
        {
			if (GameController.IsHardMode) 
			{
				if (PlayerPrefs.GetInt (kStoreKey_hard, 0) < stage) 
				{
					PlayerPrefs.SetInt(kStoreKey_hard, stage);
				}
			}
            else if (PlayerPrefs.GetInt(kStoreKey, 0) < stage)
            {
                PlayerPrefs.SetInt(kStoreKey, stage);

                if (1 == stage)
                {
                    GooglePlayManager.Instance.UnlockAchievementById(kFirstStageAchID);
                }
                else if (2 == stage)
                {
                    GooglePlayManager.Instance.UnlockAchievementById(kSecondStageAchID);
                }
                else if (3 == stage)
                {
                    GooglePlayManager.Instance.UnlockAchievementById(kThirdStageAchID);
                }
                else if (4 == stage)
                {
                    GooglePlayManager.Instance.UnlockAchievementById(kForthtStageAchID);
                }
                else if (5 == stage)
                {
                    GooglePlayManager.Instance.UnlockAchievementById(kFifthStageAchID);
                }
                else if (6 == stage)
                {
                    GooglePlayManager.Instance.UnlockAchievementById(kSixthStageAchID);
                }
            }
        }

        public void ResetProgress()
        {
            CurrentStage.NullifyProgress();
			GameController.GainedScore = GameController.IsHardMode ? PlayerPrefs.GetInt(kStoreKeyForScore_hard, 0) : PlayerPrefs.GetInt(kStoreKeyForScore, 0);
        }

        internal void UnlockDBG(int stageN)
        {
			if (GameController.IsHardMode) 
			{
				PlayerPrefs.SetInt(kStoreKey_hard, stageN);
			} 
			else 
			{
				PlayerPrefs.SetInt(kStoreKey, stageN);
			}
            
            stage = stageN;

			foreach (var st in stages)
			{
				st.NullifyProgress();
			}
        }

        public void RestoreProgress()
        {
			stage = GameController.IsHardMode ? PlayerPrefs.GetInt(kStoreKey_hard, 0) : PlayerPrefs.GetInt(kStoreKey, 0);
			GameController.GainedScore = GameController.IsHardMode ? PlayerPrefs.GetInt(kStoreKeyForScore_hard, 0) : PlayerPrefs.GetInt(kStoreKeyForScore, 0);
        }

        private void Next()
        {
            ++stage;
        }

        internal bool Unlocked()
        {
			return GameController.IsHardMode ? PlayerPrefs.GetInt(kStoreKey_hard, 0) >= stage : PlayerPrefs.GetInt(kStoreKey, 0) >= stage;
        }
    }
}
