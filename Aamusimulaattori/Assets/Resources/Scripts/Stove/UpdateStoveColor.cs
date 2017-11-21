using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStoveColor : MonoBehaviour {

    public Color hotColor;
    public Color coldColor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(coldColor, hotColor, BurnObject.intensity);
	}
}
