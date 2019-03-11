using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WandControlGeneralInteraction : MonoBehaviour
{
    #region PickUpDrop
    // ************************* For basic picking up and throwing behavior *************************
    // Get the required buttons
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;


	// Understand which controller it is
	private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device controller
    {
        // Gives the index of the connected device
        get
        {
            return SteamVR_Controller.Input((int)trackedObject.index);
        }
    }

    // This is a hash-set of all the objects that this wand is interacting with Hash-sets are like lists but with no duplicates inside of it
    HashSet<InteractableObject> hoveredObject = new HashSet<InteractableObject>();
    InteractableObject closesyItem;
    InteractableObject interactingItem;
	// ************************* For basic picking up and throwing behavior *************************
	#endregion

	private Animator anim;
	private MeshRenderer mr;
	private Color originalMat;
	
    // Set Up
    void Start()
    {
		anim = this.transform.GetChild(0).GetComponent<Animator>();
		
        // Set the tracked object
        trackedObject = this.GetComponent<SteamVR_TrackedObject>();


		mr = this.transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
		originalMat = mr.materials[1].color;
	}

    void Update()
    {
        // If the controller is not yet set up
        if (controller == null)
        {
            return;
        }

		if (anim != null)
		{
			int controlerInd = (int)controller.index;
			anim.SetFloat("Blend", SteamVR_Controller.Input(controlerInd).GetAxis(triggerButton).x);
		}

		if (controller.GetPress(gripButton) && controller.GetPressDown(menuButton))
		{
			if (SceneManager.GetActiveScene().buildIndex > 0)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		}

		if (controller.GetPressDown(menuButton))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}


		#region PickUpDrop

		// If you press the trigger button
		if (controller.GetPressDown(triggerButton))
        {
            // Set hand pos to be clenched
            //handPos.SetHand(1);

            // Calculate the minimum distance if there are multiple object you are interacting with
            GetClosestItem();

            // Set the item that you are interacting with
            interactingItem = closesyItem;

            // If you aren't holding anything
            CheckHoldIfNull();

            if (interactingItem != null)
            {
                interactingItem.StartInteraction(this);
				mr.materials[1].color = interactingItem.GetCubeColor().color;
			}
        }

        // If you release the trigger button
        if (controller.GetPressUp(triggerButton))
        {
            // Set hand pos to be relaxed
            //handPos.SetHand(0);
            // Drop the item
            DropItem();
            if (interactingItem != null)
            {
                interactingItem = null;
                closesyItem = null;
            }

			mr.materials[1].color = originalMat;
		}
		#endregion
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

    // Drop the item
    private void DropItem()
    {
        if (interactingItem != null)
        {
            interactingItem.EndInteraction(this);
        }
    }

    // Calculate the minimum distance if there are multiple object you are interacting with
    private void GetClosestItem()
    {
        float minDistance = float.PositiveInfinity;
        float distance;

        foreach (InteractableObject item in hoveredObject)
        {
            if (item != null)
            {
                distance = (item.transform.position - this.transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closesyItem = item;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Find what its touching
        InteractableObject collidedObject = other.GetComponent<InteractableObject>();
        // Set it up to be carried
        if(collidedObject != null)
        {
            hoveredObject.Add(collidedObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Find what its touching
        InteractableObject collidedObject = other.GetComponent<InteractableObject>();
        // Set it up to be dropped
        if (collidedObject != null)
        {
            // Set the color to be un-highlighted
            hoveredObject.Remove(collidedObject);
        }
    }

    public SteamVR_Controller.Device GetController()
    {
        return controller;
    }

	public bool TestConnection()
	{
		if (controller.valid == false || trackedObject.isActiveAndEnabled == false || controller.hasTracking == false || controller.connected == false || trackedObject.isValid == false || controller.outOfRange == true || trackedObject.index == SteamVR_TrackedObject.EIndex.None)
			return false;
		else
			return true;
	}
	public bool GetControllerIsActive()
	{
		return trackedObject.isActiveAndEnabled;
	}

	public bool GetTracking()
	{
		return controller.hasTracking;
	}

	public bool GetConnected()
	{
		return controller.connected;
	}
}
