using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy itself immediately
/// </summary>
public class DebugSelfDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
