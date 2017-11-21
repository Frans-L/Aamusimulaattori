using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopGroundPlayer : MonoBehaviour {

    public string groundTag = "Ground"; //object with the GroundData component
    GroundData ground;

    float mirrorDistMaxX; //when further -> thiis object is teleported
    float mirrorDistMaxZ;

    int missionTeleportCount = 0; //rintamakarkuri tehtävää varten

    // Use this for initialization
    void Start () {
        ground = GameObject.FindWithTag(groundTag).GetComponent<GroundData>();
        mirrorDistMaxX = ground.mapLengthX / 2;
        mirrorDistMaxZ = ground.mapLengthZ / 2;

    }

    // Update is called once per frame
    void Update()
    {

        if (Mathf.Abs(gameObject.transform.position.x) > mirrorDistMaxX)
        {
            TeleportAxisX(gameObject); //teleport the player 
            List<GameObject> carried = gameObject.GetComponent<PickupObject>().CarriedObject();
            foreach(GameObject o in carried)
            {
                TeleportAxisX(o); //teleport the carried object
            }

            missionTeleportCount++;
        }

        if (Mathf.Abs(gameObject.transform.position.z) > mirrorDistMaxZ)
        {
            TeleportAxisZ(gameObject); //teleport the player 
            List<GameObject> carried = gameObject.GetComponent<PickupObject>().CarriedObject();
            foreach (GameObject o in carried)
            {
                TeleportAxisZ(o); //teleport the carried object
            }

            missionTeleportCount++;
        }

        if ((missionTeleportCount-1)%10 == 0) //mission completed when 1st time teleports, and each 10th time
        {
            MissionHandler.SetCompleted("MissionRintamaKarkuri");
            missionTeleportCount++;
        }

            
    }

    void TeleportAxisX(GameObject o)
    {
        if (o != null)
        {
            Vector3 pos = o.transform.position;
            float direction = pos.x / Mathf.Abs(pos.x);
            o.transform.position = new Vector3(pos.x - ground.mapLengthX * direction, pos.y, pos.z); //teleport the object
        }

    }

    void TeleportAxisZ(GameObject o)
    {
        if (o != null)
        {
            Vector3 pos = o.transform.position;
            float direction = pos.z / Mathf.Abs(pos.z);
            o.transform.position = new Vector3(pos.x, pos.y, pos.z - ground.mapLengthZ * direction); //teleport the object
        }

    }
}
