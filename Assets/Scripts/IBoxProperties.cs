using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoxProperties
{
	// Every box will have these methods in them, wish i could inherent but life's not perfect 
	void ActivateProperties(bool dontPlaySound);
	void DestroyBox(bool playSound);
	void OnBoxRelease(Transform wand);
	void OnBoxGrab();
    void ConnectedToSticky(YellowBoxProperties ybp);
	bool LeftStartBox();
	bool Fading();
}
