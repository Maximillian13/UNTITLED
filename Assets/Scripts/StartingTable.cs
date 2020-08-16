using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingTable : MonoBehaviour
{
	public BoxInformation[] boxes; // The original boxes that are in the scene before loading
	private Vector3[] boxPosition; // The original positions
	private BoxInformation[] awayBox; // The boxes that are outside the boxes
	private Collider boxBlocker; // What will not let the box back in

	// Use this for initialization
	void Start ()
	{
		// Set up everything 
		boxBlocker = this.transform.GetChild(0).GetComponent<Collider>();
		boxPosition = new Vector3[boxes.Length];
		awayBox = new BoxInformation[boxes.Length];
		for(int x = 0; x < boxPosition.Length; x++)
		{
			boxPosition[x] = boxes[x].transform.position;
			boxes[x].CreateKey();
			Physics.IgnoreCollision(boxes[x].GetComponent<Collider>(), boxBlocker, true); // Let the boxes be in the container
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Box" && other.GetType() == typeof(BoxCollider)) // If its the box leaving
		{
			if (other.GetComponent<BoxInformation>().LeftTheStart == true)
			{
				// Remove any grip the player has on it
				InteractableObject io = other.GetComponent<InteractableObject>();
				if (io != null)
					io.EndInteraction();
				other.GetComponent<IBoxProperties>().DestroyBox(true);
			}
		}
	}

	// When you take a box out
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Box" && other.GetType() == typeof(BoxCollider)) // If its the box leaving
		{
			for (int x = 0; x < boxes.Length; x++) // run through all the boxes
			{
				if (boxes[x].Key == other.gameObject.GetComponent<BoxInformation>().Key) // Once we found the right box
				{
					// If there is already one of these out on the field destroy the old one 
					if (awayBox[x] != null)
						if (other.GetComponent<BoxInformation>().Key == awayBox[x].Key)
							awayBox[x].GetComponent<IBoxProperties>().DestroyBox(true);

					// Create a new box and set it up
					GameObject rBGO = Instantiate(boxes[x].gameObject, boxPosition[x], Quaternion.Euler(Vector3.zero));
					Physics.IgnoreCollision(boxBlocker, rBGO.GetComponent<Collider>(), true);
					BoxInformation respawnedBox = rBGO.GetComponent<BoxInformation>();

					respawnedBox.GetComponent<IBoxProperties>().OnBoxRelease(null); // Set it as if the player is not holding it

					respawnedBox.Key = boxes[x].Key; // Set the key of the new box to the origianl one

					BoxInformation ab = other.GetComponent<BoxInformation>(); // Get the away box (ab)
					ab.LeftTheStart = true; // Show that the box has left the start area
					awayBox[x] = ab; // Set the away box as the box that is on the field

					boxes[x] = respawnedBox; // Set the new re-spawned box as the original

					Physics.IgnoreCollision(other, boxBlocker, false); // Make it so the box can go back in the table cover

					other.GetComponent<IBoxProperties>().ActivateProperties(false); // Activate the properties of the box so it acts like it should

					return; // Leave because we did what we needed to do
				}
			}
		}
	} 
}
