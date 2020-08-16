using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPull : MonoBehaviour
{
	public float pullStrength = 1;
	private List<Rigidbody> boxList = new List<Rigidbody>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void Update()
	{
		foreach (Rigidbody r in boxList)
		{
			if(r == null)
				continue;
			float pull = Vector3.Distance(this.transform.position, r.transform.position);
			pull = Mathf.Abs(pull - 20) * pullStrength;
			r.transform.LookAt(this.transform);
			r.GetComponent<Rigidbody>().AddForce(r.transform.forward * pull);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Box")
			boxList.Add(other.GetComponent<Rigidbody>());
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag == "Box")
			boxList.Remove(other.GetComponent<Rigidbody>());
	}
}
