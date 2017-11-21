using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour {

	public GameObject flashLight;

	void Update () {
		if (Input.GetButtonDown("FlashButton")) {
			flashLight.SetActive(!(flashLight.activeSelf));
		}
	}
}