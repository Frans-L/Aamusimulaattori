using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the shitting animation
/// step 0 and final step coordinates cannot be changed with stepPosition
/// </summary>
public class Shitting : MonoBehaviour {

    public float triggerDistance = 1.75f;

    public int actionStep = 2;

    public List<float> stepDuration = new List<float>();
    public List<Vector3> stepPosition = new List<Vector3>();
    public AudioClip fartSound;

    bool isAnimationPlaying = false;
    float timer = 0;
    int step = 0;

    GameObject player;


    Vector3 orgPos; //coordniates where player is returned after animation
    Quaternion orgAngle; 

    Vector3 startPos, targetPos; //for the animation steps
    Quaternion startAngle, targetAngle;

	// Use this for initialization
	void Start () {

        player = GameObject.FindWithTag("Player");

	}

    // Update is called once per frame
    void Update () {


        if (Vector3.Distance(player.transform.position, transform.position) < triggerDistance && !isAnimationPlaying)
        {

            DisablePlayer.DisableMovement(true); //Disable player's movements
            DisablePlayer.DisableCollision(true);

            orgPos = player.transform.position;
            orgAngle = player.transform.rotation;

            InitializeFirstStep();     
        }


        if (isAnimationPlaying)
        {
            UpdateShitAnimation();
        }

    }


    void UpdateShitAnimation()
    {

        timer += Time.deltaTime;

        player.transform.position = Vector3.Lerp(startPos, targetPos, timer / stepDuration[step]);
        player.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, timer / stepDuration[step]);

        if (timer >= stepDuration[step])
        {

            if (step == actionStep)
            {
                Shit();
            }

            step++;

            if (step == stepDuration.Count - 1)
                InitializeLastStep();
            if(step >= stepDuration.Count)
                EndAnimation();
            else
                InitializeStep();

            

        }

    }

    void Shit()
    {
        GetComponent<AudioSource>().PlayOneShot(fartSound);
        GetChild(gameObject, "Shit").SetActive(true);

    }

    //when animations ends
    void EndAnimation()
    {
        DisablePlayer.DisableMovement(false); //Enables player's movements
        DisablePlayer.DisableCollision(false);

        GameObject plank = GetChild(gameObject, "Plank");
        plank.GetComponent<PickupAble>().enabled = true;
        plank.GetComponent<Burnable>().enabled = true;

        MissionHandler.SetCompleted("MissionLannoittaja"); //set mission completed

        Destroy(GetComponent<Shitting>());
    }

    //initialises next step
    void InitializeStep()
    {
        startPos = player.transform.position; //set variables to next step
        startAngle = player.transform.rotation;
        targetPos += stepPosition[step];
        timer = 0;
    }

    //initialises the first step
    void InitializeLastStep()
    {
        startPos = player.transform.position; //set variables to next step
        startAngle = player.transform.rotation;
        targetPos = orgPos;
        targetAngle = orgAngle;

        timer = 0;
    }

    //initialises the last step
    void InitializeFirstStep()
    {
        //set values that is needed for thr 1st lerp animation
        startPos = player.transform.position;
        targetPos = transform.position;
        startAngle = player.transform.rotation;
        targetAngle = Quaternion.Euler(new Vector3(0, -220, 0));

        timer = 0;
        isAnimationPlaying = true;
    }

    static GameObject GetChild(GameObject o, string name)
    {
        for (int i = 0; i < o.transform.childCount; i++)
        {
            if (o.transform.GetChild(i).name == name)
                return o.transform.GetChild(i).gameObject;
        }

        return null;
    }
}
