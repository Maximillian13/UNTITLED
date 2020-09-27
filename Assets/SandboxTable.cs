using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxTable : MonoBehaviour
{
	private Vector3[] boxPosition; // The original positions
	private Collider boxBlocker; // What will not let the box back in

	public SandboxTableCubeSpawner[] cubeSpawners;

	// Use this for initialization
	void Start()
	{
		// Set up everything 
		boxBlocker = this.transform.GetChild(0).GetComponent<Collider>();
	}

	// Add specified block if there is room for it, if not do nothing 
	public void SpawnInBox(string boxType)
	{
		foreach(SandboxTableCubeSpawner cs in cubeSpawners)
		{
			if(cs.Blocked == false)
			{
				Collider cube = Instantiate(Resources.Load<Collider>("Cube " + boxType), cs.transform.position, Quaternion.Euler(Vector3.zero));
				Physics.IgnoreCollision(boxBlocker, cube, true);
				return;
			}
		}
	}

	public void ClearAllCubeSpawners()
	{
		foreach (SandboxTableCubeSpawner s in cubeSpawners)
			s.Blocked = false;
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
		if (other.tag == "Box" && other.GetType() == typeof(BoxCollider)) // If its the box leaving
		{
			BoxInformation ab = other.GetComponent<BoxInformation>(); // Get the away box (ab)
			ab.LeftTheStart = true; // Show that the box has left the start area

			Physics.IgnoreCollision(other, boxBlocker, false); // Make it so the box cant go back in the table cover

			other.GetComponent<IBoxProperties>().ActivateProperties(false); // Activate the properties of the box so it acts like it should

			return; // Leave because we did what we needed to do
		}
	}
}
