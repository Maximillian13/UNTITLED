using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlueBoxProperties : MonoBehaviour, IBoxProperties
{
	// Get components 
	public AudioSource leaveSound;
	public AudioSource hum;
	public AudioSource desSound;
	private Rigidbody rig;
	private BoxCollider boxCol;
	private MeshRenderer mr;

	public Transform hands;
	private bool active;
	private float comeBackTimer;

	private float strengthMultiplyer = 1.5f;
	private float timer = 3;
	private Vector3 startingPosition;
	private int counter;

	// For fading out
	private float duration = 2;
	private float t;
	private bool fade;

    private YellowBoxProperties connectedSticky;

    void OnEnable()
	{
		// Setting everything
		rig = this.GetComponent<Rigidbody>();
		mr = this.GetComponent<MeshRenderer>();
		boxCol = this.GetComponent<BoxCollider>();
		startingPosition = this.transform.position;
		//hands = GameObject.FindWithTag("Hands").transform;
	}

	void FixedUpdate()
	{
		if(active == true)
		{
			if (hands != null) // If it has a hand to come back to
			{
				Vector3 direction = hands.position - this.transform.position;
				this.rig.MovePosition(this.transform.position + direction * strengthMultiplyer * Time.deltaTime);

				if (Time.time > comeBackTimer + timer) // Se the velocity back to 0 so the cube can return to the player
				{
					rig.velocity = Vector3.zero;
					comeBackTimer = float.PositiveInfinity;
				}
			}
		}

		// Fade out and destroy self
		if (fade == true)
		{
			float a = Mathf.Lerp(1, 0, t / duration);
			mr.materials[0].color = new Color(mr.materials[0].color.r, mr.materials[0].color.g, mr.materials[0].color.b, a);
			mr.materials[1].color = new Color(mr.materials[1].color.r, mr.materials[1].color.g, mr.materials[1].color.b, a);

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
		active = true;
	}

	/// <summary>
	/// Fade out and destroy box
	/// </summary>
	public void DestroyBox(bool playSound)
	{
        if (connectedSticky != null)
            connectedSticky.DestroyFJConnections();
        if (fade == false)
		{
			if (playSound == true)
				desSound.Play();

            InteractableObject io = this.GetComponent<InteractableObject>();
            if (io != null)
                this.GetComponent<InteractableObject>().EndInteraction();

			Material[] ms = new Material[2];
			ms[0] = Resources.Load<Material>("Materials/Cube Rim");
			ms[1] = Resources.Load<Material>("Materials/Cube LightBlue");
			mr.materials = ms;

			fade = true;
			rig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			boxCol.enabled = false;
			rig.velocity = Vector3.zero;
			rig.AddTorque(new Vector3(UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000)));
		}
	}

    // When the player released the cube have the hand set to the hand they threw it with
	public void OnBoxRelease(Transform wand)
	{
		if (wand != null)
			hands = wand;
		comeBackTimer = Time.time;
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
}
