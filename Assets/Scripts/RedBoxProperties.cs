using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBoxProperties : MonoBehaviour, IBoxProperties
{
	public AudioSource leaveSound;
	public AudioSource hum;
	public AudioSource desSound;
	public float force = 9.81f; // How much anti grav force should be applied 
	private Rigidbody rig;
	private Vector3 antiGrav;
	private BoxCollider boxCol;
	private bool active;
	private SpriteRenderer[] numbersSprites;

	private Vector3 startingPosition;
	private int counter;

	private bool leftStartBox;

	// For fading out the box
	private MeshRenderer mr;
	private float duration = 2;
	float t;

	private bool fading;

    private YellowBoxProperties connectedSticky;

    void OnEnable()
	{
		// Set everything up
		rig = this.GetComponent<Rigidbody>();
		antiGrav = new Vector3(0, force, 0);
		boxCol = this.GetComponent<BoxCollider>();
		mr = this.GetComponent<MeshRenderer>();
		active = false;
		startingPosition = this.transform.position;
		numbersSprites = this.GetComponentsInChildren<SpriteRenderer>();
	}

	void FixedUpdate()
	{
		// When it is out of the table cover start acting like an anti grav box
		if(active == true)
			rig.AddForce(antiGrav);

		// If the box is being destroyed have it fade out
		if (fading == true)
		{
			float a = Mathf.Lerp(1, 0, t / duration);
			mr.materials[0].color = new Color(mr.materials[0].color.r, mr.materials[0].color.g, mr.materials[0].color.b, a);
			mr.materials[1].color = new Color(mr.materials[1].color.r, mr.materials[1].color.g, mr.materials[1].color.b, a);
			for (int i = 0; i < numbersSprites.Length; i++)
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
	/// Activate the properties of the box
	/// </summary>
	public void ActivateProperties(bool dontPlaySound)
	{
		if (dontPlaySound == false)
		{
			leaveSound.Play();
			hum.Play();
		}
		leftStartBox = true;
		rig.useGravity = false;
		active = true;
	}


	/// <summary>
	/// Fades box out and destroys it 
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
			ms[1] = Resources.Load<Material>("Materials/Cube Red");
			mr.materials = ms;

			fading = true;
			rig.useGravity = false;
			boxCol.enabled = false;
			active = false;
			rig.velocity = Vector3.zero;
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
