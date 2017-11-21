using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAxeTarget : MonoBehaviour, ITargetHit {

    public String LogMainCopyTag = "MasterLog";

    GameObject masterLog;

    // Use this for initialization
    void Start () {

        GameObject gm = GameObject.FindWithTag("ObjectMasters");

        masterLog = gm.transform.GetChild(0).gameObject; //Just for TESTING, not final
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int HitAction(Collider col)
    {

        //masterLog.SetActive(true);
        GameObject log = Instantiate(masterLog);
        //masterLog.SetActive(false);

        Vector3 direction = new Vector3(UnityEngine.Random.value*2-1.0f, 0, UnityEngine.Random.value * 2 - 1.0f);

        log.transform.position = gameObject.transform.position + Vector3.up * 2.5f + direction * 1.5f;
        log.SetActive(true);
        log.GetComponent<Rigidbody>().velocity = direction * 4f;


        return 1; //operation successful
    }


}
