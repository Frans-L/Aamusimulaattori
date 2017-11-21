using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairSize : MonoBehaviour {

    bool action = false;

    Vector3 normalScale;
    public Vector3 actionScale = new Vector3(1.1f, 1.1f, 1);

	// Use this for initialization
	void Start () {
        normalScale = gameObject.GetComponent<RectTransform>().localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (action)
        {
            gameObject.GetComponent<RectTransform>().localScale = actionScale;
            action = false;
        }
        else{
            gameObject.GetComponent<RectTransform>().localScale = normalScale;
        }
	}

    //action size only one frame time
    public void ToActionSize()
    {
        action = true;
    }

}
