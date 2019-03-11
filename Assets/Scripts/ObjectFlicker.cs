using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFlicker : MonoBehaviour
{
	public int chanceOfFliker;
	public bool singleObject;
	private MeshRenderer mesh;
	private MeshRenderer[] meshs;

	// Use this for initialization
	void Start ()
	{
		if (singleObject == true)
			mesh = this.GetComponent<MeshRenderer>();
		else
			meshs = this.GetComponentsInChildren<MeshRenderer>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
        // Make the objects flicker on and off
		if (Random.Range(0, chanceOfFliker + 1) == 0)
		{
			if (singleObject == true)
				mesh.enabled = !mesh.enabled;
			else
				for(int x = 0; x < meshs.Length; x++)
					meshs[x].enabled =!meshs[x].enabled;
		}
	}
}
