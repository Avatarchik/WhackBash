using System; 
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;

namespace Scripts
{
    public class CirclesBarScript : MonoBehaviour
    {
        List<BarCircle> circles = new List<BarCircle>();

        void Start()
        { 
            circles = new List<BarCircle>(); 
            foreach(Transform ch in transform) 
            {
                circles.Add(ch.gameObject.GetComponent<BarCircle>());
            }
        }

        public void UpdateBar(int lit, int cap)
        { 
            if (circles.Count == 0) Start();
             
            for (int i = 0; i < circles.Count; ++i)
            {
                circles[i].SetSelectedStatus(i < lit);
                circles[i].gameObject.SetActive(i < cap);
            }
        } 



        public IEnumerator ResetCor(int cap)
        {
            UpdateBar(cap, cap-1);
            yield return new WaitForSeconds(0.4f);
			var bonus = GameController.Bonus.GetLitCapPair();
			UpdateBar(bonus.Key, bonus.Value);
        }
    }
}
