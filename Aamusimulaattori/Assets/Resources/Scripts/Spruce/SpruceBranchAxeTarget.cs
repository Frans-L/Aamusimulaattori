using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpruceBranchAxeTarget : MonoBehaviour, IAxeTargetHit {

    public int health = 2; //how many hits can take before falling down
    public AudioClip chopSound;

    AudioSource audioSource; //component that plays sounds

    //animation variables
    public float collapseSpeed = 1.5f;
    public float collapseAngularSpeed = 1.5f;
    public float collapseRandomMultiplier = 2f;
    public float collapseBaseHeight = 2.5f;

    public bool cutted = false;
    public String stumpExtensionTag = "StumpExtension";
    GameObject stumpExtension;

    //animation steps
    public List<float> collapseSizeMultiplier;
    public List<float> collapseStepDuration;

    public int enableStumpExtensionStep = 1; //when 
    public bool stumpExtensionEnabled = false;

    List<CollapseAnimationStep> collapseAnimation = new List<CollapseAnimationStep>();
    float animationTime = 0;
    int currentStep = 0;

    //for the shrinking animation
    Vector3 startSize, targetSize;
    bool Collapsing = false;

    // Use this for initialization
    void Start () {

        //find the stumpExtension, and save it
        //this object and stumpExtension are under same parent
        for (int i=0; i < transform.parent.transform.childCount; i++)
        {
            if(transform.parent.GetChild(i).tag == stumpExtensionTag)
            {
                stumpExtension = transform.parent.GetChild(i).gameObject;
                break;
            }
        }

        //create collapse animation
        for(int i=0; i < collapseStepDuration.Count; i++)
        {
            collapseAnimation.Add(new CollapseAnimationStep(collapseSizeMultiplier[i], collapseStepDuration[i]));
        }

        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {

        if (Collapsing)
        {
            UpdateCollapsing(); //update animation
        }

	}

    //when object is hitted
    public int HitAction(Collider col)
    {
        health--; //decrease the life

        audioSource.PlayOneShot(chopSound, 1f); //play sound

        if (health <= 0 && !cutted)
        {
            transform.parent = null; //deattach it from it parent

            //remove extra colliders
            if (GetComponent<MeshCollider>())
                Destroy(GetComponent<MeshCollider>()); 
            if (GetComponent<BoxCollider>())
                Destroy(GetComponent<BoxCollider>());

            GetComponent<CapsuleCollider>().enabled = true; //set main collider active

            GetComponent<Rigidbody>().isKinematic = false; //make rigidbody active
            cutted = true;

            Vector3 force = new Vector3(
                (UnityEngine.Random.value - 0.5f) * collapseRandomMultiplier,
                (UnityEngine.Random.value) * collapseRandomMultiplier + collapseBaseHeight,
                (UnityEngine.Random.value - 0.5f) * collapseRandomMultiplier);

            GetComponent<Rigidbody>().velocity = force * collapseSpeed; //add force to cutted tree

            force = new Vector3(
                (UnityEngine.Random.value - 0.5f) * collapseRandomMultiplier,
                (UnityEngine.Random.value - 0.5f) * collapseRandomMultiplier,
                (UnityEngine.Random.value - 0.5f) * collapseRandomMultiplier);

            GetComponent<Rigidbody>().angularVelocity = force * collapseAngularSpeed; //add force to cutted tree

            Collapsing = true;

            currentStep = 0;
            InitializeAnimationStep();

        }

        return 1; //successful
    }


    //updates the animation
    void UpdateCollapsing()
    {
        animationTime += Time.deltaTime;
        transform.localScale = Vector3.Lerp(startSize, targetSize, animationTime / collapseAnimation[currentStep].duration); //shrink the object

        if(animationTime >= collapseAnimation[currentStep].duration)
        {
            currentStep++;

            if (!stumpExtensionEnabled && currentStep == enableStumpExtensionStep) 
                EnableStumpExtension(); //active extension

            if (currentStep < collapseAnimation.Count)
            {
                InitializeAnimationStep();
            }
            else //animation is finished
            {
                EndAnimation();
            }
        }
    }


    //initializes the animation step
    void InitializeAnimationStep()
    {
        animationTime = 0;  //reset timer
        startSize = transform.localScale;
        targetSize = transform.localScale * collapseAnimation[currentStep].sizeMultiplier;
    }

    //stops the animation and activates stumpExtension's collisionbox
    void EndAnimation()
    {
        Collapsing = false;

        if (GetComponent<PickupAble>() != null)
            GetComponent<PickupAble>().enabled = true; //make this object pickuable
        if (GetComponent<Burnable>() != null)
            GetComponent<Burnable>().enabled = true; //and burnable

        Destroy(GetComponent<SpruceBranchAxeTarget>()); //destroy this component
    }

    //enables StumpExtension, actives right components
    void EnableStumpExtension()
    {
        if (stumpExtension.GetComponent<CapsuleCollider>() != null)
            stumpExtension.GetComponent<CapsuleCollider>().enabled = true; //active a collision box of the stumpExtension 
        if (stumpExtension.GetComponent<SpruceStumpAxeTarget>() != null)
            stumpExtension.GetComponent<SpruceStumpAxeTarget>().enabled = true; //make stump cuttable

        stumpExtensionEnabled = true;
    }

}


//one step of the collapse animation
public class CollapseAnimationStep
{
    public float sizeMultiplier;
    public float duration;

    public CollapseAnimationStep(float sizeMultiplier, float duration)
    {
        this.sizeMultiplier = sizeMultiplier;
        this.duration = duration;
    }
}
