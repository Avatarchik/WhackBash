using UnityEngine;
using System.Collections;

public class BackgroundTouchListener : MonoBehaviour
{

    // Use this for initialization
    public event CircleDeactivatedDelegate OnBackgroundTouch;

    public BoxCollider2D box;

    void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

    public void MakeItRed()
    {
        this.GetComponent<UnityEngine.UI.Image>().color = new Color(0.9f, 0.369f, 0.227f);
    }

    bool pressed = false;
    RaycastHit2D hit = new RaycastHit2D();
    private void FixedUpdate()
    {
        bool isTouched = false;
        if (Input.touchCount > 0 && box.enabled)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var pos = (Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position)); 
                hit = Physics2D.Raycast(pos, new Vector2(0, 0), 40);

                isTouched = hit.collider == box;

                if (isTouched && !pressed && (Input.GetTouch(i).phase == TouchPhase.Began))
                {
                    //MakeItRed();
                    this.OnBackgroundTouch(null, new CircleDeactivatedEventArgs {
                        GainedPoints = CircleDeactivatedEventArgs.kTapOutsideCode
                    });

                    box.enabled = false;
                    Debug.LogWarning("Background was touched");
                }

                if (isTouched) break;
            }
        }

        if (!isTouched && pressed)
        {
            //OnTouchLeft();
        }
    }

#if UNITY_EDITOR
    void OnMouseDown()
    {
        if (!box.enabled)
        {
            return;
        }

       // this.GetComponent<UnityEngine.UI.Image>().color =  new Color(0.9f, 0.369f, 0.227f);
        this.OnBackgroundTouch(null, new CircleDeactivatedEventArgs
        {
            GainedPoints = CircleDeactivatedEventArgs.kTapOutsideCode
        }); 

        box.enabled = false;
        Debug.LogWarning("Background was touched");
    }
#endif
}
