using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBoxProperties : MonoBehaviour, IBoxProperties
{
	// Get components 
	public AudioSource leaveSound;
	public AudioSource hum;
	public AudioSource desSound;
	private Rigidbody rig;
	private BoxCollider boxCol;
	private MeshRenderer mr;
	private SpriteRenderer[] numbersSprites;

	private bool leftStartBox;

	private Vector3 startingPosition;
	private int counter;

	// For fading out
	private float duration = 2;
	private float t;
	private bool fading;

    private YellowBoxProperties connectedSticky;

    void OnEnable()
	{
		// Setting everything
		rig = this.GetComponent<Rigidbody>();
		mr = this.GetComponent<MeshRenderer>();
		boxCol = this.GetComponent<BoxCollider>();
		startingPosition = this.transform.position;

		numbersSprites = this.GetComponentsInChildren<SpriteRenderer>();
	}

	void FixedUpdate()
	{
		// Fade out and destroy self
		if (fading == true)
		{
			float a = Mathf.Lerp(1, 0, t / duration);
			mr.materials[0].color = new Color(mr.materials[0].color.r, mr.materials[0].color.g, mr.materials[0].color.b, a);
			mr.materials[1].color = new Color(mr.materials[1].color.r, mr.materials[1].color.g, mr.materials[1].color.b, a);
			for(int i = 0; i < numbersSprites.Length; i++)
			{
				if (numbersSprites[i] != null)
					numbersSprites[i].color = new Color(1, 1, 1, a);
			}

			t += Time.deltaTime;
			if (t / duration >= 1.3f) 
				Destroy(this.gameObject);
		}

		// If the box gets really far away destroy it to reduce system stress
		if (counter == 60)
		{
			float curDist = Vector3.Distance(startingPosition, this.transform.position);
			if (curDist > 200)
				DestroyBox(false);
			counter = 0;
		}
		counter++;
	}

	/// <summary>
	/// Activates the property of the box (No gravity)
	/// </summary>
	public void ActivateProperties(bool dontPlaySound = false)
	{
		if (dontPlaySound == false)
		{
			leaveSound.Play();
			hum.Play();
		}
		rig.useGravity = false;
		leftStartBox = true;
	}

	/// <summary>
	/// Fade out and destroy box
	/// </summary>
	public void DestroyBox(bool playSound)
	{
		if (fading == false)
		{
            if (connectedSticky != null)
                connectedSticky.DestroyFJConnections();
            if (playSound == true)
				desSound.Play();
            InteractableObject io = this.GetComponent<InteractableObject>();
			if (io != null)
			{
				io.EndInteraction();
				io.TurnOffTrail();
			}

			Material[] ms = new Material[2];
			ms[0] = Resources.Load<Material>("Materials/Cube Rim");
			ms[1] = Resources.Load<Material>("Materials/Cube Blue");
			mr.materials = ms;

			fading = true;
			boxCol.enabled = false;
			rig.velocity = Vector3.zero;
			rig.useGravity = false;
			rig.AddTorque(new Vector3(UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000)));
		}
	}

	public void OnBoxRelease(Transform wand)
	{
		return;
	}

	public void OnBoxGrab()
	{
		return;
	}

    // Set if there is a sticky block connected to this cube
    public void ConnectedToSticky(YellowBoxProperties ybp)
    {
        connectedSticky = ybp;
    }

	public bool LeftStartBox()
	{
		return leftStartBox;
	}

	public bool Fading()
	{
		return fading;
	}
}
