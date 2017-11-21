using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionScore : MonoBehaviour {

    public int[] scoreLimit = new int[5];
    public string[] missionTag = new string[5];

    int counter = 0;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        if (counter < scoreLimit.Length && Score.score >= scoreLimit[counter])
        {
            MissionHandler.SetCompleted(missionTag[counter]);
            counter++;
        }
		
	}
}
