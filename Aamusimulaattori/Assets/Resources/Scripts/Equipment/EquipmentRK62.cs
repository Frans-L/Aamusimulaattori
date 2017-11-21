using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentRK62 : MonoBehaviour, IEquipmentPreAction
{

    EquipmentAnimation takeOnAnim, takeOffAnim;

    public List<AudioClip> shootAudio;

    //change-equipment animation
    public List<Vector3> takeOnStepEulerAngle = new List<Vector3>();
    public List<Vector3> takeOnStepPosition = new List<Vector3>();
    public List<float> takeOnStepDuration = new List<float>();

    public List<Vector3> takeOffStepEulerAngle = new List<Vector3>();
    public List<Vector3> takeOffStepPosition = new List<Vector3>();
    public List<float> takeOffStepDuration = new List<float>();

    // Use this for initialization
    void Start () {

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
	
	// Update is called once per frame
	void Update () {
        UpdateChangeEquipment();
    }

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

    public float EquipmentTakeOn()
    {
        if (!AnimationRunning())
        {
            takeOnAnim.Start();
            return takeOnAnim.TotalDuration;
        }
        else
            return 0; //the operation was failed
    }

    public float EquipmentUse()
    {

        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.85f, 1.1f);
            GetComponent<AudioSource>().PlayOneShot(shootAudio[UnityEngine.Random.Range(0, shootAudio.Count)], 1f);
        }
            

        return 1;
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
        return takeOnAnim.running || takeOffAnim.running;
    }

    public void SetActive()
    {
        GameObject.FindWithTag("EquipmentControl").GetComponent<EquipmentControl>().
            SelectEquipment(gameObject.GetComponent<IEquipmentPreAction>()); //use this equipment
    }
}
