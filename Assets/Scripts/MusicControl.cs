using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
	// Dont destroy the music until the end 
	void Start ()
	{
		DontDestroyOnLoad(this.gameObject);	
	}
	
}
