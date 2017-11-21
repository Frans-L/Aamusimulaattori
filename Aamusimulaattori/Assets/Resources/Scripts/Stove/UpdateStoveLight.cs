using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStoveLight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Light>().intensity = BurnObject.intensity;
	}
}
