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
/// 

public class EquipmentAxe : MonoBehaviour, IEquipmentPreAction
{
    EquipmentAnimation slashAnim;
    EquipmentAnimation takeOnAnim, takeOffAnim;

    //Slash animation
    public List<Vector3> slashStepEulerAngle = new List<Vector3>();
    public List<float> slashStepDuration = new List<float>();

    public byte slashActionStep; //on which step SlashAction() is called, index starts from 0
    bool slashAction = false; //is slash action is done
    public String hitBoxTag;
    GameObject hitBox;


    //change-equipment animation
    public List<Vector3> takeOnStepEulerAngle = new List<Vector3>();
    public List<Vector3> takeOnStepPosition = new List<Vector3>();
    public List<float> takeOnStepDuration = new List<float>();

    public List<Vector3> takeOffStepEulerAngle = new List<Vector3>();
    public List<Vector3> takeOffStepPosition = new List<Vector3>();
    public List<float> takeOffStepDuration = new List<float>();



    // Use this for initialization
    void Start () {      
        
        //create slashing animation
        slashAnim = new EquipmentAnimation(gameObject, slashStepDuration.Count); 
        for(int i = 0; i < slashStepDuration.Count; i++)
        {
            slashAnim.AddStep(new EquipmentAnimationStep(
                GetComponent<Rigidbody>().transform.localPosition, //object position stays the same
                Quaternion.Euler(slashStepEulerAngle[i]), //rotate object
                slashStepDuration[i])); //step duration
        }

        hitBox = GameObject.FindWithTag(hitBoxTag); //find the object which handles the action (IEquipmentAction)


        //create on and off animation
        takeOnAnim = new EquipmentAnimation(gameObject, takeOnStepDuration.Count);
        for (int i = 0; i < takeOnStepDuration.Count; i++)
        {
            takeOnAnim.AddStep(new EquipmentAnimationStep(
                takeOnStepPosition[i],
                Quaternion.Euler(takeOnStepEulerAngle[i]),
                takeOnStepDuration[i]));
        }

        takeOffAnim = new EquipmentAnimation(gameObject, takeOffStepDuration.Count);
        for (int i = 0; i < takeOffStepDuration.Count; i++)
        {
            takeOffAnim.AddStep(new EquipmentAnimationStep(
                takeOffStepPosition[i],
                Quaternion.Euler(takeOffStepEulerAngle[i]),
                takeOffStepDuration[i]));
        }

        takeOffAnim.JumpToStep(1); //sets the starting coordinates

    }

    //when the equipment is used
    public float EquipmentUse()
    {

        if (!AnimationRunning())
        {
            slashAnim.Start();
            slashAction = false; //slash action isn't done yet
            return slashAnim.TotalDuration;
        }
        else
            return 0; //the operation was failed
    }

    //when the equipment is put away
    public float EquipmentTakeOff()
    {
        if (!AnimationRunning())
        {
            takeOffAnim.Start();
            return takeOnAnim.TotalDuration;
        }
        else
            return 0f; //the operation was failed
    }

    //when the equipment is put away
    public float EquipmentTakeOn()
    {
        if (!AnimationRunning())
        {
            takeOnAnim.Start();
            return takeOffAnim.TotalDuration;
        }
        else
            return 0; //the operation was failed
    }


    // Update is called once per frame
    void Update () {
        UpdateSlash();
        UpdateChangeEquipment();
    }

    //updates the slashing animation
    void UpdateSlash()
    {
        slashAnim.Update(); //if animation not running, nothing happens

        if (!slashAction && slashAnim.step == slashActionStep) //if it is the action moment (target is hitted)
        {
            hitBox.GetComponent<IEquipmentAction>().EquipmentAction();
            slashAction = true;
        }

    }

    //updates the change equipment animation
    void UpdateChangeEquipment()
    {
        takeOnAnim.Update(); //if animation not running, nothing happens
        takeOffAnim.Update();
    }

    //return true if one animation is running
    bool AnimationRunning()
    {
        return slashAnim.running || takeOnAnim.running || takeOffAnim.running;
    }

    public void SetActive()
    {
        GameObject.FindWithTag("EquipmentControl").GetComponent<EquipmentControl>().
            SelectEquipment(gameObject.GetComponent<IEquipmentPreAction>());
    }
}
