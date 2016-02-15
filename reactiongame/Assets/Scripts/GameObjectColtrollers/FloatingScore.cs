using UnityEngine;
using System.Collections;


public class FloatingScore : MonoBehaviour
{

    public Circle Circle;
    public int Score;
    private Vector3 randomisedDirection;

    public void ResetScore(int blockedDirection = -1)
    {
        // todo ------------------------------------------------------
        var c = this.Circle.CurrentCircleColor;
        c.r += 0.2f;
        c.b -= 0.1f;
        this.GetComponent<TextMesh>().color =c;
        this.GetComponent<TextMesh>().text = string.Format("+{0}", this.Score);
		this.transform.position = this.Circle.transform.position - new Vector3(0.46f, -0.22f, 0);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -6.0f);
        // to be above the circle

        while (true)
        {
            var randomValue = new System.Random().Next(3);
            if (blockedDirection == randomValue)
            {
                continue;
            }

            switch (randomValue)
            {
                case 0:
                    {
                        this.randomisedDirection = Vector3.up;
                        break;
                    }
                case 2:
                    {
                        this.randomisedDirection = Vector3.down;
                        break;
                    }
                case 3:
                    {
                        this.randomisedDirection = Vector3.right;
                        break;
                    }
                default:
                    {
                        this.randomisedDirection = Vector3.left;
                        break;
                    }
            }
            break;
        }
    }



    void Update()
    {
        var currentColor = this.GetComponent<TextMesh>().color;
        if (currentColor.a > 0)
        {
            this.GetComponent<TextMesh>().color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a - 0.01f);
            transform.Translate(this.randomisedDirection * Time.deltaTime, Camera.main.transform);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
