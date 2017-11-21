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

public class EquipmentControl : MonoBehaviour {


    bool active = true;
    GameObject mainCamera;

    List<IEquipmentPreAction> inventoryEquipment = new List<IEquipmentPreAction>(); //equipments in inventory
    List<IEquipmentPreAction> currentEquipment = new List<IEquipmentPreAction>(); //currently used one
    List<IEquipmentPreAction> nextEquipment = new List<IEquipmentPreAction>(); //next one (future one)

    bool changingEquipment = false;
    
    // Use this for initialization
    void Start () {
        mainCamera = GameObject.FindWithTag("MainCamera");
    }
	
	// Update is called once per frame
	void Update () {

        //Rotates only X axes because Equipment object has common Z and Y axes
        //with the mainCamera because both are childs of the Player
        gameObject.transform.rotation = mainCamera.transform.rotation;

        if (active)
        {
            if (Input.GetButton("ChangeEquipment"))
            {
                if (!changingEquipment && inventoryEquipment.Count > 0)
                {
                    PreSelectNextWeapon();
                    StartCoroutine(ChangeEquipment());
                }

            }

            //using equipment
            if (Input.GetButton("UseEquipment"))
            {
                UseEquipment();
            }
        }
	}


    //nextEquipment = player has selected these weapons to be his next ones,
    //player will use these equipments as soon as the ChangeEquipment routine is called
    void PreSelectNextWeapon()
    {
        nextEquipment.Clear();
        nextEquipment.Add(inventoryEquipment[0]); //coming equipment
    }


    IEnumerator ChangeEquipment(bool forceChange = false)
    {
        changingEquipment = true;

        float waitingTime = 0 ;
        waitingTime = TakeOffEquipment(); //put current equipment away

        if(forceChange && waitingTime <= 0) //forcechange is on, and 1st try failed
        {
            int i = 0;
            while(i < 10) //try 10 times to change weapong
            {
                waitingTime = TakeOffEquipment();
                if (waitingTime <= 0)
                    yield return new WaitForSeconds(0.25f); //if failed, sleep 250 milliseconds
                else
                    break;   
            }
        }

        if(waitingTime > 0)
        {
            yield return new WaitForSeconds(waitingTime +0.1f); //wait animation duration

            waitingTime = TakeOnEquipment() + 0.1f; //take next equipment to use
            yield return new WaitForSeconds(waitingTime +0.1f);
 
        }

        changingEquipment = false;
    }

    //currentEquipment = player using them at the moment
    //nextEquipment = player has selected these weapons to be his next ones,
    //                player will use these equipments as soon as the change animation is over
    //inventoryEquipment = player has these equipments but doesn't use them
    //put selected equipments back to inventory
    //reeturn coming animation duration
    public float TakeOffEquipment()
    {

        if (currentEquipment.Count == 0) return 0.1f; //takeoff succeeded, since there was nothing to take off

        float animationTime = 0; //greatest animation time
        for (int i = currentEquipment.Count - 1; i >= 0; i--)
        {
            IEquipmentPreAction eq = currentEquipment[i];
            float time = eq.EquipmentTakeOff();
            animationTime = Mathf.Max(time, animationTime);
            if (time != 0) //if succesfull{
            {       
                inventoryEquipment.Add(eq); //add it back to inventory
                currentEquipment.Remove(eq); //remove it from use
            }         
        }
    
        return animationTime;
    }

    //currentEquipment = player using them at the moment
    //nextEquipment = player has selected these weapons to be his next ones,
    //                player will use these equipments as soon as the change animation is over
    //inventoryEquipment = player has these equipments but doesn't use them
    //put selected equipments back to inventory
    //reeturn coming animation duratio
    public float TakeOnEquipment()
    {
        float animationTime = 0; //greatest animation time
        foreach (IEquipmentPreAction eq in nextEquipment)
        {
            float time = eq.EquipmentTakeOn();
            animationTime = Mathf.Max(time, animationTime);
            if (animationTime != 0) //if successful{
            {
                currentEquipment.Add(eq); //add it to use
                inventoryEquipment.Remove(eq); //remove it from inventory
            }
                
        }

        return animationTime;
    }


    //call EquipmentUse mehtod of the every selected equipment
    public void UseEquipment()
    {
        //use all selected equipments
        foreach (IEquipmentPreAction eq in currentEquipment)
        {
            eq.EquipmentUse();
        }
    }


    //changes the current equipment
    public void SelectEquipment(IEquipmentPreAction eq, bool keepPreviousEquipment = false)
    {
        nextEquipment.Clear();
        nextEquipment.Add(eq);
        StartCoroutine(ChangeEquipment(true));

    }

    //changes the current equipment
    public void AddToInventory(IEquipmentPreAction eq)
    {
        inventoryEquipment.Add(eq);
    }


    //activates controls
    public void SetActive(bool active)
    {
        this.active = active;
    }

}


public interface IEquipmentPreAction
{
    //returns animation total duration
    //0 if failed
    float EquipmentUse();

    float EquipmentTakeOff();
    float EquipmentTakeOn();
}

public interface IEquipmentAction
{
    //returns 0 if not succesfull
    int EquipmentAction();
}

public interface ITargetHit
{
    //returns 0 if not succesfull
    int HitAction(Collider col);
}



//handles the equiptments animations
public class EquipmentAnimation
{

    public GameObject gameObj;
    public List<EquipmentAnimationStep> animation;

    public bool running = false;
    public int step;
    public int stepAmount;

    Vector3 startPos, endPos; //for the lerp
    Quaternion startAngle, endAngle;

    public float animationTime;

    //creates animation without steps
    public EquipmentAnimation(GameObject gameObj, int stepAmount)
    {
        this.gameObj = gameObj;
        this.stepAmount = stepAmount;

        animation = new List<EquipmentAnimationStep>();

    }


    //return and calculates total duration
    public float TotalDuration
    {
        get
        {
            float duration = 0;
            foreach (EquipmentAnimationStep step in animation)
                duration += step.duration;
            return duration;
        }
    }

    //creates one step and moves it to last
    public void AddStep(EquipmentAnimationStep step)
    {
        animation.Add(step);
    }

    //updates animation
    public void Update()
    {
        if (running)
        {
            animationTime += Time.deltaTime; //update timer

            //make sure not to divide by 0
            float progress = 1;
            if (animation[step].duration != 0)
                progress = animationTime / animation[step].duration;

            gameObj.GetComponent<Rigidbody>().transform.localRotation = Quaternion.Lerp(startAngle,
                endAngle, progress); //rotate the object
            gameObj.GetComponent<Rigidbody>().transform.localPosition = Vector3.Lerp(startPos,
                endPos, progress); //move the object

            //if step is over
            if (animationTime >= animation[step].duration)
            {
                step++;
                if (step >= stepAmount)
                    End();
                else
                    InitializeStep();
            }
        }
        
    }

    //sets the all variable rdy for next step
    public void InitializeStep()
    {
        startPos = gameObj.transform.localPosition;
        endPos = animation[step].position;
        startAngle = gameObj.transform.localRotation;
        endAngle = animation[step].angle;

        animationTime = 0;
    }

    //sets the step variables straight to the end
    public void JumpToStep(int step)
    {
        this.step = step;
        startPos = animation[step].position;
        startAngle = animation[step].angle;

        gameObj.GetComponent<Rigidbody>().transform.localRotation = startAngle;
        gameObj.GetComponent<Rigidbody>().transform.localPosition = startPos;

        InitializeStep();
    }

    //ends the animation
    public void End()
    {
        running = false;
        step = 0;
        animationTime = 0;
    }

    //starts the animation from the beginning
    public void Start()
    {
        running = true;
        step = 0;
        animationTime = 0;

        InitializeStep();
    }

}

/// <summary>
/// One step of the change equipment animation 
/// </summary>
public class EquipmentAnimationStep
{

    public Vector3 position;
    public Quaternion angle;
    public float duration;

    public EquipmentAnimationStep(Vector3 position, Quaternion angle, float duration)
    {
        this.position = position;
        this.angle = angle;
        this.duration = duration;
    }

}


