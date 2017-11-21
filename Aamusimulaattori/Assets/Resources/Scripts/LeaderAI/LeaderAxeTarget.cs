using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeaderAxeTarget : MonoBehaviour, IAxeTargetHit {

    public int health = 3;

    public AudioClip hitSound;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int HitAction(Collider col)
    {
        health--;
        GetComponent<AudioSource>().PlayOneShot(hitSound);

        if(health <= 0)
        {
            Destroy(GetComponent<LeaderMove>());
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(GetChild(gameObject, "Leader").GetComponent<Animator>());

            GetComponent<Rigidbody>().isKinematic = false; //make it active again
            GetComponent<PickupAble>().enabled = true;
            GetComponent<Burnable>().enabled = true;

            GetComponent<Rigidbody>().angularVelocity = new Vector3(-150, -150); //make sure leader falls

            Destroy(GetComponent<LeaderAxeTarget>());
        }

        return 1; //succeed
    }


    static GameObject GetChild(GameObject o, string name)
    {
        for (int i = 0; i < o.transform.childCount; i++)
        {
            if (o.transform.GetChild(i).name == name)
                return o.transform.GetChild(i).gameObject;
        }

        return null;
    }
}
