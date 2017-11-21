using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAble : MonoBehaviour {


    public bool IsCarried { get; set; } //is carried at the moment
    public bool ForceDrop { get; set; }

    public float carryWeight = 5.0f;

    // Use this for initialization
    void Start () {
        IsCarried = false;
        ForceDrop = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
