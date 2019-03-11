using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Box")
		{
			Rigidbody rig = other.GetComponent<Rigidbody>();
			rig.velocity = new Vector3(-rig.velocity.x, -rig.velocity.y, -rig.velocity.z);
		}
	}
}
