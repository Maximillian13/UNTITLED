using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollisionDetection : MonoBehaviour
{
	private BoxCollider col;
	private BoxInformation boxInfo;

	void Start()
	{
		col = this.transform.parent.GetComponent<BoxCollider>();
		boxInfo = this.transform.parent.GetComponent<BoxInformation>();
	}

	// Have them not collide if they are in the box and have them collide if they have left the box 
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Box")
		{
			if (col == null || boxInfo == null)
				return;
			if (boxInfo != null && (boxInfo.LeftTheStart == true || other.GetComponent<BoxInformation>().LeftTheStart == true || other.GetComponent<BoxInformation>().GetBoxType() == "Orange"))
				Physics.IgnoreCollision(col, other.GetComponent<Collider>(), false);
			else
				Physics.IgnoreCollision(col, other.GetComponent<Collider>(), true);
		}
	}
}
