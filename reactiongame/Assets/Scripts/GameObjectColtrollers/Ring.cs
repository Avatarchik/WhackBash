using UnityEngine;
using System.Collections;

public class Ring : MonoBehaviour
{

    // Use this for initialization
    public Circle circle;
    public SpriteRenderer currentRenderer; 

	public void ResetRing ()
    {
       // this.currentRenderer = this.GetComponent<SpriteRenderer>();
        currentRenderer.material.color = this.circle.GetComponent<SpriteRenderer>().material.color;        
		transform.position = circle.transform.position;
		transform.localScale = new Vector3(0.2f, 0.2f);
		//transform.localScale = circle.transform.localScale;
    }

    private void OnDisable()
    {
        this.StopAllCoroutines();
    }

    private void Update()
    {
        currentRenderer.material.color = new Color(currentRenderer.material.color.r, currentRenderer.material.color.g, currentRenderer.material.color.b, currentRenderer.material.color.a - 0.045f);
        transform.localScale = new Vector3(transform.localScale.x + 0.01f, transform.localScale.y + 0.01f);

        if (currentRenderer.material.color.a <= 0)
        {
            gameObject.SetActive(false);
            //this.circle.ResetCircle();
        }
    }
}
