using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkBoxProperties : MonoBehaviour, IBoxProperties
{
	public float force = 9.81f; // How much anti grav force should be applied 
	public AudioSource leaveSound;
	public AudioSource hum;
	public AudioSource desSound;
	private Rigidbody rig;
	private BoxCollider boxCol;
	private bool active;
	private SpriteRenderer[] numbersSprites;

	private Vector3 startingPosition;
	private int counter;

	private bool leftStartBox;

	// For fading out the box
	private MeshRenderer pointerBoxMR;
	private Material pointerMat;
	private MeshRenderer mr;
	private float duration = 2;
	float t;

	private bool fading;

    private YellowBoxProperties connectedSticky;

    void OnEnable()
	{
		// Set everything up
		rig = this.GetComponent<Rigidbody>();
		boxCol = this.GetComponent<BoxCollider>();
		mr = this.GetComponent<MeshRenderer>();
		active = false;
		startingPosition = this.transform.position;
		pointerBoxMR = this.transform.GetChild(0).GetComponent<MeshRenderer>();
		pointerMat = Resources.Load<Material>("Materials/Cube Pointer");
		//ActivateProperties(true);
		numbersSprites = this.GetComponentsInChildren<SpriteRenderer>();
	}

	void FixedUpdate()
	{
		// When it is out of the table cover start acting like an anti grav box
		if (active == true)
		{
			rig.AddForce(this.transform.up * force);
		}

		// If the box is being destroyed have it fade out
		if (fading == true)
		{
			float a = Mathf.Lerp(1, 0, t / duration);
			mr.materials[0].color = new Color(mr.materials[0].color.r, mr.materials[0].color.g, mr.materials[0].color.b, a);
			mr.materials[1].color = new Color(mr.materials[1].color.r, mr.materials[1].color.g, mr.materials[1].color.b, a);
			pointerBoxMR.material.color = new Color(pointerBoxMR.material.color.r, pointerBoxMR.material.color.b, pointerBoxMR.material.color.b, a);
			rig.useGravity = false;
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
			ms[1] = Resources.Load<Material>("Materials/Cube Pink");
			mr.materials = ms;
			pointerBoxMR.material = Resources.Load<Material>("Materials/Cube Pointer Fade");

			fading = true;
			boxCol.enabled = false;
			active = false;
			rig.velocity = Vector3.zero;
			rig.AddTorque(new Vector3(UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000)));
		}
	}

    // Show guide rails
	public void OnBoxGrab()
	{
		Transform oldPointer = this.transform.Find("Pointer");
		if (oldPointer == null)
		{
			GameObject pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pointer.GetComponent<BoxCollider>().enabled = false;
			pointer.name = "Pointer";
			pointer.transform.rotation = this.transform.rotation;
			pointer.transform.localScale = new Vector3(.005f, 10, .005f);
			pointer.transform.parent = this.transform;
			pointer.transform.localPosition = new Vector3(0, .38f, 0);
			pointer.GetComponent<MeshRenderer>().material = pointerMat;
		}
	}

    // Hide guide rails 
	public void OnBoxRelease(Transform wand)
	{
		Transform pointer = this.transform.Find("Pointer");
		if (pointer != null)
			Destroy(pointer.gameObject);
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
