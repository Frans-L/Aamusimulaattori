using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionLost : MonoBehaviour {

    const float targetDistance = 21;
    const float targetTime = 24f;

    float timer = 0;

    GameObject player;

    // Use this for initialization
    void Start()
    {

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(player.transform.position,new Vector3(0,0,0)) > targetDistance)
        {
            timer += Time.deltaTime;

            if(timer >= targetTime)
            {
                MissionHandler.SetCompleted("MissionLost");
                timer = 0;
            }
            
        }
        else
        {
            timer = 0;
        }

    }
}
