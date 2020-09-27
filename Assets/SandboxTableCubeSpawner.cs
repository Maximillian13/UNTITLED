using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxTableCubeSpawner : MonoBehaviour
{
	public bool Blocked { get; set; }

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Box" && other.GetType() == typeof(BoxCollider))
			Blocked = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Box" && other.GetType() == typeof(BoxCollider))
			Blocked = false;
	}
}
