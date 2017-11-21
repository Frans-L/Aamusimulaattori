using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateImageColor : MonoBehaviour {

    Image img;

	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        img.color = new Color(BurnObject.uiColor.r, BurnObject.uiColor.g, BurnObject.uiColor.b, img.color.a);
    }
}
