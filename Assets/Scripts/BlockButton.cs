using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockButton : MonoBehaviour
{
	// The things that the button will remove
	public Material diffBackMat;
	public GameObject[] thingsToGetRidOf;
	public bool[] startOff;

	private AudioSource buttonHitSound;

	// Components to get
	private BoxCollider[] boxCols;
	private MeshRenderer[] mr;

	private MeshRenderer mesh;
	private Material backMat;
	private Color originalBackMat;

	// For fading out
	private float duration = 1;
	private Color[] startCol;
	private Color[] endCol;
	private float[] t;
	private bool[] fadeOut;
	private bool[] fadeIn;
	private bool[] buttonOn;

	private bool changeCol;
	private int buttonPressCounter;
	private float colorT;

	private float buttonAcceptanceDelay;

	// Use this for initialization
	void Start()
	{
		// Set up the arrays
		boxCols = new BoxCollider[thingsToGetRidOf.Length];
		mr = new MeshRenderer[thingsToGetRidOf.Length];
		startCol = new Color[thingsToGetRidOf.Length];
		endCol = new Color[thingsToGetRidOf.Length];
		buttonOn = new bool[startOff.Length];
		fadeIn = new bool[startOff.Length];
		fadeOut = new bool[startOff.Length];
		t = new float[startOff.Length];

		buttonHitSound = this.GetComponent<AudioSource>();

		mesh = this.GetComponent<MeshRenderer>();
		backMat = mesh.materials[0];
		originalBackMat = backMat.color;
		changeCol = false;
		buttonPressCounter = 0;

		// Set all components for each blocker
		for (int x = 0; x < thingsToGetRidOf.Length; x++)
		{
			boxCols[x] = thingsToGetRidOf[x].GetComponent<BoxCollider>();
			mr[x] = thingsToGetRidOf[x].GetComponent<MeshRenderer>();
			startCol[x] = mr[x].material.color;
			endCol[x] = new Color(startCol[x].r, startCol[x].g, startCol[x].b, 0);
			fadeOut[x] = false;
			t[x] = 0;
		}
		buttonAcceptanceDelay = 0;

		for (int x = 0; x < thingsToGetRidOf.Length; x++)
		{
			if (startOff[x] == true)
			{
				mr[x].material.color = endCol[x];
				boxCols[x].enabled = false;
				buttonOn[x] = true;
			}
		}
	}

	void FixedUpdate()
	{
		// Fade out all the boxes 
		for (int x = 0; x < thingsToGetRidOf.Length; x++)
		{
			if (fadeOut[x] == true)
			{
				t[x] += Time.deltaTime;
				mr[x].material.color = Color.Lerp(startCol[x], endCol[x], t[x] / duration);

				if (mr[x].material.color.a <= endCol[x].a)
				{
					fadeOut[x] = false;
					t[x] = 0;
					break;
				}
			}
		}

		// Fade out all the boxes 
		for (int x = 0; x < thingsToGetRidOf.Length; x++)
		{
			if (fadeIn[x] == true && fadeOut[x] == false)
			{
				t[x] += Time.deltaTime;
				mr[x].material.color = Color.Lerp(endCol[x], startCol[x], t[x] / duration);

				if (mr[x].material.color.a >= startCol[x].a)
				{
					fadeIn[x] = false;
					t[x] = 0;
					break;
				}
			}
		}

		if(changeCol == true)
		{
			colorT += Time.deltaTime;

			// Change to the other ones 
			if (buttonPressCounter % 2 == 0)
				backMat.color = Color.Lerp(diffBackMat.color, originalBackMat, colorT / duration); // Change to second color
			else
				backMat.color = Color.Lerp(originalBackMat, diffBackMat.color, colorT / duration); // Change to first color

			// Change things back after the color has changed 
			if (colorT / duration >= 1)
			{
				colorT = 0;
				changeCol = false;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// If a box hits it make it so all the boxes turn off and start to fade out
		if (other.tag == "Box")
		{
			if (Time.time > buttonAcceptanceDelay)
			{
				buttonHitSound.Play();
				for (int x = 0; x < thingsToGetRidOf.Length; x++)
				{
					if (buttonOn[x] == false)
					{
						if (boxCols[x] != null)
							boxCols[x].enabled = false;
						fadeOut[x] = true;
					}
					if (buttonOn[x] == true)
					{
						if (boxCols[x] != null)
							boxCols[x].enabled = true;
						fadeIn[x] = true;
					}
					buttonOn[x] = !buttonOn[x];
				}
				buttonAcceptanceDelay = Time.time + 1;
				changeCol = true;
				buttonPressCounter++;
			}
		}
	}
}
