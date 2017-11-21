using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMissions : MonoBehaviour {

    public new List<string> name = new List<string>();
    public new List<string> tag = new List<string>();

	// Use this for initialization
	void Start () {
		for(int i=0; i<name.Count; i++)
        {
            MissionHandler.AddMission(name[i], tag[i], i);
        }

        MissionHandler.StartHotSpot();
    }


    GameObject GetChildGameObject(GameObject o, string name)
    {
        for (int i = 0; i < o.transform.childCount; i++)
        {
            if (o.transform.GetChild(i).name == name)
                return o.transform.GetChild(i).gameObject;
        }

        return null;
    }


}
