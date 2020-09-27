using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecailRecepticle : MonoBehaviour
{
	// The level to load
	public int levelToLoad;
	public bool specailLevels;

	// For fading out
	private Material mat;
	private float duration = 1;
	private Color startCol;
	private Color endCol;
	float t;
	private bool fade;

	private AudioSource clip;

	private float fadingWhiteTimer;
	private EyeFadeControl eyeFade;


	// Use this for initialization
	void Start()
	{
		mat = this.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0];
		startCol = mat.color;
		endCol = new Color(startCol.r, startCol.g, startCol.b, 0);
		clip = this.GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		// If the box hit the receptacle and it is complete then fade out 
		if (fade == true)
		{
			mat.color = Color.Lerp(startCol, endCol, t / duration);
			t += Time.deltaTime;
			if (mat.color.a <= 0)
			{
				// Get and start fade
				if (eyeFade == null)
				{
					eyeFade = GameObject.Find("[CameraRig]").transform.Find("Camera").Find("EyeCover").GetComponent<EyeFadeControl>();
					eyeFade.FadeWhite();
				}

				// After fade load level
				fadingWhiteTimer += Time.deltaTime;
				if (fadingWhiteTimer > .3f)
				{
					if (specailLevels == false)
					{
						SceneManager.LoadScene(levelToLoad);
					}
					else
					{
						if(levelToLoad == -1)
							SceneManager.LoadScene("Sandbox");
						else
							SceneManager.LoadScene("S" + levelToLoad);
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Box") // If it is the box
		{
			clip.Play();

			// Remove any grip the player has on it
			InteractableObject io = other.GetComponent<InteractableObject>();
			if (io != null)
				io.EndInteraction();
			// Have it get rid of the box and make this recepticle fade out
			other.GetComponent<IBoxProperties>().DestroyBox(false);
			fade = true;
		}
	}
}
