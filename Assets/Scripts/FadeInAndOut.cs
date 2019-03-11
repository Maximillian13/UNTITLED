using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAndOut : MonoBehaviour {

    // Flips the sprites at the end of the game
	private SpriteRenderer sr;
    public Sprite[] sprites;
	private float timer;
    private int ind;
	// Use this for initialization
	void Start ()
	{
		sr = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        // after a second change the sprite of the disconnect
		timer += Time.deltaTime;
		if (timer >= 1)
		{
            sr.sprite = sprites[ind % 2];
            ind++;
			timer = 0;
		}
	}
}
