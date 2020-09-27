using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    // For picking up
    private Rigidbody ridge;
	private TrailRenderer tr;
	private IBoxProperties boxProp;
    private bool currentlyInteracting;
    private WandControlGeneralInteraction attachedWand;
    private Transform interactionPoint;

    private float velocityFactor = 3500f;
    private Vector3 posDelta;

    private float rotationFactor = 100;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;

	private MeshRenderer mr;
	private Material cubeColor;
	private bool noTrail;


	// Use this for initialization
	void Start () 
    {
        // Set up
        mr = this.GetComponent<MeshRenderer>();
		cubeColor = mr.materials[1];
        ridge = this.GetComponent<Rigidbody>();
		tr = this.GetComponent<TrailRenderer>();
		boxProp = this.GetComponent<IBoxProperties>();
        interactionPoint = new GameObject().transform;
        velocityFactor = velocityFactor / ridge.mass;
        rotationFactor = rotationFactor / ridge.mass;
	}
	
	// Update is called once per frame
	void Update () 
    {
		if(ridge.IsSleeping() == true)
			ridge.WakeUp();

        // If you are holding something
	    if(currentlyInteracting == true)
        {
            this.KeepInHand(attachedWand);
			tr.enabled = false;
		}
		else
		{
			tr.enabled = !noTrail;
			float speed = Mathf.Abs(ridge.velocity.magnitude);
			if (speed > 3)
				tr.time = .5f;
			else
				tr.time = speed * .05f;

		}
	}

    private void KeepInHand(WandControlGeneralInteraction normWand)
    {
		// Set the position based on where the wand is
		posDelta = normWand.transform.position - interactionPoint.position;
        if (Mathf.Abs(posDelta.x) > .5f || Mathf.Abs(posDelta.y) > .5f || Mathf.Abs(posDelta.z) > .5f)
        {
            EndInteraction(normWand);
        }
        this.ridge.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;
        

        // Set the rotation based on where the wand is
        rotationDelta = normWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
        rotationDelta.ToAngleAxis(out angle, out axis);

        // Helps make it so it does not do a weird flip
        if (angle > 180)
        {
            angle -= 360;
        }


		if (float.IsNaN(angle) == true || float.IsNaN(axis.x) == true || float.IsNaN(this.ridge.angularVelocity.x) || float.IsNaN(rotationFactor) == true ||float.IsNaN(Time.fixedDeltaTime) == true ) 
		{
			Debug.Log("Gotcha bitch");

			EndInteraction();
			return;
		}

		this.ridge.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
    }

    // At the start of an interaction
    public void StartInteraction(WandControlGeneralInteraction wand)
    {
        // Attach the wand 
        attachedWand = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(this.transform, true);

		boxProp.OnBoxGrab();

        currentlyInteracting = true;
    }

    public void EndInteraction(WandControlGeneralInteraction wand)
    {
        if(wand == attachedWand) // So the other wand cant trigger this
        {
            // Detach wand
            attachedWand = null;
            currentlyInteracting = false;
			wand.DropBox();
			boxProp.OnBoxRelease(wand.transform);
		}

    }

	public void EndInteraction()
	{
		// Detach wand
		attachedWand = null;
		currentlyInteracting = false;
		if(boxProp != null)
			boxProp.OnBoxRelease(null);
	}

    // If its already interacting
    public bool IsInteracting()
    {
        return currentlyInteracting;
    }

	public Material GetCubeColor()
	{
		return cubeColor;
	}

	public void TurnOffTrail()
	{
		noTrail = true;
	}
}
