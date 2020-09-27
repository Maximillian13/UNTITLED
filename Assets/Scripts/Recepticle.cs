using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Recepticle : MonoBehaviour
{
	//private float loadLevelTimer;
	public string boxTypeAccepted;
	private bool complete;
	private LevelControl lc;
	private bool instanceDoneAndWaiting;
	private float multiRecWaitTimer;
	private AudioSource clip;

	// For fading out
	private Material mat;
	private float duration = 2;
	private Color startCol;
	private Color endCol;
	float t;
	private bool fade;

	// Use this for initialization
	void Start ()
	{
		mat = this.GetComponent<MeshRenderer>().materials[0];
		startCol = mat.color;
		endCol = new Color(1, 1, 1, 1);
		lc = GameObject.Find("LevelControl").GetComponent<LevelControl>();
		clip = this.GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		// If the box hit the receptacle and it is complete then fade out 
		if (fade == true)
		{
			if (lc.GetMultiRec() == true && lc.GetMultiRecDone() == false)
			{
				if(multiRecWaitTimer >= 1.5f)
				{
					fade = false;
					instanceDoneAndWaiting = false;
					multiRecWaitTimer = 0;
				}
				multiRecWaitTimer += Time.deltaTime;
				return;
			}

			mat.color = Color.Lerp(startCol, endCol, t / duration);
			t += Time.deltaTime;
			if (t / duration >= 1.25) 
			{
				fade = false;
				complete = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Box" && other.GetType() == typeof(BoxCollider)) // If it is the box
		{
			// If the box is the one we want
			BoxInformation boxInfo = other.GetComponent<BoxInformation>();
			if (boxInfo.GetBoxType() == boxTypeAccepted)
			{
				clip.Play();
				// Remove any grip the player has on it
				InteractableObject io = other.GetComponent<InteractableObject>();
				if (io != null)
					io.EndInteraction();
				// Have it get rid of the box and make this recepticle fade out
				other.GetComponent<IBoxProperties>().DestroyBox(false);
				fade = true;

				if (lc.GetMultiRec() == true)
					instanceDoneAndWaiting = true;
			}
		}
	}

	/// <summary>
	/// If this receptacle is complete
	/// </summary>
	public bool Complete
	{
		get { return complete; }
		set { complete = value; }
	}

	public bool InstanceIsDoneAndWaiting
	{
		get { return instanceDoneAndWaiting; }
	}
}
