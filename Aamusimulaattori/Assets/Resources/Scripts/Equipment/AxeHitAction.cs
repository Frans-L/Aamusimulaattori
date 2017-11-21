using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The class EquipmentControl handles positions of equipments, which one is currently used  etc.
/// When the equipment is used
///     -> The class calls a method 'EquipmentUse' of 
///     the equipment's class that implements interface IEquipmentPreAction 
///     
/// A class, that implements interface IEquipmentPreAction handles, animations, delays etc.
/// When a preaction is done, for instace the axe is at the hit moment
///     -> The class calls a method 'EquipmentAction' of the
///     the equipment's class that implements interface IEquipmentAction
///     
/// A class, that implements interface IEquipmentAction handles the action
/// If the action targets at an object
///      -> The class calls a method 'HitAction' of the
///      the target's class that implements interface ITargetHit
///     
/// EquipmentControl -> IEquipmentPreAction -> IEquipmentAction ( -> ITargetHit )
/// </summary>


public class AxeHitAction : MonoBehaviour, IEquipmentAction {

    List<Collider> targetHitBox = new List<Collider>(); //if the list is empty, axe didn't hit anything

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider col)
    {
        if(!targetHitBox.Contains(col))
            targetHitBox.Add(col);
    }

    //remove the collider from the targetHitBox list
    private void OnTriggerExit(Collider col)
    {
        targetHitBox.Remove(col);
    }


    public int EquipmentAction()
    {
        foreach(Collider col in targetHitBox)
        {
            if (col != null && col.gameObject.GetComponent<IAxeTargetHit>() != null)
            {
                col.gameObject.GetComponent<IAxeTargetHit>().HitAction(col);
            }
        }

        return 1; //1 = succesful
    }


}


public interface IAxeTargetHit
{
    //returns 0 if not succesfull
    int HitAction(Collider col);
}
