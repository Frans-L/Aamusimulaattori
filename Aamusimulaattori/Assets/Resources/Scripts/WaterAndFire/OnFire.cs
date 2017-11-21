using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : MonoBehaviour {
	public GameObject OnFireParticles;

	// Use this for initialization
	void Start () {
		GameObject newObj = (GameObject)Instantiate(OnFireParticles);
		newObj.transform.parent = gameObject.transform;
		newObj.transform.localScale = gameObject.transform.lossyScale;
		newObj.transform.position = gameObject.transform.position;
		Material OnFireMaterial = Resources.Load ("Content/Generic/Materials/OnFire", typeof(Material)) as Material;
		Renderer rend = GetComponent<Renderer>();
		if (rend != null) {
			rend.material = OnFireMaterial;
		}

	}
}
