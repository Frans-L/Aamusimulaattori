using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTextColor : MonoBehaviour {

    Text txt; 

	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        txt.color = new Color(BurnObject.uiColor.r, BurnObject.uiColor.g, BurnObject.uiColor.b, txt.color.a);
    }
}
