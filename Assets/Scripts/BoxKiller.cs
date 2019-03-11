using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxKiller : MonoBehaviour
{
	private LastLevelControl llc;
	// Use this for initialization
	void Start ()
    {
		llc = GameObject.Find("LastLevelControl").GetComponent<LastLevelControl>();
	}
	

    // When the player tries to take out the box on the second to last level, kill box, kill music, set it up to go to the next level
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Box")
		{
            GameObject music = GameObject.Find("Music");
            if (music != null)
                Destroy(music.gameObject);
            InteractableObject io = other.GetComponent<InteractableObject>();
			if (io != null)
				io.EndInteraction();
			other.GetComponent<IBoxProperties>().DestroyBox(true);
			llc.CubeKilled();
		}
	}
}
