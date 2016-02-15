using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HackForIPAD : MonoBehaviour 
{

	//public UnityEvent ONClick;
	// Use this for initialization
	void OnMouseDown() 
	{
		//ONClick.Invoke();
		Debug.Log("Starting new level.");
		
		Application.LoadLevelAsync("Game");
	}
}
