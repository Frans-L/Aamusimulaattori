using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    bool showMenu = false;
    bool showAchivements = false;

    GameObject uiMenu, uiAchievement;
    GameObject uiCredits, uiButtons;

	// Use this for initialization
	void Start () {
        uiMenu = gameObject;
        uiAchievement = GameObject.FindWithTag("UIAchievement");
        uiCredits = GameObject.FindWithTag("UICredits");
        uiButtons = GameObject.FindWithTag("UIButtons");

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("ShowMenu")){
           
            showMenu = !showMenu;

            if (showMenu)
            {
                DisablePlayer.MenuControls(true);           
            }
            else
            {
                DisablePlayer.MenuControls(false);
                Cursor.lockState = CursorLockMode.Locked; //lock mouse
            }
        }

        

        if (showMenu)
        {
            uiMenu.GetComponent<CanvasGroup>().alpha = 1f;
            uiAchievement.GetComponent<ShowAchievemetns>().ForceShow(true, showAchivements);
        }
        else //hide menu
        {
            uiMenu.GetComponent<CanvasGroup>().alpha = 0f;
            uiCredits.GetComponent<CanvasGroup>().alpha = 0f;
            uiButtons.GetComponent<CanvasGroup>().alpha = 0f;
            uiAchievement.GetComponent<ShowAchievemetns>().ForceShow(false);
            showAchivements = true;
        }
	}


    // continue button is pressed
    public void OnClickContinue()
    {
        Debug.Log("hei");
        showMenu = false;
        DisablePlayer.MenuControls(false);
    }


    // restart button is pressed
    public void OnClickRestart()
    {
        showMenu = false;
        DisablePlayer.MenuControls(false);

        GameLost.EndGame(true);
    }

    // restart button is pressed
    public void OnClickButtons()
    {
        showAchivements = false; //hide achviements
        uiCredits.GetComponent<CanvasGroup>().alpha = 0f; //hide credits
        uiButtons.GetComponent<CanvasGroup>().alpha = 1f; //show buttons
    }

    // Exit button is pressed
    public void OnClickExit()
    {
        Application.Quit();
        
    }

    // credits button is pressed
    public void OnClickCredits()
    {
        showAchivements = false; //hide achviements
        uiButtons.GetComponent<CanvasGroup>().alpha = 0f; //hide buttons
        uiCredits.GetComponent<CanvasGroup>().alpha = 1f; //show credits
    }
}
