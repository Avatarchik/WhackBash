using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TapsToGoText : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        this.GetComponent<Text>().text =  GameController.TapsLeft.ToString();
    }
}
