using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAchievemetns : MonoBehaviour {

    public bool force = false; //if control is forced
    public bool forceShow = true; //the forced result, is shown or not

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!force)
        {
            if (Input.GetButton("ShowAchievements"))
                GetComponent<CanvasGroup>().alpha = 1;
            else
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        else //control is forced
        {
            if(forceShow)
                GetComponent<CanvasGroup>().alpha = 1;
            else
                GetComponent<CanvasGroup>().alpha = 0;
        }       
        

	}

    //forces showing achievements on, or off
    public void ForceShow(bool show, bool forceShow = true)
    {
        force = show;
        this.forceShow = forceShow;
    }
}
