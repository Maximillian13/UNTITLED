using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackWhiteBoxProperties : MonoBehaviour, IBoxProperties
{
	// Get components 
	public AudioSource leaveSound;
	public AudioSource hum;
	public AudioSource desSound;
	private Rigidbody rig;
	private BoxCollider boxCol;
	private MeshRenderer mr;
	private SpriteRenderer[] numbersSprites;
	private List<BoxTimerInfo> dragList = new List<BoxTimerInfo>();

	public bool leftStartBox;

	private Vector3 startingPosition;
	private int counter;

	// For fading out
	private float duration = 2;
	private float t;
	private bool fading;

	private YellowBoxProperties connectedSticky;

	public bool black = true;
	public float strength = 230;
	public float distance = 5;
	private SphereCollider sCol;
	private List<Rigidbody> boxList = new List<Rigidbody>();

	private const float SCALE_FACTOR = 13;
	private const float DRAG_TIME = .4f;

	void OnEnable()
	{
		// Setting everything
		rig = this.GetComponent<Rigidbody>();
		mr = this.GetComponent<MeshRenderer>();
		boxCol = this.GetComponent<BoxCollider>();
		startingPosition = this.transform.position;

		sCol = this.GetComponent<SphereCollider>();
		sCol.radius = (distance / SCALE_FACTOR) / 2;

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
				this.DestroyBox(false);
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
			ms[1] = Resources.Load<Material>("Materials/Cube " + this.BoxTypeString());
			mr.materials = ms;

			fading = true;
			rig.useGravity = false;
			boxCol.enabled = false;
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

	private void Update()
	{
		// If in start box or dying 
		if (leftStartBox == false || fading == true) 
			return;

		// Gravity pull
		foreach (Rigidbody r in boxList)
		{
			if (r == null)
				continue;
			if(r.GetComponent<IBoxProperties>().Fading() == true)
				continue;

			float pull = Vector3.Distance(this.transform.position, r.transform.position);
			pull = Mathf.Abs(pull - distance) * strength;
			if (black == true)
				r.GetComponent<Rigidbody>().AddForce((this.transform.position - r.position) * pull * Time.deltaTime);
			else
				r.GetComponent<Rigidbody>().AddForce((r.position - this.transform.position) * pull * Time.deltaTime);
		}

		// Todo: This is lame because it will keep setting the drag to 0 for forever 
		// Drag timer
		dragList.RemoveAll(item => item.Cube == null); // Remove nulls with arrow function 🤮
		foreach (BoxTimerInfo bti in dragList)
		{
			bti.DragTimer -= Time.deltaTime;
			if (bti.DragTimer <= 0)
				bti.Cube.drag = 0;
		}
	}

	// On stay so we can capture boxes even if they start in the sphere 
	private void OnTriggerStay(Collider other)
	{
		if (leftStartBox == false)
			return;

		if (other.tag == "Box" && other != this)
		{
			if (other.GetComponent<IBoxProperties>().LeftStartBox() == true)
			{
				Rigidbody r = other.GetComponent<Rigidbody>();
				if (boxList.Contains(r) == false)
				{
					// Check if we are interacting with another black/white box. If we are 
					// check if its the opposite. If so return to ignore the opposite color 
					BlackWhiteBoxProperties boxProp = other.GetComponent<BlackWhiteBoxProperties>();
					if (boxProp != null && this.BoxTypeString() != boxProp.BoxTypeString())
						return;

					// Add to list that will effect gravity 
					boxList.Add(r);
					// Drag
					if (this.InBoxInfoList(r) == false)
					{
						// We dont want to mess with the drag of other black/white boxes 
						if (boxProp == null)
						{
							Debug.Log("Start Drag");
							dragList.Add(new BoxTimerInfo(r, DRAG_TIME));
							r.drag = 1;
						}
					}
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (leftStartBox == false)
			return;

		if (other.tag == "Box" && other != this)
		{
			Rigidbody r = other.GetComponent<Rigidbody>();
			boxList.Remove(r);
			int ind = this.FindBoxInfoInd(r);
			if (ind != -1)
			{ 
				r.drag = 0;
				dragList.RemoveAt(ind);
			}
		}
	}

	private int FindBoxInfoInd(Rigidbody r)
	{
		for(int i = 0; i < dragList.Count; i++)
		{
			if (dragList[i].Cube == r)
				return i;
		}
		return -1;
	}

	private bool InBoxInfoList(Rigidbody r)
	{
		// If we loop through and get -1 then that means its not in there 
		return this.FindBoxInfoInd(r) != -1;
	}

	public bool LeftStartBox()
	{
		return leftStartBox;
	}
	
	public string BoxTypeString()
	{
		return black == true ? "Black" : "White";
	}

	public bool Fading()
	{
		return fading;
	}
}

public class BoxTimerInfo
{
	public Rigidbody Cube { get; set; }
	public float DragTimer { get; set; }

	public BoxTimerInfo(Rigidbody cube, float dragTimer)
	{
		this.Cube = cube;
		this.DragTimer = dragTimer;
	}
}
