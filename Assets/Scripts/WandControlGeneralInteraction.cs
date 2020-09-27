using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Valve.VR;

public class WandControlGeneralInteraction : MonoBehaviour
{
	//#region PickUpDrop
	//// ************************* For basic picking up and throwing behavior *************************
	//// Get the required buttons
	//private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	//   private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
	//private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;


	//// Understand which controller it is
	//private SteamVR_TrackedObject trackedObject;
	//   private SteamVR_Controller.Device controller
	//   {
	//       // Gives the index of the connected device
	//       get
	//       {
	//           return SteamVR_Controller.Input((int)trackedObject.index);
	//       }
	//   }

	//// ************************* For basic picking up and throwing behavior *************************
	//#endregion
	// This is a hash-set of all the objects that this wand is interacting with Hash-sets are like lists but with no duplicates inside of it
	private SteamVR_Input_Sources hand;
	private HashSet<InteractableObject> hoveredObject = new HashSet<InteractableObject>();
	private CubeButton button;
	private InteractableObject interactingItem;

	private Animator anim;
	private MeshRenderer mr;
	private Color originalColor;
	private Color cubeColor;
	private float colorLerp;

	public bool leftHand;
	
    // Set Up
    void Start()
    {
		anim = this.transform.GetChild(0).GetComponent<Animator>();

		// Set the tracked object
		if (leftHand == true)
			hand = SteamVR_Input_Sources.LeftHand;
		else
			hand = SteamVR_Input_Sources.RightHand;

		mr = this.transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
		originalColor = mr.materials[1].color;
	}

    void Update()
    {
		if (anim != null)
		{
			int controlerInd = leftHand == true ? 0 : 1;
			anim.SetFloat("Blend", SteamVR_Actions._default.TriggerPull.GetAxis(hand));
		}

		if (SteamVR_Actions._default.MenuButton.GetStateDown(hand))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		// If you press the trigger button
		if (SteamVR_Actions._default.TriggerClick.GetStateDown(hand))
        {
			// Calculate the minimum distance if there are multiple object you are interacting with
			interactingItem = this.GetClosestItem();

			// If you aren't holding anything
			this.CheckHoldIfNull();

            if (interactingItem != null)
            {
				interactingItem.StartInteraction(this);
				cubeColor = interactingItem.GetCubeColor().color;
			}
		}

		if (SteamVR_Actions._default.TriggerClick.GetStateDown(hand) && button != null)
			button.PressButton();

		// If you release the trigger button
		if (SteamVR_Actions._default.TriggerClick.GetStateUp(hand))
        {
			this.DropBox();
		}

		// Hand Color
		if (interactingItem != null)
		{
			if (colorLerp < 1)
			{
				colorLerp += Time.deltaTime * 8;
				mr.materials[1].color = Color.Lerp(originalColor, cubeColor, colorLerp);
			}
		}
		else
		{
			if (colorLerp > 0)
			{
				colorLerp -= Time.deltaTime * 8;
				mr.materials[1].color = Color.Lerp(originalColor, cubeColor, colorLerp);
			}
		}
	}

    // If you aren't holding anything
    private void CheckHoldIfNull()
    {
        // Check if you grabbed anything
        if (interactingItem != null)
        {
            // End interaction if you are all ready holding it with other hand
            if (interactingItem.IsInteracting() == true)
            {
				interactingItem.EndInteraction(this);
            }
        }
    }

	public void DropBox()
	{
		// Drop the item
		if (interactingItem != null)
		{
			mr.materials[1].color = originalColor;
			interactingItem.EndInteraction(this);
		}
		if (interactingItem != null)
		{
			interactingItem = null;
		}

		mr.materials[1].color = originalColor;
	}

	public void SetOrigTest()
	{
		mr.materials[1].color = originalColor;
	}

	// Calculate the minimum distance if there are multiple object you are interacting with
	private InteractableObject GetClosestItem()
    {
        float minDistance = float.PositiveInfinity;
        float distance;

		InteractableObject closes = null;

        foreach (InteractableObject item in hoveredObject)
        {
            if (item != null)
            {
                distance = (item.transform.position - this.transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
					closes = item;
                }
            }
        }

		return closes;
    }

    void OnTriggerEnter(Collider other)
    {
        // Find what its touching
        InteractableObject collidedObject = other.GetComponent<InteractableObject>();
        // Set it up to be carried
        if(collidedObject != null && other.GetType() != typeof(SphereCollider))
            hoveredObject.Add(collidedObject);
    }

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "BoxButton")
			button = other.GetComponent<CubeButton>();
	}

	void OnTriggerExit(Collider other)
    {
        // Find what its touching
        InteractableObject collidedObject = other.GetComponent<InteractableObject>();
        // Set it up to be dropped
        if (collidedObject != null && other.GetType() != typeof(SphereCollider))
        {
            // Set the color to be un-highlighted
            hoveredObject.Remove(collidedObject);
        }

		CubeButton cb = other.GetComponent<CubeButton>();
		if (cb != null)
		{
			button = null;
		}
	}
}
