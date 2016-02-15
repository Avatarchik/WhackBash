using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public delegate void CircleDeactivatedDelegate(Circle sender, CircleDeactivatedEventArgs args);

public class Circle : MonoBehaviour
{
    public event CircleDeactivatedDelegate OnCircleDeactivated;
#if UNITY_EDITOR
    public static float circleColorChangeTimedelay = 0.060f;
	//public static float circleColorChangeTimedelay = 10f;
#else
    public static float circleColorChangeTimedelay = 0.015f;
#endif
    public CircleCollider2D CircleCol;

    [Header("None = -1, up=0, left=1, down=2, right=3")]
    public int BlockedDirectionID = -1;

    private Ring ring;
    private FloatingScore floatingText;
    private bool pressed = false;

    private Color currentSircleColor;
    private SpriteRenderer currentRenderer;
    private Color requiredColor;
    private CircleState currentState = CircleState.Red;


    private GameObject shadow;
    private Vector3 shadowOriginalPosition;
    private Vector3 shadowOffset = new Vector2(0.05f, 0.05f);

    private DateTime spawnedAt;
    private DateTime tappedAt;

	private float origScale;
	private Vector2 quadLLCor;
	private Vector2 quadTRCor;

    private AudioSource tapSoundSource;

    public bool IsActivated { get; set; }

    public Color CurrentCircleColor
    {
        get
        {
            return this.currentSircleColor;
        }
        set
        {
            this.currentSircleColor = value;
        }
    }

    public Color RequiredColor
    {
        get
        {
            return requiredColor;
        }

        set
        {
            requiredColor = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        CircleCol = GetComponent<CircleCollider2D>();
        ring = GameObject.Instantiate(Resources.Load("Ring", typeof(Ring)), this.transform.position, Quaternion.identity) as Ring;
        floatingText = GameObject.Instantiate(Resources.Load("Score", typeof(FloatingScore)), this.transform.position, Quaternion.identity) as FloatingScore;
        currentRenderer = GetComponent<SpriteRenderer>();
        ring.circle = this;
        floatingText.Circle = this;
        ring.gameObject.SetActive(false);
        floatingText.gameObject.SetActive(false);

        tapSoundSource = this.GetComponent<AudioSource>();

        shadow = transform.GetChild(0).gameObject;
        shadowOriginalPosition = shadow.transform.position;

		if (GameController.IsHardMode) 
		{			
			origScale = gameObject.transform.localScale.x;
			GetComponent<SpriteRenderer> ().enabled = false;
			shadow.GetComponent<SpriteRenderer>().enabled = false;

			float llx, trx;
			float yll, ytr;

			if (name == "1" || name == "4" || name == "7") {
				llx = GameController.llCorner.x;
				trx = GameController.trCorner.x - ((GameController.boardWidth / 3f) * 2f);
			} else if (name == "2" || name == "5" || name == "8") {
				llx = GameController.llCorner.x + (GameController.boardWidth / 3f);
				trx = GameController.trCorner.x - (GameController.boardWidth / 3f);
			} else {
				llx = GameController.llCorner.x + ((GameController.boardWidth/3f)*2f);
				trx = GameController.trCorner.x;
			}

			if (name == "7" || name == "8" || name == "9") {
				yll = GameController.llCorner.y;
				ytr = GameController.trCorner.y - ((GameController.boardHeight / 3f)*2f);
			} else if (name == "4" || name == "5" || name == "6") {
				yll = GameController.llCorner.y + (GameController.boardHeight / 3f);
				ytr = GameController.trCorner.y - (GameController.boardHeight / 3f);
			} else {
				yll = GameController.llCorner.y + ((GameController.boardHeight / 3f)*2f);
				ytr = GameController.trCorner.y;
			}

			quadLLCor = new Vector2 (llx, yll);
			quadTRCor = new Vector2 (trx, ytr);
		}

        GameController.AddCircle(this);
    }
#if UNITY_EDITOR
    private void OnMouseDown()
    {
        if (CircleCol.enabled == true)
        {
            OnTapped();

//TODO            shadow.transform.position = shadowOriginalPosition + shadowOffset;
        }
    }

    private void OnMouseUp()
    {
//TODO        shadow.transform.position = shadowOriginalPosition;
    }

#endif
    private void Update()
    {
        bool isTouched = false;
        if (Input.touchCount > 0 && CircleCol.enabled == true)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                isTouched = TouchTest(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position));

                if (isTouched && !pressed && (Input.GetTouch(i).phase == TouchPhase.Began))
                {
                    OnTapped();
                    pressed = true;
                }
                if (isTouched)
                {
//TODO                    shadow.transform.position = shadowOriginalPosition + shadowOffset;
                }

                if (isTouched) break;
            }
        }

        if (!isTouched && pressed)
        {
            //OnTouchLeft();
//TODO            shadow.transform.position = shadowOriginalPosition;
            pressed = false;
        }

		if (Input.GetKey (KeyCode.Space)) {
			OnTapped();
		}

    }

    private bool TouchTest(Vector3 wp)
    {
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        return (CircleCol == Physics2D.OverlapPoint(touchPos));
    }

    private void OnTapped()
    {
        Debug.Log("Circle've been tapped! = " + name);

        if (this.IsActivated)
        {
            tapSoundSource.Play();
			this.DeactivateCircle();
        }
		else if (!GameController.IsHardMode) 
		{
			this.DeactivateCircle();
		}
    }
// 
//     public void ResetCircle()
//     {
//         this.currentState = CircleState.Red;
// 		this.IsActivated = false;
//         this.GetComponent<SpriteRenderer>().material.color = Color.white;
//         this.ring.gameObject.SetActive(false);
//     }

    public void ActivateCircle()
    {
		if (GameController.IsHardMode) 
		{
			shadow.GetComponent<SpriteRenderer> ().enabled = true;

			bool overlap;

			do {

				UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
				float scale = UnityEngine.Random.Range (origScale/2f, origScale);
				this.transform.localScale = new Vector3(scale, scale, 1f);
				CircleCol.enabled = true;

				UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
				//float x = UnityEngine.Random.Range (GameController.llCorner.x, GameController.trCorner.x);
				float x = UnityEngine.Random.Range (quadLLCor.x, quadTRCor.x);

				UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
				//float y = UnityEngine.Random.Range (GameController.llCorner.y, GameController.trCorner.y);
				float y = UnityEngine.Random.Range (quadLLCor.y, quadTRCor.y);
				transform.position = new Vector3 (x, y, -5f);

				if(CircleCol.bounds.min.x > GameController.llCorner.x && CircleCol.bounds.min.y > GameController.llCorner.y &&
				   CircleCol.bounds.max.x < GameController.trCorner.x && CircleCol.bounds.max.y < GameController.trCorner.y)
				{
					overlap = false;

					foreach (Circle circ in GameController.temporaryUnavailable.circles) 
					{												
						Bounds circBounds = circ.GetComponent<CircleCollider2D>().bounds;
						Vector3 min = circBounds.min;
						Vector3 max = circBounds.max;
						min.x -= (0.02f * GameController.boardWidth);
						max.x += (0.02f * GameController.boardWidth);

						min.y -= (0.02f * GameController.boardHeight);
						max.y += (0.02f * GameController.boardHeight);
						circBounds.SetMinMax(min, max);

						if (CircleCol.bounds.Intersects (circBounds)) 
						{
							overlap = true;
							break;
						}
					}
				}
				else
				{
					overlap = true;
				}

			} while (overlap);

			GetComponent<SpriteRenderer> ().enabled = true;
		}

		spawnedAt = DateTime.Now;
		this.RequiredColor = new Color(0.0f, 1.0f, 0.4f);
		this.currentState = CircleState.White;
		this.IsActivated = true;

        this.StartCoroutine("ActivateCircleCorountine");
    }

    public void SetToWhiteAndDisable()
    {
        currentRenderer.material.color = Color.white;
        this.currentState = CircleState.White;
        this.StopAllCoroutines();
    }

    public double GetReactionTimeMs()
    {
        return (tappedAt - spawnedAt).TotalMilliseconds;
    }

    public void DeactivateCircle()
    {
        tappedAt = DateTime.Now;
        this.IsActivated = false;

        this.StopAllCoroutines();

        var args = new CircleDeactivatedEventArgs();

        args.GainedPoints = (int)currentState;

        if (args.GainedPoints >= 1)
        {
			
            this.ring.gameObject.SetActive(true);
			this.ring.circle = this;
			this.ring.ResetRing();
            
            this.floatingText.gameObject.SetActive(true);
			this.floatingText.Score = args.GainedPoints;
			this.floatingText.ResetScore(BlockedDirectionID);
        }

        if (this.OnCircleDeactivated != null)
        {
            this.OnCircleDeactivated(this, args);
        }
        this.currentState = CircleState.Red;
        currentRenderer.material.color = Color.white;

		if (GameController.IsHardMode) 
		{
			GetComponent<SpriteRenderer> ().enabled = false;
			shadow.GetComponent<SpriteRenderer>().enabled = false;
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -1f);
		}
    }

    IEnumerator ActivateCircleCorountine()
    {
        const float totalFadeInT = 0.2f;
        float fadeInT = totalFadeInT;
        while (fadeInT > 0)
        {
            currentRenderer.material.color = Color.Lerp(Color.white, Color.green, (totalFadeInT - fadeInT)/ totalFadeInT);

            fadeInT -= Time.deltaTime;
            fadeInT = fadeInT < 0 ? 0 : fadeInT;

            yield return new WaitForEndOfFrame();
        }

        while (this.currentState != CircleState.Red)
        {
            this.currentSircleColor = currentRenderer.material.color;
            if (Math.Abs(currentSircleColor.r - RequiredColor.r) > 0.2f)
            {
                if (currentSircleColor.r > RequiredColor.r)
                {
                    currentSircleColor.r -= 0.05f;
                }
                else
                {
                    currentSircleColor.r += 0.05f;
                }
            }

            if (Math.Abs(currentSircleColor.b - RequiredColor.b) > 0.2f)
            {
                if (currentSircleColor.b > RequiredColor.b)
                {
                    currentSircleColor.b -= 0.05f;
                }
                else
                {
                    currentSircleColor.b += 0.05f;
                }
            }

            if (Math.Abs(currentSircleColor.g - RequiredColor.g) > 0.2f)
            {
                if (currentSircleColor.g > RequiredColor.g)
                {
                    currentSircleColor.g -= 0.05f;
                }
                else
                {
                    currentSircleColor.g += 0.05f;
                }
            }

            currentRenderer.material.color = this.currentSircleColor;

            if (Math.Abs(currentSircleColor.r - RequiredColor.r) < 0.2f 
                && Math.Abs(currentSircleColor.g - RequiredColor.g) < 0.2f 
                && Math.Abs(currentSircleColor.b - RequiredColor.b) < 0.2f)
            {
                SwitchCircleState();
            }

            yield return new WaitForSeconds(circleColorChangeTimedelay);
        }   
    }

    private void SwitchCircleState()
    {
        if (this.IsActivated)
        {
            switch (currentState)
            {
                case CircleState.White:
                    {
                        RequiredColor = Color.yellow;
                        currentState = CircleState.Green;
                        break;
                    }
                case CircleState.Green:
                    {
						RequiredColor = new Color(0.8f, 0.4f, 0.1f);
                        currentState = CircleState.Yellow;
                        break;
                    }
                case CircleState.Yellow:
					{
						RequiredColor = Color.red;
						currentState = CircleState.AlmostRed;
						break;
                    }
				case CircleState.AlmostRed:
					{
						
						currentState = CircleState.Red;
						DeactivateCircle();
						Debug.Log("Switched.");
						break;
					}
            }
        }
    }

    private enum CircleState
    {
        White = 4,
        Green = 3,
		Yellow = 2,
		AlmostRed = 1,
        Red = 0
    }
}

public class CircleDeactivatedEventArgs : EventArgs
{
    public const int kTapOutsideCode = 0;
    
    public int GainedPoints { get; set; }

    public bool IsEndOfGame
    {
        get { return GainedPoints <= 0; }
    }

    public bool TappedOutside
    {
        get { return GainedPoints == kTapOutsideCode; }
    }
}

