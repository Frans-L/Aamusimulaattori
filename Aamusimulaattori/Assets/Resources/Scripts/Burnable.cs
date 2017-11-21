using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Burnable : MonoBehaviour {

    public float sizeMultiplier = 1.0f; //multiplies the size changes of each step
    public float durationMultiplier = 1.0f; //multiplies the duration of each step

    public float burningOffset = 0.1f; //max random distance from the animation rally points
    public float fuelAmount = 5.0f; //the temperature increase when burned
    public float cooldownSpeedIncrease = 1f; //the cooldownSpeed increase when burned --> game becomes harder
    public bool destroyOnBurn = true; //should this object be destroyed after burning or not

    float scoreValue; //the amount of score added when burned

    Vector3 startSize, targetSize; //needed for the size change animation
    Vector3 startPos, targetPos; //needed for the rallypoint animation

    float animationTime; //to calclute lerp

    GameObject targetStove; //object which has BurnObject component, from a parameter of Burn() method

    bool burning = false;
    int currentStep = 0; //current rallypoint step

    // Note: rallyPoints and rallyPointSpeed are got from the parameters of the Burn method
    // So in future, there could be multiple stoves, and each have their own animation
    float rallyPointSpeed; //speed of the animation
    List<BurnAnimationStep> burnAnimation = new List<BurnAnimationStep>(); //animation rallyPoints

    // Use this for initialization
    void Start () {
        scoreValue = (int) (fuelAmount * 2);
    }

	// Update is called once per frame
	void Update () {
        if (burning)
        {
            UpdateBurn();
        }
	}


    //updtes the burning animation
    void UpdateBurn()
    {
        GetComponent<Rigidbody>().isKinematic = true; //doesn't react to any forces
        animationTime += Time.deltaTime / durationMultiplier;

        transform.localScale = Vector3.Lerp(startSize, targetSize, animationTime / burnAnimation[currentStep].duration); //change size
        GetComponent<Rigidbody>().position = Vector3.Lerp(startPos, targetPos, animationTime / burnAnimation[currentStep].duration); //move position

        if(animationTime >= burnAnimation[currentStep].duration)
        {
            currentStep++;
            if (currentStep < burnAnimation.Count){
                InitializeAnimationStep();
            }
            else //animation is finished
            {
                EndAnimation();
            }

        }


    }

    //what happens when the animatione ends
    void EndAnimation()
    {
        targetStove.GetComponent<BurnObject>().AddFuel(fuelAmount);
        targetStove.GetComponent<BurnObject>().IncreaseCooldownSpeed(cooldownSpeedIncrease);
        Score.AddScore(scoreValue);

        if(destroyOnBurn) Destroy(gameObject);
        else enabled = false;
    }

    //starts the burning animation
    public void Burn(List<BurnAnimationStep> burnAnimation, GameObject targetStove)
    {
        if (burning == false)
        {
            this.burnAnimation = burnAnimation;
            burning = true;
            if(GetComponent<PickupAble>() != null)
            {
                GetComponent<PickupAble>().ForceDrop = true;
            }

			InitializeAnimationStep(); //initialize the
            this.targetStove = targetStove; //which object has AddFuel method
        }

    }


    //initializes the variables for the next animation step
    void InitializeAnimationStep()
    {
        animationTime = 0; //starts the step timer back to 0

        startSize = transform.localScale; //orginal size
        targetSize = startSize * burnAnimation[currentStep].sizeMultiplier * sizeMultiplier; //target size

        startPos = GetComponent<Rigidbody>().position; //orginal size
        float x = (UnityEngine.Random.value * 2 - 1) * burningOffset; // value between [-offset,offset]
        float z = (UnityEngine.Random.value * 2 - 1) * burningOffset;

        targetPos = burnAnimation[currentStep].rallyPoint + new Vector3(x,0,z); //target siz
    }

}
