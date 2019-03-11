using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInformation : MonoBehaviour
{
	// A unique key used for identifying what box should be cloned/destroyed 
	private int key;
	public string boxType;
	private bool leftTheStart;

	void Start()
	{
		leftTheStart = false;
	}

	/// <summary>
	/// Creates a unique key the cube (This will be used to respawn cubes)
	/// </summary>
	public void CreateKey()
	{
		key = this.gameObject.GetHashCode();
	}

	/// <summary>
	/// Get or set a unique key used for identifying what box should be cloned/destroyed 
	/// </summary>
	public int Key
	{
		get { return key; }
		set { key = value; }
	}

	/// <summary>
	/// What type of box it is
	/// </summary>
	/// <returns>The color represented as a string</returns>
	public string GetBoxType()
	{
		return boxType;
	}

	/// <summary>
	/// If the box has left the starting area
	/// </summary>
	public bool LeftTheStart
	{
		get { return leftTheStart; }
		set { leftTheStart = value; }
	}
}
