using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour {

    // Use this for initialization
    private float speed = 4.0f;
    public Text currentText;

    private void Update()
    {
        float aValue = currentText.color.a;
        
        currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b, (float)(Mathf.Sin(Time.time * speed) + 1.0) / 2.0f);
        
    }

   

}
