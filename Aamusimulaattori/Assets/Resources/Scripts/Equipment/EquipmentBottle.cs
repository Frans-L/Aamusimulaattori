using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

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

[RequireComponent(typeof(AudioSource))]
public class EquipmentBottle : MonoBehaviour, IEquipmentPreAction
{
    EquipmentAnimation drinkAnim;
    EquipmentAnimation takeOnAnim, takeOffAnim;

    //Slash animation
    public List<Vector3> drinkStepEulerAngle = new List<Vector3>();
    public List<Vector3> drinkStepPosition = new List<Vector3>();
    public List<float> drinkStepDuration = new List<float>();

    public byte drinkActionStep; //on which step SlashAction() is called, index starts from 0
    bool drinkAction = false; //is drink action is done

    //change-equipment animation
    public List<Vector3> takeOnStepEulerAngle = new List<Vector3>();
    public List<Vector3> takeOnStepPosition = new List<Vector3>();
    public List<float> takeOnStepDuration = new List<float>();

    public List<Vector3> takeOffStepEulerAngle = new List<Vector3>();
    public List<Vector3> takeOffStepPosition = new List<Vector3>();
    public List<float> takeOffStepDuration = new List<float>();

    public float drunkEffectIntroSpeed = 0.1f; //how fast per second the motion blur is introduced
    public float drunkEffectMaxDuration = 60f; //how long the drunk effect lasts
    public float drunkEffectDurationIncrement = 10f; //how much to add to the duration when used
    public float drunkEffectOutroSpeed = 0.1f; //how fast per second the motion blur is outroduced

    private float drunkEffectAmount = 0f;
    private float drunkEffectDuration = 0f;

    private PostProcessingBehaviour postProcess;
    private AudioSource audioSource;

    // Use this for initialization
    //creates animation from unity's UI data
    void Start()
    {
        postProcess = Camera.main.gameObject.GetComponent<PostProcessingBehaviour>();
        audioSource = GetComponent<AudioSource>();

        //create drink animation
        drinkAnim = new EquipmentAnimation(gameObject, drinkStepDuration.Count);
        for (int i = 0; i < drinkStepDuration.Count; i++)
        {
            drinkAnim.AddStep(new EquipmentAnimationStep(
                drinkStepPosition[i], //object position stays the same
                Quaternion.Euler(drinkStepEulerAngle[i]), //rotate object
                drinkStepDuration[i])); //step duration
        }

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
            drinkAnim.Start();
            drinkAction = false; //slash action isn't done yet
            return drinkAnim.TotalDuration;
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
    void Update()
    {
        UpdateDrink();
        UpdateChangeEquipment();
        UpdateBuff();
        UpdateMotionBlur();
    }

    void UpdateBuff()
    {
        if(drunkEffectDuration > 0)
        {
            drunkEffectAmount = Mathf.Min(1f, drunkEffectAmount + drunkEffectIntroSpeed * Time.deltaTime);
        }
        else
        {
            drunkEffectAmount = Mathf.Max(0f, drunkEffectAmount - drunkEffectIntroSpeed * Time.deltaTime);
        }
        drunkEffectDuration = Mathf.Max(0f, drunkEffectDuration - Time.deltaTime);
    }

    void UpdateMotionBlur()
    {
        if(drunkEffectAmount > 0)
        {
            postProcess.profile.motionBlur.enabled = true;
            postProcess.profile.motionBlur.settings = new MotionBlurModel.Settings()
            {
                shutterAngle = 270,
                sampleCount = 1,
                frameBlending = drunkEffectAmount,
            };
        }
        else
        {
            postProcess.profile.motionBlur.enabled = false;
        }
    }

    //updates the drinking animation
    void UpdateDrink()
    {
        drinkAnim.Update(); //if animation not running, nothing happens

        if (!drinkAction && drinkAnim.step == drinkActionStep) //if it is the action moment (target is hitted)
        {
            drinkAction = true;
            drunkEffectDuration = Mathf.Min(drunkEffectMaxDuration, drunkEffectDuration + drunkEffectDurationIncrement);

            if (drunkEffectDuration >= drunkEffectMaxDuration*0.75f) //all most max drunk
                MissionHandler.SetCompleted("MissionAlkoholi");

            audioSource.Play();
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
        return drinkAnim.running || takeOnAnim.running || takeOffAnim.running;
    }

    public void SetActive()
    {
        GameObject.FindWithTag("EquipmentControl").GetComponent<EquipmentControl>().
            SelectEquipment(gameObject.GetComponent<IEquipmentPreAction>());
    }
}
