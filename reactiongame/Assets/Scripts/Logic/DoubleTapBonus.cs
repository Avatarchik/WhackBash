using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripts.Logic
{
    public delegate void BonusCallback(bool bonus, int points);
    public class DoubleTapBonus
    {
        #region nested
        class ProgressController
        {
            private int Lit = 0;
            private int CurrentCap = 3; 
            private const int kMaxCircles = 9; 

            public int GetCirclesLitCount()
            {
                return Lit;
            }

            public int GetCap()
            {
                return CurrentCap;
            }

            public bool OnDoubletapped()
            {
                Lit = (++Lit >= CurrentCap) ? 0 : Lit;

                if (0 == Lit)
                { 
                    CurrentCap = (++CurrentCap >= kMaxCircles) ? kMaxCircles : CurrentCap; 
                }

                return 0 == Lit;
            }

			public void OnGameEnd()
			{
				Lit = 0; 
			}
        }
        #endregion

        public event BonusCallback OnBonusGiven;

        private ProgressController controller;
        private DateTime GameStarted = DateTime.Now;

        public DoubleTapBonus()
        {
            controller = new ProgressController();
        }

        public void CheckForTimeout()
        {
            var span = DateTime.Now - GameStarted;
            if (span.Hours >= 24)
            {
                controller = new ProgressController();
            }
        }

        public KeyValuePair<int, int> GetLitCapPair()
        {
            return new KeyValuePair<int, int>(controller.GetCirclesLitCount(),
                                              controller.GetCap());
        }

        public void OnDoubletapped(int pts)
        {
            bool flag = controller.OnDoubletapped();
            if (flag && OnBonusGiven != null)
            {
                OnBonusGiven(true, pts);
            }
            else
            {
                OnBonusGiven(false, pts);
            }
        }

		public void ResetLit()
		{
			controller.OnGameEnd();
		}
    }
}
