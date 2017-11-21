using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


/// <summary>
/// Handles the shitting animation
/// step 0 and final step coordinates cannot be changed with stepPosition
/// </summary>
public class WakeUp : MonoBehaviour
{

    public float triggerDistance = 1.75f;

    public int audioStep = 1;

    public List<float> stepDuration = new List<float>();
    public List<Vector3> stepPosition = new List<Vector3>();
    public List<Vector3> stepAngle = new List<Vector3>();
    public AudioClip audioStartClip, audioStepClip;

    bool isPlaying = false;
    bool isInitialized = false;
    float timer = 0;
    int step = 0;

    GameObject player;
    GameObject mainCamera;
    GameObject sleepingBag;

    PostProcessingBehaviour postProcess;

    Vector3 startPos, targetPos; //for the animation steps
    Quaternion startAngle, targetAngle;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        sleepingBag = GameObject.FindWithTag("SleepingBag");
        mainCamera = GameObject.FindWithTag("MainCamera");
        postProcess = mainCamera.GetComponent<PostProcessingBehaviour>();
        postProcess.profile.motionBlur.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < triggerDistance && !isInitialized)
        {
            DisablePlayer.DisableMovement(true); //Disable player's movements
            DisablePlayer.DisableCollision(true);
            DisablePlayer.DisableEquipment(true);
            GetChild(sleepingBag, "SleepingBag").GetComponent<Rigidbody>().isKinematic = true; //make sleeping bag inactive
            GetChild(sleepingBag, "AirMatress").GetComponent<Rigidbody>().isKinematic = true;
            InitializeFirstStep();
            isInitialized = true;
        }

        if (isPlaying)
        {
            UpdateAnimation();
        }
    }


    void UpdateAnimation()
    {
        timer += Time.deltaTime;

        player.transform.position = Vector3.Lerp(startPos, targetPos, timer / stepDuration[step]);
        player.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, timer / stepDuration[step]);

        if (timer >= stepDuration[step])
        {

            if (step == audioStep)
            {
                Audio();
            }

            step++;

            if (step == stepDuration.Count - 1)
                InitializeLastStep();
            if (step >= stepDuration.Count)
                EndAnimation();
            else
                InitializeStep();
        }
    }

    void Audio()
    {
        GetChild(player, "SpeechAudio").GetComponent<AudioSource>().PlayOneShot(audioStepClip);
    }

    //when animations ends
    void EndAnimation()
    {
        DisablePlayer.DisableMovement(false); //Enables player's movements
        DisablePlayer.DisableCollision(false);
        DisablePlayer.DisableEquipment(false);

        GetChild(sleepingBag, "SleepingBag").GetComponent<Rigidbody>().isKinematic = false; //make sleeping bag active again
        GetChild(sleepingBag, "AirMatress").GetComponent<Rigidbody>().isKinematic = false;

        postProcess.profile.motionBlur.enabled = false;
        isPlaying = false;

        Destroy(gameObject, 3); //Destroy wakeup script
    }

    //initialises next step
    void InitializeStep()
    {
        startPos = player.transform.position; //set variables to next step
        startAngle = player.transform.rotation;

        targetPos = stepPosition[step]; //target positon
        targetAngle = Quaternion.Euler(stepAngle[step]);

        timer = 0;
    }

    //initialises the first step
    void InitializeLastStep()
    {
        startPos = stepPosition[step]; //set variables to next step
        startAngle = Quaternion.Euler(stepAngle[step]);
        targetPos = stepPosition[step]; //target positon
        targetAngle = Quaternion.Euler(stepAngle[step]);

        timer = 0;
    }

    //initialises the last step
    void InitializeFirstStep()
    {
        GetComponent<AudioSource>().PlayOneShot(audioStartClip);

        //set values that is needed for thr 1st lerp animation
        startPos = stepPosition[step]; //start position
        startAngle = Quaternion.Euler(stepAngle[step]);

        step++;

        targetPos = stepPosition[step]; //target positon
        targetAngle = Quaternion.Euler(stepAngle[step]);

        timer = 0;
        isPlaying = true;

        postProcess.profile.motionBlur.settings = new MotionBlurModel.Settings()
        {
            shutterAngle = 270f,
            sampleCount = 10,
            frameBlending = 1f,
        };
        postProcess.profile.motionBlur.enabled = true;
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
