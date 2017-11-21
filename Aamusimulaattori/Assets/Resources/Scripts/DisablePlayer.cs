using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePlayer : MonoBehaviour {

    static GameObject player, equipmentControl;

	// Use this for initialization
	void Start () {
        player = gameObject; //static methos cannot use gameObject
        equipmentControl = GameObject.FindWithTag("EquipmentControl");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void MenuControls(bool active)
    {
        DisableMovement(active); //disable movement
        DisableEquipmentControl(active); //disable equipment control
        SetMouseLock(!active); //disable mouse lock
    }

    public static void SplashScreenControls(bool active)
    {
        DisableMovement(!active); //disable movement
        DisableEquipmentControl(active); //disable equipment control
        SetMouseLock(active); //mouse lock on
    }

    public static void DisableMovement(bool disabled)
    {
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = !disabled;
    }

    public static void DisableEquipmentControl(bool disabled)
    {
        equipmentControl.GetComponent<EquipmentControl>().SetActive(!disabled);
    }

    public static void DisableEquipment(bool disabled)
    {
        equipmentControl.SetActive(!disabled );
    }

    public static void SetMouseLock(bool locked)
    {
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().SetMouseLock(locked);
    }

    public static void DisableCollision(bool disabled)
    {
        player.GetComponent<CharacterController>().enabled = !disabled;
    }
}
