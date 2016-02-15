using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarCircle : MonoBehaviour
{
    public Color Selected;
    public Color Default;

    Image Back;
    
	// Use this for initialization
	void Start ()
    {
        Back = transform.GetChild(0).gameObject.GetComponent<Image>();
	}
	
	public void SetSelectedStatus(bool selected)
    { 
        if (null == Back) Start(); 
        Back.color = selected ? Selected : Default;
    }
}
