using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBoxProperties : MonoBehaviour, IBoxProperties
{
    // Sound
	public AudioSource leaveSound;
	public AudioSource hum;
	public AudioSource desSound;

	private BoxCollider stickyBox; // The trigger box collider that will be used to stick to stuff
	private Rigidbody rig;
	private BoxCollider boxCol;
	private InteractableObject io;
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

    private List<FixedJoint> fjs = new List<FixedJoint>();

	void OnEnable()
	{
		// Set everything up
		rig = this.GetComponent<Rigidbody>();
		boxCol = this.GetComponent<BoxCollider>();
		stickyBox = this.transform.GetChild(0).GetComponent<BoxCollider>();
		io = this.GetComponent<InteractableObject>();
		mr = this.GetComponent<MeshRenderer>();
		stickyBox.enabled = false;
		startingPosition = this.transform.position;
		numbersSprites = this.GetComponentsInChildren<SpriteRenderer>();
	}

	void FixedUpdate()
	{
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


    // If it hits a wall or a cube
	void OnTriggerEnter(Collider other)
	{
		if (active == true)
		{
			if ((other.tag == "Untagged" || other.tag == "Box") && other.GetType() == typeof(BoxCollider))
			{
				if (other.GetComponent<BoxBlocker>() == null) // Do not stick to the table
				{
                    // If it is a cube connect to that rigid body if not just connect to the world (null)
					FixedJoint fj = this.gameObject.AddComponent<FixedJoint>();
					Rigidbody rb = other.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.GetComponent<IBoxProperties>().ConnectedToSticky(this);
                        fj.connectedBody = rb;
                    }
                    else
                        fj.connectedBody = null;

					fjs.Add(fj); // adds to list so it can be destroyed later 
				}
			}

		}
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
		stickyBox.enabled = true;
		active = true;
	}


	/// <summary>
	/// Fades box out and destroys it 
	/// </summary>
	public void DestroyBox(bool playSound)
	{
		if (fading == false)
		{
            if (connectedSticky != null) // If there it is connected to sticky get rid of connection
                connectedSticky.DestroyFJConnections();
            if (playSound == true)
				desSound.Play();
			DestroyFJConnections();
			io.TurnOffTrail();
            io.EndInteraction();
			io.enabled = false;

			Material[] ms = new Material[2];
			ms[0] = Resources.Load<Material>("Materials/Cube Rim");
			ms[1] = Resources.Load<Material>("Materials/Cube Yellow");
			mr.materials = ms;

			fading = true;
			rig.useGravity = false;
			boxCol.enabled = false;
			stickyBox.enabled = false;
			rig.velocity = Vector3.zero;
			rig.AddTorque(new Vector3(UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000), UnityEngine.Random.Range(-5000, 5000)));
		}
	}

    // Destroy all Fixed joid connections that this cube has
	public void DestroyFJConnections()
	{
		if (fjs.Count > 0)
		{
			FixedJoint[] fja = fjs.ToArray();
			for (int x = 0; x < fja.Length; x++)
			{
				Destroy(fja[x]);
			}
		}
	}

	public void OnBoxGrab()
	{
		return;
	}

	public void OnBoxRelease(Transform wand)
	{
		return;
	}

    // Set if this is connected to a sticky block
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
