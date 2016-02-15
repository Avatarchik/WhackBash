using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelText : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GetComponent<Text>().text = string.Format("Level: {0}", GameController.Stages.CurrentStage.CurrentLevelIndexLocal + 1);
        StartCoroutine("Fade"); 
	}
	

    IEnumerator Fade()
    {
        for (float f = 0f; f <= 10; f += 0.1f)
        {
            Color c = GetComponent<Text>().color;
            c.a = f;
            GetComponent<Text>().color = c;
            yield return new WaitForSeconds(.1f);
        }
    }

}
