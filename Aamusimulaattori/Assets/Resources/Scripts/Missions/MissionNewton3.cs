using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNewton3 : MonoBehaviour {

    bool completed = false;
    const float targetHeight = 8;
    GameObject player;

	// Use this for initialization
	void Start () {

        player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
        if(!completed && player.transform.position.y >= targetHeight)
        {
            MissionHandler.SetCompleted("MissionNewton3");
            completed = true;
        }

    }
}
