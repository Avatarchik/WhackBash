using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PNode : MonoBehaviour {

	public Sprite spriteDone;
	public Sprite spriteNotDone;
	public Sprite spriteEnd;

	private Image image;
	private Color32 colorDone = new Color(0.82352941176471f, 0.9921568627451f, 0.82745098039216f);
	private Color32 colorCurrent = new Color(0.56470588235294f, 1f, 0.9843137254902f);
	private Color32 colorEnd = new Color(0.93333333333333f, 1f, 0.69803921568627f);

	// Use this for initialization
	void Start ()
	{
		image = transform.GetChild(0).GetComponent<Image>();

		int currentStage = GameController.Stages.CurrentStage.CurrentLevelIndexLocal + 1;
		int nodeNum = int.Parse(name);

		if (nodeNum == 10) 
		{
			image.sprite = spriteEnd;
			GetComponent<Image>().color = colorEnd;
			transform.localScale = 1.3f * Vector3.one;
		}
		else if (nodeNum < currentStage)
		{
			image.sprite = spriteDone;
			image.color = colorDone;
			transform.localScale = Vector3.one;
		}
		else if (nodeNum == currentStage) 
		{
			image.sprite = spriteNotDone;
			image.color = colorCurrent;
			image.transform.localScale = 1.2f * Vector3.one;
			transform.localScale = 1.3f * Vector3.one;
		}
		else 
		{
			image.sprite = spriteNotDone;
			image.color = Color.white;
			transform.localScale = 0.8f * Vector3.one;
		}
	}
}
