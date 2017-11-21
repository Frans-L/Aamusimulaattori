using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Always use this with the PickupAble
public class WaterSource : MonoBehaviour {

    public GameObject WaterSprinkle;
    GameObject newObj;

    void Start () {
        newObj = (GameObject)Instantiate(WaterSprinkle);
        newObj.transform.parent = gameObject.transform;
        newObj.transform.localScale = gameObject.transform.lossyScale;
        newObj.transform.position = gameObject.transform.position;
    }
    
    void Update()
    {
        //Turn on particle effect when picked up
        newObj.SetActive(!(gameObject.GetComponent<Rigidbody>().useGravity));
    }
}
