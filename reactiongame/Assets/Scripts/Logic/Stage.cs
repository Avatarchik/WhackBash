using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public enum ELevelType
    {
        Simple,
        Boss
    }

    public class Level
    {
        public readonly int TapsToWin = 2;
        public readonly float DoublespawnChance = -0.1f;
        public readonly float TriplespawnChance = -0.1f;

        public ELevelType Type;

        public Level(int taps, float dchance, float tchance, ELevelType type)
        {
            Type = type;
            TapsToWin = taps;
            DoublespawnChance = dchance;
            TriplespawnChance = tchance;
        }
    }

    public class Stage
    {
        public int StartLevelIndex = 0;

        public float DifficultyModifier = 1.0f;
        public int LevelCount
        {
            get { return levels.Count; }
        }
        private int curLevel = 0;

        public int CurrentLevelIndex
        {
            get { return StartLevelIndex + curLevel; }
        }

        public int CurrentLevelIndexFriendly
        {
            get { return StartLevelIndex + curLevel + 1; }
        }

        public int CurrentLevelIndexLocal
        {
            get { return curLevel; }
        }

        private List<Level> levels = new List<Level>();

        public Stage(int startIndx, int lvlCnt, int baseTaps, int tapsStep, float dchance, float tchance)
        {
            StartLevelIndex = startIndx;

            int taps = 0;
            for (int i = 0; i < lvlCnt; ++i)
            {
                ELevelType tp = (i + 1 == lvlCnt) ? ELevelType.Boss : ELevelType.Simple;
                Level newOne = new Level(baseTaps + taps, dchance, tchance, tp);

                levels.Add(newOne);

                taps += tapsStep;
            }
        }

        public bool IsOver()
        {
            return curLevel >= levels.Count;
        }

        public bool IsLastLevel()
        {
            return curLevel == (levels.Count -1);
        }

        public void ToNextLevel()
        {
            ++curLevel;
        }

        public void NullifyProgress()
        {
            curLevel = 0;
        }

        public Level CurrentLevel
        {
            get
            {
                return levels[(curLevel >= levels.Count) ?
                              (levels.Count - 1) :
                              curLevel]; //infinite on last stage
            }
        }

        public int MaxTapsPerLevel
        {
            get { return levels[(levels.Count - 1)].TapsToWin; }
        }

        internal bool RollForDoublespawn()
        {
            float roll = UnityEngine.Random.Range(0.0f, 1.0f);
            return (CurrentLevel.DoublespawnChance > roll);
        }

        internal bool RollForTriplespawn()
        {
            float roll = UnityEngine.Random.Range(0.0f, 1.0f);
            return (CurrentLevel.TriplespawnChance > roll);
        }
    }
}
